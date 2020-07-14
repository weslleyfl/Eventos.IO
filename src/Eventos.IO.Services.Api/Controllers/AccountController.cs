using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.OrganizadoresRoot.Commands;
using Eventos.IO.Domain.OrganizadoresRoot.Repository;
using Eventos.IO.Infra.CrossCutting.Identity.Authorization;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Controllers
{
    /// <summary>
    /// Autenticaçao via REST WEB API
    /// </summary> 
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IBus _bus;
        private readonly IOrganizadorRepository _organizadorRepository;

        // tokenConfigurations
        private readonly JwtTokenOptions _jwtTokenOptions;

        public AccountController(
                    UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    ILoggerFactory loggerFactory,
                    IOptions<JwtTokenOptions> jwtTokenOptions,
                    IBus bus,
                    INotificationHandler<DomainNotification> notifications,
                    IOrganizadorRepository organizadorRepository,
                    IUser user) : base(notifications, user, bus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _bus = bus;
            _jwtTokenOptions = jwtTokenOptions.Value;
            _organizadorRepository = organizadorRepository;

            ThrowIfInvalidOptions(_jwtTokenOptions);
            _logger = loggerFactory.CreateLogger<AccountController>();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("nova-conta")]
        public async Task<IActionResult> Register([FromBody] InputModel model)
        {
            // so teste para verificar com funciona o versionamento
            //if (version == 2)
            //{
            //    return Response(new { Message = "API V2 não disponível" });
            //}

            if (!ModelState.IsValid)
            {
                NotificarErroModelInvalida();
                return Response(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AdicionarErrosIdentity(result);
                return Response(model);
            }

            _logger.LogInformation("O usuário criou uma nova conta com senha.");

            var claimAdicionado = await AdicionarClaimsIniciais(user);

            if (!claimAdicionado)
            {
                _logger.LogInformation("Claims não adicioandas ao registrar o usuário.");

                NotificarErroModelInvalida();
                return Response(model);
            }

            var registroCommand = new RegistrarOrganizadorCommand(Guid.Parse(user.Id), model.Nome, model.CPF, user.Email);
            await _bus.SendCommand(registroCommand);

            if (!OperacaoValida())
            {
                await _userManager.DeleteAsync(user);
                return Response(model);
            }

            // ja logo o usuario
            // await _signInManager.SignInAsync(user, isPersistent: false);

            _logger.LogInformation(1, "Usuario criado com sucesso!");

            var response = await GerarTokenUsuario(new LoginViewModel { Email = model.Email, Password = model.Password });

            return Response(response);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("conta")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotificarErroModelInvalida();
                return Response(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, "Usuario logado com sucesso");

                var response = await GerarTokenUsuario(model);
                return Response(response);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(1, "Conta de usuário bloqueada.");
                NotificarErro(result.ToString(), "Conta de usuário bloqueada.");
                return Response(model);
            }

            NotificarErro(result.ToString(), "Falha ao realizar o login");
            return Response(model);

        }

        private async Task<bool> AdicionarClaimsIniciais(ApplicationUser user)
        {

            // Permissoes que todos os usuarios devem ter ao ser criados       
            var eventosLerClaim = await _userManager.AddClaimAsync(user, new Claim(type: "Eventos", value: "Ler"));

            if (!eventosLerClaim.Succeeded)
            {
                foreach (var error in eventosLerClaim.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // throw new InvalidOperationException($"Unexpected error occurred setting claim" +
                //    $" ({eventosLerClaim.ToString()}) for user with ID '{user.Id}'.");

                return false;
            }

            var eventosGravarClaim = await _userManager.AddClaimAsync(user, new Claim(type: "Eventos", value: "Gravar"));

            if (!eventosGravarClaim.Succeeded)
            {
                foreach (var error in eventosGravarClaim.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return false;
            }

            await _signInManager.RefreshSignInAsync(user);

            return true;
        }

        private async Task<object> GerarTokenUsuario(LoginViewModel login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            var userClaims = await _userManager.GetClaimsAsync(user);

            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, await _jwtTokenOptions.JtiGenerator()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtTokenOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64));

            //var userClaims = new List<Claim>{
            //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
            //        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtTokenOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            //        new Claim(JwtRegisteredClaimNames.Jti, await _jwtTokenOptions.JtiGenerator())
            //};

            var jwt = new JwtSecurityToken(
                  issuer: _jwtTokenOptions.Issuer,
                  audience: _jwtTokenOptions.Audience,
                  claims: userClaims,
                  notBefore: _jwtTokenOptions.NotBefore,
                  expires: _jwtTokenOptions.Expiration,
                  signingCredentials: _jwtTokenOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var orgUser = _organizadorRepository.ObterPorId(Guid.Parse(user.Id));

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtTokenOptions.ValidFor.TotalSeconds,
                user = new
                {
                    id = user.Id,
                    nome = orgUser.Nome,
                    email = orgUser.Email,
                    claims = userClaims.Select(c => new { c.Type, c.Value })
                }
            };


            return response;
        }


        private Task<object> GenerateToken(LoginViewModel user)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.Email, "Login"), // user.UserID
                new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Email) // user.UserID
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = _jwtTokenOptions.Expiration;

            // criando o token
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _jwtTokenOptions.Issuer,
                Audience = _jwtTokenOptions.Audience,
                SigningCredentials = _jwtTokenOptions.SigningCredentials,
                Subject = identity,
                IssuedAt = dataCriacao,
                Expires = dataExpiracao

            });
            var token = handler.WriteToken(securityToken);

            //return new Token()
            //{
            //    Authenticated = true,
            //    Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
            //    AccessToken = token,
            //    Message = "OK"
            //};

            //return new JwtTokenOptions()
            var response = new
            {
                authenticated = true,
                issuedAt = dataCriacao,
                validFor = dataExpiracao.TimeOfDay,
                accessToken = token,
                subject = "OK"
            };

            return Task.FromResult<object>(response);
        }

        private static long ToUnixEpochDate(DateTime date)
                => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtTokenOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtTokenOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtTokenOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtTokenOptions.JtiGenerator));
            }
        }

    }
}
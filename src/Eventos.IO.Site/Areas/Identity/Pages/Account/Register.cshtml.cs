﻿using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Eventos.IO.Site.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IOrganizadorAppService _organizadorAppService;
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IOrganizadorAppService organizadorAppService,
            IDomainNotificationHandler<DomainNotification> notifications)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _organizadorAppService = organizadorAppService;
            _notifications = notifications;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }


        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var claimAdicionado = await AdicionarClaimsIniciais(user);

                    if (!claimAdicionado)
                    {
                        return Page();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var organizador = new OrganizadorViewModel
                    {
                        Id = Guid.Parse(user.Id),
                        Email = user.Email,
                        Nome = Input.Nome,
                        CPF = Input.CPF
                    };

                    _organizadorAppService.Registrar(organizador);

                    if (_notifications.HasNotifications())
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, $"Erro: Usuário {Input.Nome} Id {user.Id} não registrado.");

                        return Page();
                    }

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<bool> AdicionarClaimsIniciais(ApplicationUser user)
        {

            // Permissoes que todos os usuarios devem ter ao ser criados
            //user.Claims.Add(new IdentityUserClaim<string> { ClaimType = "Eventos", ClaimValue = "Ler" });
            //user.Claims.Add(new IdentityUserClaim<string> { ClaimType = "Eventos", ClaimValue = "Gravar" });

            var eventosLerClaim = await _userManager.AddClaimAsync(user, new Claim(type: "Eventos", value: "Ler"));

            if (!eventosLerClaim.Succeeded)
            {
                foreach (var error in eventosLerClaim.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                //throw new InvalidOperationException($"Unexpected error occurred setting claim" +
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

            //await _signInManager.RefreshSignInAsync(user);

            return true;
        }
    }
}

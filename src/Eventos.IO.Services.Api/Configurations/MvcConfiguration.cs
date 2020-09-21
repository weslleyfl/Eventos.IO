using Eventos.IO.Infra.CrossCutting.AspNetFilters;
using Eventos.IO.Infra.CrossCutting.Identity.Authorization;
using Eventos.IO.Infra.CrossCutting.Identity.Data;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Eventos.IO.Services.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Eventos.IO.Services.Api.Configurations
{
    public static class MvcConfiguration
    {
        public static void AddMvcSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Ativando a utilização do ASP.NET Identity, a fim de
            // permitir a recuperação de seus objetos via injeção de
            // dependências
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;

                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(300);
                options.SlidingExpiration = true;

                //options.AccessDeniedPath =
                options.Cookie.Name = "Cookie_Do_Identity";
            });


            #region " JWT configuration"

            // configure strongly typed settings objects
            var appSettingsSection = configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(appSettingsSection);

            // SecretKey é criado aqui
            var secretKey = appSettingsSection[nameof(AppSettings.Secret)];
            // Credentials
            var _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // mvc

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PodeLerEventos", policy => policy.RequireClaim("Eventos", "Ler"));
                options.AddPolicy("PodeGravar", policy => policy.RequireClaim("Eventos", "Gravar"));
            });

            // appsettings
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtTokenOptions));

            // populando os options jwt para ser usado no accountController
            services.Configure<JwtTokenOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtTokenOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtTokenOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // configure
            // https://docs.microsoft.com/pt-br/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-3.1
            // if you face same problem JWT did not support cookie with token together so you must remove
            // when you need to authorize any action by token you can use this attribute
            // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            // to any actions. when you need to authorize any action by cookie you need to add - [Authorize]            
            services.AddAuthentication(authOptions =>
            {
                // JWT Bearer authentication scheme will use the token that is provided as part of the Authorization 
                // authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // should prompt the user to authenticate themselves.  This could for example mean that the user gets redirected to a login form
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                // bearerOptions.RequireHttpsMetadata = false; // disable so em desenvolviemtno
                // bearerOptions.SaveToken = true;

                var paramsValidation = bearerOptions.TokenValidationParameters;

                paramsValidation.ValidateIssuer = true;
                paramsValidation.ValidIssuer = jwtAppSettingOptions[nameof(JwtTokenOptions.Issuer)];

                paramsValidation.ValidateAudience = true;
                paramsValidation.ValidAudience = jwtAppSettingOptions[nameof(JwtTokenOptions.Audience)];

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.IssuerSigningKey = _signingKey;

                // Obrigar um tempo de expiraçao do token
                paramsValidation.RequireExpirationTime = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;


            });

            #endregion

           

        }

        public static void AddMvcConfig(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));

                // Caputrar exception geral - Filter Exception
                // options.Filters.Add(typeof(GlobalExceptionHandlingFilter)); // By type
                
                // TODO: Implementar o serilog aqui
                options.Filters.Add(typeof(GlobalActionLogger));
               
                // MVC com restrição de XML e adição de filtro de ações.
                options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
               

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions((options) =>
            {
                 // Remove null fields from API JSON response
                 options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }
    }
}

using Eventos.IO.Infra.CrossCutting.Identity.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Security
{
    public static class JwtSecurityExtension
    {
        public static IServiceCollection AddJwtSecurity(
            this IServiceCollection services,
            SigningConfigurations signingConfigurations,
             //TokenConfigurations tokenConfigurations;
             JwtTokenOptions tokenConfigurations)
        {
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            }).AddJwtBearer(bearerOptions =>
            {
                // bearerOptions.RequireHttpsMetadata = false; // disable so em desenvolviemtno
                bearerOptions.SaveToken = true;
                
                var paramsValidation = bearerOptions.TokenValidationParameters;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;

                paramsValidation.ValidateAudience = true;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;

                paramsValidation.ValidateIssuer = true;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;


                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Obrigar um tempo de expiraçao do token
                paramsValidation.RequireExpirationTime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;

           
                tokenConfigurations.SigningCredentials = signingConfigurations.SigningCredentials;

            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser()
                    .Build());

                auth.AddPolicy("PodeLerEventos", policy => policy.RequireClaim("Eventos", "Ler"));
                auth.AddPolicy("PodeGravar", policy => policy.RequireClaim("Eventos", "Gravar"));

            });

            return services;
        }
    }
}

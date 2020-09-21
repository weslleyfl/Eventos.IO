using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Eventos.IO.Services.Api.Configurations
{
    /// <summary>
    /// https://thecodebuzz.com/jwt-authorize-swagger-using-ioperationfilter-asp-net-core/
    /// </summary>
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Eventos.IO API",
                    Description = "API do site Eventos.IO",
                    TermsOfService = new Uri("http://eventos.io/terms"),
                    Contact = new OpenApiContact { Name = "Desenvolvedor X", Email = "email@eventos.io", Url = new Uri("http://eventos.io") },
                    License = new OpenApiLicense { Name = "MIT", Url = new Uri("http://eventos.io/licensa") }
                });

                s.SwaggerDoc("v2", new OpenApiInfo { Title = "Eventos.IO API - V2", Version = "v2" });

                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1425
                s.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });

                s.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            //services.ConfigureSwaggerGen(opt =>
            //{
            //    opt.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            //});
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Version = description.ApiVersion.ToString(),
                Title = $"Eventos.IO API {description.ApiVersion}",
                Description = "API do site Eventos.IO",
                TermsOfService = new Uri("http://eventos.io/terms"),
                Contact = new OpenApiContact { Name = "Desenvolvedor X", Email = "email@eventos.io", Url = new Uri("http://eventos.io") },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("http://eventos.io/licensa") }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
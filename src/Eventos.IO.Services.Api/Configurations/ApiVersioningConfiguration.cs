using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Configurations
{
    public static class ApiVersioningConfiguration
    {
        public static void AddApiVersioningConfig(this IServiceCollection services)
        {
            // Add API Versioning to the service container to your project
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number  true to use
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
                // multiple “ IApiVersionReader” implementations are combined to work with all the styles.
                //config.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(),
                //                                                   new HeaderApiVersionReader("api-version"),
                //                                                   new MediaTypeApiVersionReader("v"));

                config.ApiVersionReader = ApiVersionReader.Combine(new MediaTypeApiVersionReader("v"));

            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}

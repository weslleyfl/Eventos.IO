using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Middlewares
{
    /// <summary>
    /// https://www.wellingtonjhn.com/posts/tratamento-global-de-exceptions-no-asp.net-core/
    /// </summary>
    public static class ExceptionHandlerExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler((builder) =>
            {

                builder.Run(async context =>
                {

                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                        logger.LogWarning($"Erro não tratado: {exceptionHandlerFeature.Error}");

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";

                        var json = new
                        {
                            // context.Response.StatusCode,
                            // Message = "Um erro ocorreu enquanto voce processava uma request",
                            // Detailed = exceptionHandlerFeature.Error,
                            success = false,
                            errors = exceptionHandlerFeature.Error.Message
                        };
                       
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(json));

                    }

                });

            });

        }
    }
}

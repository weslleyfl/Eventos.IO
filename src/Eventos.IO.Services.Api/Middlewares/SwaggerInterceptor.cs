using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Middlewares
{
    public class SwaggerInterceptor
    {
        private readonly RequestDelegate _next;

        public SwaggerInterceptor(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var uri = context.Request.Path.ToString();
            if (uri.StartsWith("/swagger/index.html"))
            {
                var param = context.Request.QueryString.Value;

                if (!param.Equals("?key=123"))
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"result:\" \"Not Found\"}", System.Text.Encoding.UTF8);
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }

    public static class SwaggerInterceptorExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorizedInterceptor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerInterceptor>();
        }
    }

}

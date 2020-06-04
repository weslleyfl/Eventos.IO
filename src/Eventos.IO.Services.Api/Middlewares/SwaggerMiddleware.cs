using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Middlewares
{
    public class SwaggerMiddleware
    {
       
        private readonly RequestDelegate _next;
    
        public SwaggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUser user)
        {
            
            if (context.Request.Path.StartsWithSegments("/swagger")
                && !user.IsAuthenticated())
            {
                // context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next.Invoke(context);
        }
    }

    public static class SwaggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerMiddleware>();
        }
    }
}
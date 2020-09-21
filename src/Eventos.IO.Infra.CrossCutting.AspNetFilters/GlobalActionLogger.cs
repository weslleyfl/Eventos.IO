using ElmahCore;
//using Elmah.Io.Client;
//using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Eventos.IO.Infra.CrossCutting.AspNetFilters
{
    public class GlobalActionLogger : IActionFilter
    {
        private readonly ILogger<GlobalActionLogger> _logger;
        private readonly IHostingEnvironment _hostingEnviroment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalActionLogger(ILogger<GlobalActionLogger> logger,
                                  IHostingEnvironment hostingEnviroment,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _hostingEnviroment = hostingEnviroment;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null) return;

            if (_hostingEnviroment.IsDevelopment())
            {
                var data = new
                {
                    Version = "v1.0",
                    User = context.HttpContext.User.Identity.Name,
                    IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Hostname = context.HttpContext.Request.Host.ToString(),
                    AreaAccessed = context.HttpContext.Request.GetDisplayUrl(),
                    Action = context.ActionDescriptor.DisplayName,
                    TimeStamp = DateTime.Now,
                    Exceptions = context.Exception
                };

                _logger.LogWarning("Log de Auditoria {message} ", data.ToString());
                //_logger.LogInformation(1, data.ToString(), "Log de Auditoria");
                // _httpContextAccessor.HttpContext.RiseError(new ArgumentException(data.ToString(), context.Exception));

            }

            if (_hostingEnviroment.IsProduction())
            {
                var message = new // CreateMessage
                {
                    Version = "v1.0",
                    Application = "Eventos.IO",
                    Source = "GlobalActionLoggerFilter",
                    User = context.HttpContext.User.Identity.Name,
                    Hostname = context.HttpContext.Request.Host.Host,
                    Url = context.HttpContext.Request.GetDisplayUrl(),
                    DateTime = DateTime.Now,
                    Method = context.HttpContext.Request.Method,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    //Cookies = context.HttpContext.Request?.Cookies?.Keys.Select(k => new Item(k, context.HttpContext.Request.Cookies[k])).ToList(),
                    //Form = Form(context.HttpContext),
                    //ServerVariables = context.HttpContext.Request?.Headers?.Keys.Select(k => new Item(k, context.HttpContext.Request.Headers[k])).ToList(),
                    //QueryString = context.HttpContext.Request?.Query?.Keys.Select(k => new Item(k, context.HttpContext.Request.Query[k])).ToList(),
                    Exceptions = context.Exception //?.ToDataList(),
                    //Detail = JsonConvert.SerializeObject(new { DadoExtra = "Dados a mais", DadoInfo = "Pode ser um Json" })
                };

                // Serilog
                //_httpContextAccessor.HttpContext.RiseError(new ArgumentException(message.ToString()));
                _logger.LogWarning("Log de Auditoria {message} ", message.ToString());
                //_logger.LogWarning("The person {PersonId} could not be found.", message.User);
                //_logger.LogInformation("Log de Auditoria - Data Acesso {data} - Usuario {user} - Local {host}",
                //                       message.DateTime, message.User, message.Hostname);


                //var client = ElmahioAPI.Create("8f46c7cd9bfe4a618abf7a5ea652d0d9");
                //client.Messages.Create(new Guid("19ad15fd-5158-4b7a-b36d-ab56dfe4500a").ToString(), message);
            }
        }
            
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
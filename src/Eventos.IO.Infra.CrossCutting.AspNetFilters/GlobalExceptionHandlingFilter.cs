using ElmahCore;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;

namespace Eventos.IO.Infra.CrossCutting.AspNetFilters
{
    public class GlobalExceptionHandlingFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionHandlingFilter> _logger;
        private readonly IHostingEnvironment _hostingEnviroment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBus _mediator;

        public GlobalExceptionHandlingFilter(ILogger<GlobalExceptionHandlingFilter> logger,
                                             IHostingEnvironment hostingEnviroment,
                                             IModelMetadataProvider modelMetadataProvider,
                                             IHttpContextAccessor httpContextAccessor,
                                             IBus mediator)
        {
            _logger = logger;
            _hostingEnviroment = hostingEnviroment;
            _modelMetadataProvider = modelMetadataProvider;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;            
        }

        public void OnException(ExceptionContext context)
        {

            if (_hostingEnviroment.IsProduction())
            {
                _logger.LogError(1, context.Exception, context.Exception.Message);
                _mediator.RaiseEvent(new DomainNotification(nameof(GlobalExceptionHandlingFilter), context.Exception.Message));

            }

            var result = new ViewResult() { ViewName = "Error", };
            result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                { "MensagemErro", context.Exception.Message }
            };

            // TODO: Pass additional detailed data via ViewData
            // Aqui posso validar qual tipo de erro deu, qual açao tomar, mandar um email, mandar para outra
            // pagina , logger no windows e etc
            var exception = new InvalidOperationException(message: context.Exception.Message,
                                                          innerException: context.Exception);
            _httpContextAccessor.HttpContext.RiseError(exception);

            context.ExceptionHandled = true;
            context.Result = result;

        }
    }
}

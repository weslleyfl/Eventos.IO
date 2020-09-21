using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Infra.CrossCutting.AspNetFilters;
using Eventos.IO.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Controllers
{

    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public class EventosController : BaseController
    {
        private readonly ILogger<EventosController> _logger;
        private readonly IEventoAppService _eventoAppService;
        private readonly IBus _bus;
        private readonly IEventoRepository _eventoRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public EventosController(INotificationHandler<DomainNotification> notifications,
                                 IUser user,
                                 IBus bus, IEventoAppService eventoAppService,
                                 IEventoRepository eventoRepository,
                                 IMapper mapper,
                                 IMemoryCache cache,
                                 ILogger<EventosController> logger
                                 ) : base(notifications, user, bus)
        {
            _eventoAppService = eventoAppService;
            _eventoRepository = eventoRepository;
            _mapper = mapper;
            _bus = bus;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        [Route("eventos")]
        [AllowAnonymous]
        public IEnumerable<EventoViewModel> Get()
        {
            return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterTodos());
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("eventos/{id:guid}")]
        public EventoViewModel Get(Guid id)
        {
            try
            {
                _logger.LogInformation("Request Evento", $"{id}");

                return _mapper.Map<EventoViewModel>(_eventoRepository.ObterPorId(id));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.ToLogString(Environment.StackTrace));               
                return new EventoViewModel();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("eventos/categorias")]
        public IEnumerable<CategoriaViewModel> ObterCategorias()
        {
            string cacheKey = "GreetingCategorias-Invoke";            

            var cacheEntry = _cache.GetOrCreate(cacheKey, (entry) => {
                // configuração
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                entry.SetSize(1024);

                // retorno do objeto
                return _mapper.Map<IEnumerable<CategoriaViewModel>>(_eventoRepository.ObterCategorias());
            });

            return cacheEntry;
        }

        [HttpGet]
        [Authorize(Policy = "PodeLerEventos")]
        [Route("eventos/meus-eventos")]
        public IEnumerable<EventoViewModel> ObterMeusEventos()
        {
            return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterEventoPorOrganizador(base.OrganizadorId));
        }

        [HttpGet]
        [Authorize(Policy = "PodeLerEventos")]
        [Route("eventos/meus-eventos/{id:guid}")]
        public IActionResult ObterMeuEventoPorId(Guid id)
        {
            var evento = _mapper.Map<EventoViewModel>(_eventoRepository.ObterMeuEventoPorId(id, base.OrganizadorId));
            return (evento == null) ? StatusCode(404) : Response(evento);
        }

        [HttpPost]
        [Route("eventos")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Post([FromBody]EventoViewModel eventoViewModel)
        {
            if (!ModelStateValida())
            {
                return Response();
            }

            var eventoCommand = _mapper.Map<RegistrarEventoCommand>(eventoViewModel);

            _bus.SendCommand(eventoCommand);
            return Response(eventoCommand);
        }

        [HttpPut]
        [Route("eventos")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Put([FromBody]EventoViewModel eventoViewModel)
        {
            if (!ModelStateValida())
            {
                return Response();
            }

            var eventoCommand = _mapper.Map<AtualizarEventoCommand>(eventoViewModel);
            _bus.SendCommand(eventoCommand);

            return Response(eventoCommand);
        }

        [HttpDelete]
        [Route("eventos/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Delete(Guid id)
        {
            var eventoViewModel = new EventoViewModel() { Id = id };
            var eventoCommand = _mapper.Map<ExcluirEventoCommand>(eventoViewModel);

            _bus.SendCommand(eventoCommand);
            //_eventoAppService.Excluir(id);

            return Response(eventoCommand);
        }

        [HttpPost]
        [Route("endereco")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Post([FromBody]EnderecoViewModel enderecoViewModel)
        {
            if (!ModelStateValida())
            {
                return Response();
            }

            var eventoCommand = _mapper.Map<IncluirEnderecoEventoCommand>(enderecoViewModel);

            _bus.SendCommand(eventoCommand);
            //_mediator.EnviarComando(eventoCommand);
            return Response(eventoCommand);
        }

        [HttpPut]
        [Route("endereco")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Put([FromBody]EnderecoViewModel enderecoViewModel)
        {
            if (!ModelStateValida())
            {
                return Response();
            }

            var eventoCommand = _mapper.Map<AtualizarEnderecoEventoCommand>(enderecoViewModel);

            _bus.SendCommand(eventoCommand);
            //_mediator.EnviarComando(eventoCommand);
            return Response(eventoCommand);
        }

        private bool ModelStateValida()
        {
            if (ModelState.IsValid) return true;

            NotificarErroModelInvalida();
            return false;
        }

    }
}

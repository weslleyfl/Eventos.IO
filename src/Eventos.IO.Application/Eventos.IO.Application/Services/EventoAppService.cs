﻿using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Application.Services
{
    public class EventoAppService : IEventoAppService
    {
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IEventoRepository _eventoRepository;
        private readonly IUser _user;

        public EventoAppService(IBus bus,
                                IMapper mapper,
                                IEventoRepository eventoRepository,
                                IUser user
                                )
        {
            _bus = bus;
            _mapper = mapper;
            _eventoRepository = eventoRepository;
            _user = user;
        }


     
        public IEnumerable<EventoViewModel> ObterEventoPorOrganizador(Guid organizadorId)
        {
            return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterEventoPorOrganizador(organizadorId));
        }

        public EventoViewModel ObterPorId(Guid id)
        {
            // Leitura é feita direto ao respositorio via serviço desta camada, so leitura
            return _mapper.Map<EventoViewModel>(_eventoRepository.ObterPorId(id));
        }

        public IEnumerable<EventoViewModel> ObterTodos()
        {
            return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterTodos());
        }
              
        public IEnumerable<EventoViewModel> ObterMeusEventos(Guid organizadorId)
        {
            return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterEventoPorOrganizador(organizadorId));
        }
    
        public EventoViewModel ObterMeuEventoPorId(Guid id, Guid organizadorId)
        {
            var evento = _mapper.Map<EventoViewModel>(_eventoRepository.ObterMeuEventoPorId(id, organizadorId));
            return evento;
        }

        public void Atualizar(EventoViewModel eventoViewModel)
        {
            // TODO: Validar se o organizador é dono do evento
            // new AtualizarEventoCommand() uso o mapper para isso
            var atualizarEventoCommand = _mapper.Map<AtualizarEventoCommand>(eventoViewModel);
            _bus.SendCommand(atualizarEventoCommand);

        }

        public void Excluir(Guid id)
        {
            _bus.SendCommand(new ExcluirEventoCommand(id));
        }

        public void AdicionarEndereco(EnderecoViewModel enderecoViewModel)
        {
            var enderecoCommand = _mapper.Map<IncluirEnderecoEventoCommand>(enderecoViewModel);
            _bus.SendCommand(enderecoCommand);
        }

        public void AtualizarEndereco(EnderecoViewModel enderecoViewModel)
        {
            var enderecoCommand = _mapper.Map<AtualizarEnderecoEventoCommand>(enderecoViewModel);
            _bus.SendCommand(enderecoCommand);
        }

        public EnderecoViewModel ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoViewModel>(_eventoRepository.ObterEnderecoPorId(id));
        }

        public void Registrar(EventoViewModel eventoViewModel)
        {
            var registroCommand = _mapper.Map<RegistrarEventoCommand>(eventoViewModel);
            //var registroCommand = _mapper.Map<EventoViewModel, RegistrarEventoCommand>(eventoViewModel);
            _bus.SendCommand(registroCommand);
        }
        
        public void Dispose()
        {
            _eventoRepository.Dispose();
        }
    }
}

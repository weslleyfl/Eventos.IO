﻿using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.OrganizadoresRoot.Commands;
using Eventos.IO.Domain.OrganizadoresRoot.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Application.Services
{
    public class OrganizadorAppService : IOrganizadorAppService
    {
        private readonly IMapper _mapper;
        private readonly IOrganizadorRepository _organizadorRepository;
        private readonly IBus _bus;

        public OrganizadorAppService(IMapper mapper, IOrganizadorRepository organizadorRepository, IBus bus)
        {
            _mapper = mapper;
            _organizadorRepository = organizadorRepository;
            _bus = bus;
        }

        public void Registrar(OrganizadorViewModel organizadorViewModel)
        {
            var registroCommand = _mapper.Map<RegistrarOrganizadorCommand>(organizadorViewModel);
            _bus.SendCommand(registroCommand);
        }

        public void Dispose()
        {
            _organizadorRepository.Dispose();
        }
    }
}

using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Organizadores.Events;
using Eventos.IO.Domain.OrganizadoresRoot.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eventos.IO.Domain.OrganizadoresRoot.Commands
{
    public class OrganizadorCommandHandler : CommandHandler,
        IRequestHandler<RegistrarOrganizadorCommand, bool>
    {

        private readonly IOrganizadorRepository _organizadorRepository;
        private readonly IBus _mediator;

        public OrganizadorCommandHandler(
            IUnitOfWork uow,
            INotificationHandler<DomainNotification> notifications,
            IOrganizadorRepository organizadorRepository,
            IBus mediator
            ) : base(uow, mediator, notifications)
        {

            _organizadorRepository = organizadorRepository;
            _mediator = mediator;
        }

        public Task<bool> Handle(RegistrarOrganizadorCommand message, CancellationToken cancellationToken)
        {
            var organizador = new Organizador(message.Id, message.Nome, message.CPF, message.Email);

            if (!OrganizadorValido(organizador))
            {
                return Task.FromResult(false);
            }

            // TODO: Validar cpf e email valido
            var organizadorExistente = _organizadorRepository.Buscar(o => o.CPF == organizador.CPF || o.Email == organizador.Email);

            if (organizadorExistente.Any())
            {
                _mediator.RaiseEvent(new DomainNotification(message.MessageType, "CPF ou e-mail já utilizados"));
            }

            // TODO: Add no respositorio
            _organizadorRepository.Adicionar(organizador);

            if (Commit())
            {
                _mediator.RaiseEvent(new OrganizadorRegistradoEvent(organizador.Id, organizador.Nome, organizador.CPF, organizador.Email));
            }

            return Task.FromResult(true);
        }

        private bool OrganizadorValido(Organizador organizador)
        {
            if (organizador.EhValido()) return true;

            NotificarValidacoesErro(organizador.ValidationResult);
            return false;

        }
    }
}

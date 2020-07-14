using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Organizadores.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eventos.IO.Domain.OrganizadoresRoot.Events
{
    public class OrganizadorEventHandler : INotificationHandler<OrganizadorRegistradoEvent>
    {

        public Task Handle(OrganizadorRegistradoEvent message, CancellationToken cancellationToken)
        {
            // TODO: Enviar um email?
            return Task.CompletedTask;
        }
    }
}

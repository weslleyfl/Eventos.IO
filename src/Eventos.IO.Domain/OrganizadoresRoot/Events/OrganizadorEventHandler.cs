using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Organizadores.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.OrganizadoresRoot.Events
{
    public class OrganizadorEventHandler : IHandler<OrganizadorRegistradoEvent>
    {

        public void Handle(OrganizadorRegistradoEvent message)
        {
            // TODO: Enviar um email?
        }
    }
}

using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.EventosRoot.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot
{
    public class EventoEventHandler : IHandler<EventoAtualizadoEvent>, IHandler<EventoExcluidoEvent>, IHandler<EventoRegistradoEvent>
    {
        public void Handle(EventoAtualizadoEvent message)
        {
            // Enviar email - Posso fazer log
        }

        public void Handle(EventoExcluidoEvent message)
        {
            // Enviar email - Posso fazer log
        }

        public void Handle(EventoRegistradoEvent message)
        {
            // Enviar email - Posso fazer log
        }
    }
}

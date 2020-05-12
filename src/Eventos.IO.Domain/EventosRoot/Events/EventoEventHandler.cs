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
            // Gravar no banco

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento atualizado com sucesso ");
        }

        public void Handle(EventoExcluidoEvent message)
        {
            // Enviar email - Posso fazer log

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento excluido com sucesso ");

        }

        public void Handle(EventoRegistradoEvent message)
        {
            // Enviar email - Posso fazer log


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento registrado com sucesso ");
        }
    }
}

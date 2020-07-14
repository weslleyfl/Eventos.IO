using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.EventosRoot.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eventos.IO.Domain.EventosRoot
{
    public class EventoEventHandler :
                INotificationHandler<EventoAtualizadoEvent>,
                INotificationHandler<EventoExcluidoEvent>,
                INotificationHandler<EventoRegistradoEvent>,
                INotificationHandler<EnderecoEventoAdicionadoEvent>,
                INotificationHandler<EnderecoEventoAtualizadoEvent>
    {
        public Task Handle(EventoAtualizadoEvent message, CancellationToken cancellationToken)
        {
            // Enviar email - Posso fazer log
            // Gravar no banco

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento atualizado com sucesso ");

            return Task.CompletedTask;
        }

        public Task Handle(EventoExcluidoEvent message, CancellationToken cancellationToken)
        {
            // Enviar email - Posso fazer log

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento excluido com sucesso ");

            return Task.CompletedTask;

        }

        public Task Handle(EventoRegistradoEvent message, CancellationToken cancellationToken)
        {
            // Enviar email - Posso fazer log

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Evento registrado com sucesso ");

            return Task.CompletedTask;
        }

        public Task Handle(EnderecoEventoAdicionadoEvent message, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Endereco do evento adicionado com sucesso");

            return Task.CompletedTask;
        }

        public Task Handle(EnderecoEventoAtualizadoEvent message, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Endereco do evento atualizado com sucesso");

            return Task.CompletedTask;
        }
    }
}

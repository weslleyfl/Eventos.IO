using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Commands;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot;
using System;

namespace ConsoleTesting
{
    partial class Program
    {
        public class FakeBus : IBus
        {
            public void RaiseEvent<T>(T theEvent) where T : Event
            {
                Publish(theEvent);
            }

            public void SendCommand<T>(T theCommand) where T : Command
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Comando lançado {theCommand.MessageType } com sucesso ");

                Publish(theCommand);
            }

            private static void Publish<T>(T message) where T : Message
            {
                var msgType = message.MessageType;

                if (msgType.Equals("DomainNotification"))
                {
                    var obj = new DomainNotificationHandler();
                    ((IDomainNotificationHandler<T>)obj).Handle(message);
                }

                if (msgType.Equals("RegistrarEventoCommand") || msgType.Equals("ExcluirEventoCommand") ||
                    msgType.Equals("AtualizarEventoCommand"))
                {
                    var obje = new EventoCommandHandlers(new FakeEventoRepository(), new FakeUow(), new FakeBus(), new DomainNotificationHandler(), null);
                    ((IHandler<T>)obje).Handle(message);
                }

                if (msgType.Equals("EventoRegistradoEvent") || msgType.Equals("EventoAtualizadoEvent") ||
                   msgType.Equals("EventoExcluidoEvent"))
                {
                    var obj = new EventoEventHandler();
                    ((IHandler<T>)obj).Handle(message);
                }



            }
        }

    }
}

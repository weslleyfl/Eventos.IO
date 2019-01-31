using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Commands;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.EventosRoot.Commands;
using System;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var bus = new FakeBus();

            // Registro com sucesso.

            var cmd = new RegistrarEventoCommand("weslley", DateTime.Now.AddDays(1), DateTime.Now.AddDays(2),
                                                true, 0, true, "Fundep");
            


            Console.Read();

            //var evento = new Evento(
            //    "",
            //    DateTime.Now,
            //    DateTime.Now,
            //    true,
            //    90,
            //    true,
            //    ""
            //    );


            //var evento2 = new Evento(
            //    "Evento de TI",
            //    DateTime.Now,
            //    DateTime.Now,
            //    true,
            //    456.89M,
            //    true,
            //    "Weslley corporation"
            //    );

            //Console.WriteLine(evento.ToString());
            //Console.WriteLine(evento.Equals(evento2));

            //Console.WriteLine(evento.EhValido());

            //if (!evento.ValidationResult.IsValid)
            //{
            //    foreach (var erro in evento.ValidationResult.Errors)
            //    {
            //        Console.WriteLine(erro.ErrorMessage);
            //    }

            //}


            //if (!evento.EhValido())
            //    Console.Write(evento.ErrosValidacao["Valor"]);



        }

        public class FakeBus : IBus
        {
           

            public void RaiseEvent<T>(T theEvent) where T : Event
            {
                Publish(theEvent);
            }

            public void SendCommand<T>(T theCommand) where T : Command
            {
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
                    var obje = new EventoCommandHandlers()
                }

            }
        }
    }
}

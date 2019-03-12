using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.EventosRoot.Commands;
using System;

namespace ConsoleTesting
{
    partial class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            var bus = new FakeBus();

            // Registro com sucesso.
            //var cmd = new RegistrarEventoCommand("weslley", DateTime.Now.AddDays(1), DateTime.Now.AddDays(2),
            //                                    true, 0, true, "Fundep", Guid.NewGuid(), Guid.NewGuid(), null);

            //Inicio(cmd);
            //bus.SendCommand(cmd);
            //Fim(cmd);

            //// Registro com falha
            //cmd = new RegistrarEventoCommand("", DateTime.Now.AddDays(2), DateTime.Now.AddDays(1),
            //                                    false, 0, false, "", Guid.NewGuid(), Guid.NewGuid(), null);
            //Inicio(cmd);
            //bus.SendCommand(cmd);
            //Fim(cmd);


            //// Atualizar o evento
            //var cmd2 = new AtualizarEventoCommand(Guid.NewGuid(), "weslley", "", "", DateTime.Now.AddDays(1),
            //                                    DateTime.Now.AddDays(2), true, 0, true, "Fundep");

            //Inicio(cmd2);
            //bus.SendCommand(cmd2);
            //Fim(cmd2);

            ////excluir
            //var cmd3 = new ExcluirEventoCommand(Guid.NewGuid());
            //Inicio(cmd3);
            //bus.SendCommand(cmd3);
            //Fim(cmd3);



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

        private static void Inicio(Message message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Inicio do comando " + message.MessageType);
        }

        private static void Fim(Message message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Fim do comando " + message.MessageType);
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("******************");
            Console.WriteLine("");

        }
    }
}
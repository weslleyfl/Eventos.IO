using Eventos.IO.Domain.Models;
using System;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var evento = new Evento(
                "",
                DateTime.Now,
                DateTime.Now,
                true,
                90,
                true,
                ""
                );


            var evento2 = new Evento(
                "Evento de TI",
                DateTime.Now,
                DateTime.Now,
                true,
                456.89M,
                true,
                "Weslley corporation"
                );

            Console.WriteLine(evento.ToString());
            Console.WriteLine(evento.Equals(evento2));

            Console.WriteLine(evento.EhValido());

            if (!evento.ValidationResult.IsValid)
            {
                foreach (var erro in evento.ValidationResult.Errors)
                {
                    Console.WriteLine(erro.ErrorMessage);
                }

            }


            //if (!evento.EhValido())
            //    Console.Write(evento.ErrosValidacao["Valor"]);

                Console.Read();

        }
    }
}

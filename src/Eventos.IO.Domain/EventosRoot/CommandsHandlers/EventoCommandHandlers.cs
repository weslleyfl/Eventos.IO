using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot.CommandsHandlers
{
    public class EventoCommandHandlers : CommandHandler, IHandler<RegistrarEventoCommand>, IHandler<AtualizarEventoCommand>, IHandler<ExcluirEventoCommand>
    {
        // Injeçao de dependencia
        private readonly IEventoRespository _eventoRespository;

        public EventoCommandHandlers(IEventoRespository eventoRespository, IUnitOfWork uow) : base(uow)
        {
            _eventoRespository = eventoRespository;
        }

        public void Handle(RegistrarEventoCommand message)
        {
            var evento = new Evento(nome: message.Nome, dateInicio: message.DataInicio, dataFim: message.DataFim, gratuito: message.Gratuito, valor: message.Valor, online: message.Online, nomeEmpresa: message.NomeDaEmpresa);

            if (!evento.EhValido())
            {
                NotificarValidacoesErro(evento.ValidationResult);
                return;
            }

            // TODO: Validaçao do negocio no command
            // Pode tratar regras de negocio aqui tambem, se nao for o caso de tratar lá na entidade - Evento
            // Validaçoes de Negocio Exemplo
            // O Organizador pode registrar um evento? (tipo sera que ele pagou a taxa de abertura)

            // Persistencia
            _eventoRespository.Add(evento);

            if (Commit())
            {
                //Notificar um processo concluido
                Console.WriteLine("Evento registrado com sucesso");
            }
        }

        public void Handle(AtualizarEventoCommand message)
        {
            throw new NotImplementedException();
        }

        public void Handle(ExcluirEventoCommand message)
        {
            throw new NotImplementedException();
        }
    }
}

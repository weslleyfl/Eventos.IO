using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Events;
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
        private readonly IBus _bus;
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public EventoCommandHandlers(IEventoRespository eventoRespository,
                                     IUnitOfWork uow,
                                     IBus bus,
                                     IDomainNotificationHandler<DomainNotification> notifications) : base(uow, bus, notifications)
        {
            _eventoRespository = eventoRespository;
            _bus = bus;
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
                _bus.RaiseEvent(new EventoRegistradoEvent(evento.Id, evento.Nome, evento.DataInicio, evento.DataFim, evento.Gratuito, evento.Valor, evento.Online, evento.NomeDaEmpresa));
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

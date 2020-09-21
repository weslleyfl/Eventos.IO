using System;
using System.Threading.Tasks;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Commands;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using MediatR;

namespace Eventos.IO.Infra.CrossCutting.Bus
{
    public sealed class InMemoryBus : IBus
    {
     
        private readonly IMediator _mediator;
        private readonly IEventStore _eventStore;

        public InMemoryBus(IMediator mediator, IEventStore eventStore)
        {
            _mediator = mediator;
            _eventStore = eventStore;
        }

        /// <summary>
        /// PublicarEvento
        /// </summary> 
        public async Task RaiseEvent<T>(T theEvent) where T : Event
        {
          
            // https://imasters.com.br/banco-de-dados/event-sourcing-arquitetura-que-pode-salvar-seu-projeto
            // Padrao Event Sourcing
            if (!theEvent.MessageType.Equals("DomainNotification"))
                _eventStore?.SalvarEvento(theEvent);

            await _mediator.Publish(theEvent);
        }

        /// <summary>
        /// EnviarComando
        /// </summary> 
        public async Task SendCommand<T>(T theCommand) where T : Command
        {            
            await _mediator.Send(theCommand);
        }
       
    }
}
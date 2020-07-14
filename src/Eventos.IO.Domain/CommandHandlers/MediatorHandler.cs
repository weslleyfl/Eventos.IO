//using Eventos.IO.Domain.Core.Bus;
//using Eventos.IO.Domain.Core.Commands;
//using Eventos.IO.Domain.Core.Events;
//using Eventos.IO.Domain.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Eventos.IO.Domain.CommandHandlers
//{
//    public class MediatorHandler : IMediatorHandler
//    {
//        private readonly IMediator _mediator;
//        private readonly IEventStore _eventStore;

//        public MediatorHandler(IMediator mediator, IEventStore eventStore)
//        {
//            _mediator = mediator;
//            _eventStore = eventStore;
//        }

//        public async Task EnviarComando<T>(T comando) where T : Command
//        {
//            await _mediator.Send(comando);
//        }

//        public async Task PublicarEvento<T>(T evento) where T : Event
//        {
//            if (!evento.MessageType.Equals("DomainNotification"))
//                _eventStore?.SalvarEvento(evento);

//            await _mediator.Publish(evento);
//        }

//    }
//}

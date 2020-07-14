using System;

namespace Eventos.IO.Domain.Core.Events
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event evento, string data, string user)
        {
            Id = Guid.NewGuid();
            AggregateId = evento.AggregateId; // qual entidade ele pertence / agregado / root
            MessageType = evento.MessageType; // qual tipo, e um command ou um event
            Data = data; // informaçao a ser gravada
            User = user;
        }

        // EF Constructor
        protected StoredEvent() { }

        public Guid Id { get; private set; }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}
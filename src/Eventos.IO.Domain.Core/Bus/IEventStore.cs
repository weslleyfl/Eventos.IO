
using Eventos.IO.Domain.Core.Events;

namespace Eventos.IO.Domain.Core.Bus
{
    public interface IEventStore
    {
        void SalvarEvento<T>(T evento) where T : Event;
    }
}
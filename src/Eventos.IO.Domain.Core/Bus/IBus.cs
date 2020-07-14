using Eventos.IO.Domain.Core.Commands;
using Eventos.IO.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eventos.IO.Domain.Core.Bus
{
    public interface IBus
    {
        Task SendCommand<T>(T theCommand) where T : Command;
        Task RaiseEvent<T>(T theEvent) where T : Event;
    }
}

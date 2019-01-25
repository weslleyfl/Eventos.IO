using System;
using System.Collections.Generic;
using FluentValidation.Results;
using System.Text;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;

namespace Eventos.IO.Domain.CommandHandlers
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IBus _bus;
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;

        protected CommandHandler(IUnitOfWork uow, 
                                 IBus bus, 
                                 IDomainNotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _bus = bus;
            _notifications = notifications;
        }

        protected void NotificarValidacoesErro(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                Console.WriteLine(erro.ErrorMessage);
               _bus.RaiseEvent(new DomainNotification(erro.PropertyName, erro.ErrorMessage));
            }

        }

        protected bool Commit()
        {
            // TODO: Validar antes do commit se há alguma validaçao de negocio com erro!

            if (_notifications.HasNotifications()) return false;

            var responseCommand = _uow.Commit();
            if (responseCommand.Success) return true;

            Console.WriteLine("Ocorreu um erro ao salvar os dados no banco.");
            _bus.RaiseEvent(new DomainNotification("Commit", "Ocorreu um erro ao salvar os dados no banco."));

            return false;

        }
    }
}

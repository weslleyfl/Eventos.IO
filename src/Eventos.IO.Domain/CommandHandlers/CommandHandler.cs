using System;
using System.Collections.Generic;
using FluentValidation.Results;
using System.Text;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using MediatR;

namespace Eventos.IO.Domain.CommandHandlers
{
    /// <summary>
    /// É uma classe base, generica para todos os comandosHandlers
    /// </summary>
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IBus _mediator;
        private readonly DomainNotificationHandler _notifications;

        protected CommandHandler(IUnitOfWork uow,
                                 IBus mediator,
                                 INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _mediator = mediator;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected void NotificarValidacoesErro(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                Console.WriteLine(erro.ErrorMessage);
                // RaiseEvent
                _mediator.RaiseEvent(new DomainNotification(erro.PropertyName, erro.ErrorMessage));
            }

        }

        protected bool Commit()
        {
            // TODO: Validar antes do commit se há alguma validaçao de negocio com erro!

            if (_notifications.HasNotifications()) return false;

            var responseCommand = _uow.Commit();
            if (responseCommand.Success) return true;

            Console.WriteLine("Ocorreu um erro ao salvar os dados no banco.");
            _mediator.RaiseEvent (new DomainNotification("Commit", "Ocorreu um erro ao salvar os dados no banco."));

            return false;

        }
    }
}

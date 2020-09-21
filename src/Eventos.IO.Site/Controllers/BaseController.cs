using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Site.Controllers
{
    public class BaseController : Controller
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IUser _user;
        public Guid OrganizadorId { get; set; }

        public BaseController(INotificationHandler<DomainNotification> notifications, IUser user)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _user = user;

            if (_user.IsAuthenticated())
            {
                OrganizadorId = _user.GetUserId();
            }

        }

        protected bool OperacaoValida()
        {
            return (!_notifications.HasNotifications());
        }
    }
}
using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.Services;
using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Events;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Organizadores.Events;
using Eventos.IO.Domain.OrganizadoresRoot.Commands;
using Eventos.IO.Domain.OrganizadoresRoot.Events;
using Eventos.IO.Domain.OrganizadoresRoot.Repository;
using Eventos.IO.Infra.CrossCutting.AspNetFilters;
using Eventos.IO.Infra.CrossCutting.Bus;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Eventos.IO.Infra.CrossCutting.Identity.Services;
using Eventos.IO.Infra.Data.Context;
using Eventos.IO.Infra.Data.EventSourcing;
using Eventos.IO.Infra.Data.Repository;
using Eventos.IO.Infra.Data.Repository.EventSourcing;
using Eventos.IO.Infra.Data.UoW;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eventos.IO.Infra.CrossCutting.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Application
            //services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
            services.AddScoped<IEventoAppService, EventoAppService>();
            services.AddScoped<IOrganizadorAppService, OrganizadorAppService>();

            // Domain - Commands          
            services.AddScoped<IRequestHandler<RegistrarEventoCommand, bool>, EventoCommandHandlers>();           
            services.AddScoped<IRequestHandler<AtualizarEventoCommand, bool>, EventoCommandHandlers>();
            services.AddScoped<IRequestHandler<ExcluirEventoCommand, bool>, EventoCommandHandlers>();
            services.AddScoped<IRequestHandler<AtualizarEnderecoEventoCommand, bool>, EventoCommandHandlers>();
            services.AddScoped<IRequestHandler<IncluirEnderecoEventoCommand, bool>, EventoCommandHandlers>();
            services.AddScoped<IRequestHandler<RegistrarOrganizadorCommand,bool>, OrganizadorCommandHandler>();

            // Domain - Eventos
            // services.AddSingleton<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<EventoRegistradoEvent>, EventoEventHandler>();
            services.AddScoped<INotificationHandler<EventoAtualizadoEvent>, EventoEventHandler>();
            services.AddScoped<INotificationHandler<EventoExcluidoEvent>, EventoEventHandler>();
            services.AddScoped<INotificationHandler<EnderecoEventoAtualizadoEvent>, EventoEventHandler>();
            services.AddScoped<INotificationHandler<EnderecoEventoAdicionadoEvent>, EventoEventHandler>();
            services.AddScoped<INotificationHandler<OrganizadorRegistradoEvent>, OrganizadorEventHandler>();

            // Infra - Data
            services.AddScoped<IEventoRepository, EventoRepository>();
            services.AddScoped<IOrganizadorRepository, OrganizadorRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<EventosContext>();


            // Infra - Bus
            services.AddScoped<IBus, InMemoryBus>();
            
            // Domain Bus (Mediator)
            // services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Infra - Identity
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IUser, AspNetUser>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreSQLRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            // services.AddScoped<EventStoreSQLContext>();
            

            // ASPNET
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Infra - Filters
            services.AddScoped<ILogger<GlobalExceptionHandlingFilter>, Logger<GlobalExceptionHandlingFilter>>();
            services.AddScoped<GlobalExceptionHandlingFilter>();
        }
    }
}

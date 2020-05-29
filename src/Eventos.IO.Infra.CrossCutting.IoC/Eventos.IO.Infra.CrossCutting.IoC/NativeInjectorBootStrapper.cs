using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.Services;
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
using Eventos.IO.Infra.Data.Repository;
using Eventos.IO.Infra.Data.UoW;
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
            services.AddScoped<IHandler<RegistrarEventoCommand>, EventoCommandHandlers>();
            services.AddScoped<IHandler<AtualizarEventoCommand>, EventoCommandHandlers>();
            services.AddScoped<IHandler<ExcluirEventoCommand>, EventoCommandHandlers>();
            services.AddScoped<IHandler<AtualizarEnderecoEventoCommand>, EventoCommandHandlers>();
            services.AddScoped<IHandler<IncluirEnderecoEventoCommand>, EventoCommandHandlers>();
            services.AddScoped<IHandler<RegistrarOrganizadorCommand>, OrganizadorCommandHandler>();

            // Domain - Eventos
            services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<IHandler<EventoRegistradoEvent>, EventoEventHandler>();
            services.AddScoped<IHandler<EventoAtualizadoEvent>, EventoEventHandler>();
            services.AddScoped<IHandler<EventoExcluidoEvent>, EventoEventHandler>();
            services.AddScoped<IHandler<EnderecoEventoAtualizadoEvent>, EventoEventHandler>();
            services.AddScoped<IHandler<EnderecoEventoAdicionadoEvent>, EventoEventHandler>();
            services.AddScoped<IHandler<OrganizadorRegistradoEvent>, OrganizadorEventHandler>();

            // Infra - Data
            services.AddTransient<IEventoRepository, EventoRepository>();
            services.AddScoped<IOrganizadorRepository, OrganizadorRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<EventosContext>();

            // Infra - Bus
            services.AddScoped<IBus, InMemoryBus>();

            // Infra - Identity
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IUser, AspNetUser>();
            
            // ASPNET
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Infra - Filters
            services.AddScoped<ILogger<GlobalExceptionHandlingFilter>, Logger<GlobalExceptionHandlingFilter>>();
            services.AddScoped<GlobalExceptionHandlingFilter>();
        }
    }
}

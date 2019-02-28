﻿using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.EventosRoot.Commands;
using Eventos.IO.Domain.EventosRoot.Events;
using Eventos.IO.Domain.EventosRoot.Repository;
using Eventos.IO.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot
{
    public class EventoCommandHandlers : CommandHandler, IHandler<RegistrarEventoCommand>, IHandler<AtualizarEventoCommand>, IHandler<ExcluirEventoCommand>
    {
        // Injeçao de dependencia
        private readonly IEventoRepository _eventoRepository;
        private readonly IBus _bus;
        //private readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public EventoCommandHandlers(IEventoRepository eventoRespository,
                                     IUnitOfWork uow,
                                     IBus bus,
                                     IDomainNotificationHandler<DomainNotification> notifications) : base(uow, bus, notifications)
        {
            _eventoRepository = eventoRespository;
            _bus = bus;
        }

        public void Handle(RegistrarEventoCommand message)
        {
            var endereco = new Endereco(message.Endereco.Id, message.Endereco.Logradouro, message.Endereco.Numero, message.Endereco.Complemento, message.Endereco.Bairro, message.Endereco.CEP, message.Endereco.Cidade, message.Endereco.Estado, message.Endereco.EventoId.Value);

            var evento = Evento.EventoFactory.NovoEventoCompleto(message.Id, message.Nome, message.DescricaoCurta,
                message.DescricaoLonga, message.DataInicio, message.DataFim, message.Gratuito, message.Valor,
                message.Online, message.NomeEmpresa, message.OrganizadorId, endereco, message.CategoriaId);


            if (!EventoValido(evento)) return;

            // TODO: Validaçao do negocio no command
            // Pode tratar regras de negocio aqui tambem, se nao for o caso de tratar lá na entidade - Evento
            // Validaçoes de Negocio Exemplo
            // O Organizador pode registrar um evento? (tipo sera que ele pagou a taxa de abertura)

            // Persistencia
            _eventoRepository.Adicionar(evento);

            if (Commit())
            {
                //Notificar um processo concluido
                Console.WriteLine("Evento registrado com sucesso");
                _bus.RaiseEvent(new EventoRegistradoEvent(evento.Id, evento.Nome, evento.DataInicio, evento.DataFim, evento.Gratuito,
                                                            evento.Valor, evento.Online, evento.NomeDaEmpresa));
            }
        }

        public void Handle(AtualizarEventoCommand message)
        {
            var eventoAtual = _eventoRepository.ObterPorId(message.Id);


            if (!EventoExistente(message.Id, message.MessageType)) return;

            var evento = Evento.EventoFactory.NovoEventoCompleto(message.Id, message.Nome, message.DescricaoCurta, message.DescricaoLonga,
                                                                 message.DataInicio, message.DataFim, message.Gratuito, message.Valor,
                                                                 message.Online, message.NomeEmpresa, message.OrganizadorId, eventoAtual.Endereco, message.CategoriaId);

            if(!EventoValido(evento)) return;

            _eventoRepository.Atualizar(evento);

            if (Commit())
            {
                _bus.RaiseEvent(new EventoAtualizadoEvent(evento.Id, evento.Nome,evento.DescricaoCurta, evento.DescricaoLonga, evento.DataInicio, evento.DataFim, evento.Gratuito,
                                                            evento.Valor, evento.Online, evento.NomeDaEmpresa));
            }

        }        

        public void Handle(ExcluirEventoCommand message)
        {
            if (!EventoExistente(message.Id, message.MessageType)) return;

            _eventoRepository.Remover(message.Id);

            if (Commit())
            {
                _bus.RaiseEvent(new EventoExcluidoEvent(message.Id));
            }

        }

        private bool EventoValido(Evento evento)
        {
            if (evento.EhValido()) return true;

            NotificarValidacoesErro(evento.ValidationResult);
            return false;

        }

        private bool EventoExistente(Guid id, string messageType)
        {
            var evento = _eventoRepository.ObterPorId(id);

            if (evento != null) return true;

            _bus.RaiseEvent(new DomainNotification(messageType, "Evento não encontrado"));
            return false;
        }

    }
}

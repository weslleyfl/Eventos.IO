﻿using Eventos.IO.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot.Events
{
    public class EventoRegistradoEvent : BaseEventoEvent
    {
        public EventoRegistradoEvent(
           Guid id,
           string nome,
           DateTime dateInicio,
           DateTime dataFim,
           bool gratuito,
           decimal valor,
           bool online,
           string nomeEmpresa)
        {
            Id = id;
            Nome = nome;
            DataInicio = dateInicio;
            DataFim = dataFim;
            Gratuito = gratuito;
            Valor = valor;
            Online = online;
            NomeDaEmpresa = nomeEmpresa;

            AggregateId = id;
        }
    }
}

﻿using Eventos.IO.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot.Commands
{
    public class AtualizarEventoCommand : BaseEventoCommand
    {
        public AtualizarEventoCommand(
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
        }
    }
}
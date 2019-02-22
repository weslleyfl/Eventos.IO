using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot.Commands
{
    public class RegistrarEventoCommand : BaseEventoCommand
    {
        public RegistrarEventoCommand(
            string nome,
            DateTime dateInicio,
            DateTime dataFim,
            bool gratuito,
            decimal valor,
            bool online,
            string nomeEmpresa,
            Guid organizadorId,
            Guid categoriaId,
            IncluirEnderecoEventoCommand endereco

            )
        {

            Nome = nome;
            DataInicio = dateInicio;
            DataFim = dataFim;
            Gratuito = gratuito;
            Valor = valor;
            Online = online;
            NomeEmpresa = nomeEmpresa;
            OrganizadorId = organizadorId;
            CategoriaId = categoriaId;
            Endereco = endereco;
        }

        public IncluirEnderecoEventoCommand Endereco { get; private set; }



    }
}

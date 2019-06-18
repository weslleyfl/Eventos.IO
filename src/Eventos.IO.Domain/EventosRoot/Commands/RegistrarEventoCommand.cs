using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot.Commands
{
    /// <summary>
    /// Comand é como se fosse um DTO - Carregar informaçoes para serem gravadas no banco
    /// O que voce precisa no minimo para registrar um evento
    /// </summary>
    public class RegistrarEventoCommand : BaseEventoCommand
    {
        public RegistrarEventoCommand(
            string nome,
            string descricaoCurta,
            string descricaoLonga,
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
            DescricaoCurta = descricaoCurta;
            DescricaoLonga = descricaoLonga;
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

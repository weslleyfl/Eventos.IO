using Eventos.IO.Domain.Core.Models;
using Eventos.IO.Domain.OrganizadoresRoot;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eventos.IO.Domain.EventosRoot
{
    public class Evento : Entity<Evento>
    {
        //TODO: TODA ENTIDADE TEM OBRIGAÇAO DE SE AUTO VALIDAR. Validaçao dentro do modelo - public abstract bool EhValido();
        // uma class deve ter somente um construtor publico. boas praticas . se precisar mais , criar um factory
        public Evento(
            string nome,
            DateTime dateInicio,
            DateTime dataFim,
            bool gratuito,
            decimal valor,
            bool online,
            string nomeEmpresa)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            DataInicio = dateInicio;
            DataFim = dataFim;
            Gratuito = gratuito;
            Valor = valor;
            Online = online;
            NomeEmpresa = nomeEmpresa;

            //ErrosValidacao = new Dictionary<string, string>();

            //if (nome.Length < 3)
            //    //throw new ArgumentException("O nome do Evento deve ter mais de 3 caracteres");
            //    ErrosValidacao.Add("Nome", "O nome do Evento deve ter mais de 3 caracteres");

            //if ((gratuito) && valor != 0)
            //    ErrosValidacao.Add("Valor", "Se é gratuito valor nao pode ser preenchido");

        }

        private Evento() { }

        public string Nome { get; private set; }
        public string DescricaoCurta { get; private set; }
        public string DescricaoLonga { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public bool Gratuito { get; private set; }
        public decimal Valor { get; private set; }
        public bool Online { get; private set; }
        public string NomeEmpresa { get; private set; }
        public bool Excluido { get; private set; }
        public ICollection<Tags> Tags { get; private set; }

        // Chave estrangeira (foreign key) EF
        public Guid? CategoriaId { get; private set; } // ? campo nao obrigatorio EF
        public Guid? EnderecoId { get; private set; }
        public Guid OrganizadorId { get; private set; }

        //public Dictionary<string, string> ErrosValidacao { get; set; }

        // EF propriedades de navegacao
        public virtual Categoria Categoria { get; private set; }
        public virtual Endereco Endereco { get; private set; }
        public virtual Organizador Organizador { get; private set; }

        public void AtribuirEndereco(Endereco endereco)
        {
            if (!endereco.EhValido()) return;
            Endereco = endereco;
        }

        public void AtribuirCategoria(Categoria categoria)
        {
            if (!categoria.EhValido()) return;
            Categoria = categoria;
        }

        public void ExcluirEvento()
        {
            // TODO: Deve validar alguma regra?
            // Se alguem ja se inscreveu no evento, nao posso excluir
            Excluido = true;
        }

        public void TornarPresencial()
        {
            // TODO: Alguma validacao de negocio?
            Online = false;
        }

        public override bool EhValido()
        {
            Validar();
            return ValidationResult.IsValid;
        }



        #region Validaçoes

        private void Validar()
        {
            // validaçoes do evento
            ValidarNome();
            ValidarValor();
            ValidarData();
            ValidarLocal();
            ValidarNomeEmpresa();
            ValidationResult = Validate(this); // aqui valida só evento

            // Validacoes adicionais 
            ValidarEndereco();
        }

        private void ValidarNome()
        {
            RuleFor(reg => reg.Nome)
                .NotEmpty().WithMessage("O nome do evento precisa ser fornecido")
                .Length(2, 150).WithMessage("o nome do evento precisa ter entre 2 e 150 caracteres");
        }

        private void ValidarValor()
        {
            if (!Gratuito)
            {
                RuleFor(c => c.Valor)
                    .ExclusiveBetween(1, 50000) // e a regra é feita para dar certo, se nao ela envia a mensagem abaixo
                    .WithMessage("O valor deve estar entre 1.00 e 50.000");
            }

            if (Gratuito)
            {
                RuleFor(c => c.Valor)                    
                    .InclusiveBetween(0, 0).When(e => e.Gratuito)
                    .WithMessage("O valor deve ser 0 para um evento gratuito");
            }
        }

        private void ValidarData()
        {
            RuleFor(c => c.DataInicio)
                .LessThan(c => c.DataFim)
                .WithMessage("A data de inicio deve ser menor que a data fim do evento");

            RuleFor(c => c.DataInicio)
                .GreaterThan(DateTime.Now)
                .WithMessage("A data de inicio nao deve ser menor que a data atual");
        }

        private void ValidarLocal()
        {
            if (Online)
                RuleFor(c => c.Endereco)
                    .Null().When(c => c.Online)
                    .WithMessage("O evento nao deve possuir um endereço se for online");

            if (!Online)
                RuleFor(c => c.Endereco)
                    .NotNull().When(c => c.Online == false)
                    .WithMessage("O evento deve possuir um endereço");
        }

        private void ValidarNomeEmpresa()
        {
            RuleFor(c => c.NomeEmpresa)
                .NotEmpty().WithMessage("O nome do organizador deve ser preenchido")
                .Length(2, 150).WithMessage("o nome do organizador precisa ter entre 2 e 200 caracteres");

        }


        private void ValidarEndereco()
        {
            // Regra de negocio - regra de validaçao - se o evento for online nao tem endereço fisico.
            if (Online) return;
            if (Endereco.EhValido()) return;

            foreach (var error in Endereco.ValidationResult.Errors)
            {
                ValidationResult.Errors.Add(error);
            }
        }

        #endregion

        public static class EventoFactory
        {
            // Isso nao é um construtor ,e um metodo. voce pode colocar outras validaçoes aqui
            public static Evento NovoEventoCompleto(Guid id, string nome, string descricaoCurta, string descricaoLonga, DateTime dateInicio, DateTime dataFim, bool gratuito, decimal valor, bool online, string nomeEmpresa, Guid? organizadorId, Endereco endereco, Guid categoriaId)
            {
                var evento = new Evento()
                {
                    Id = id,
                    Nome = nome,
                    DescricaoCurta = descricaoCurta,
                    DescricaoLonga = descricaoLonga,
                    DataInicio = dateInicio,
                    DataFim = dataFim,
                    Gratuito = gratuito,
                    Valor = valor,
                    Online = online,
                    NomeEmpresa = nomeEmpresa,
                    Endereco = endereco,
                    CategoriaId = categoriaId
                };

                if (organizadorId.HasValue)
                    evento.OrganizadorId = organizadorId.Value;

                if (online)
                    evento.Endereco = null;

                return evento;
            }

        }

    }

}


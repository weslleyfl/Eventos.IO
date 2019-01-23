﻿using Eventos.IO.Domain.Core.Models;
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
            NomeDaEmpresa = nomeEmpresa;

            //ErrosValidacao = new Dictionary<string, string>();

            //if (nome.Length < 3)
            //    //throw new ArgumentException("O nome do Evento deve ter mais de 3 caracteres");
            //    ErrosValidacao.Add("Nome", "O nome do Evento deve ter mais de 3 caracteres");

            //if ((gratuito) && valor != 0)
            //    ErrosValidacao.Add("Valor", "Se é gratuito valor nao pode ser preenchido");

        }


        public string Nome { get; private set; }
        public string DescricaoCurta { get; private set; }
        public string DescricaoLonga { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public bool Gratuito { get; private set; }
        public decimal Valor { get; private set; }
        public bool Online { get; private set; }
        public string NomeDaEmpresa { get; private set; }
        public Categoria Categoria { get; private set; }
        public ICollection<Tags> Tags { get; private set; }
        public Endereco Endereco { get; private set; }
        public Organizador Organizador { get; private set; }

        //public Dictionary<string, string> ErrosValidacao { get; set; }

        public override bool EhValido()
        {
            Validar();
            return ValidationResult.IsValid;
        }

        #region Validaçoes

        private void Validar()
        {
            ValidarNome();
            ValidarValor();
            ValidarData();
            ValidarLocal();
            ValidarNomeEmpresa();

            ValidationResult = Validate(this);
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
                    .ExclusiveBetween(1, 50000)
                    .WithMessage("O valor deve estar entre 1.00 e 50.000");
            }

            if (Gratuito)
            {
                RuleFor(c => c.Valor)
                    .ExclusiveBetween(0, 0).When(e => e.Gratuito)
                    .WithMessage("O valor deve ser 0 para um evento gratuito");
            }
        }

        private void ValidarData()
        {
            RuleFor(c => c.DataInicio)
                .GreaterThan(c => c.DataFim)
                .WithMessage("A data de inicio deve ser maior que a data fim do evento");

            RuleFor(c => c.DataInicio)
                .LessThan(DateTime.Now)
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
            RuleFor(c => c.NomeDaEmpresa)
                .NotEmpty().WithMessage("O nome do organizador deve ser preenchido")
                .Length(2, 150).WithMessage("o nome do organizador precisa ter entre 2 e 200 caracteres");

        }

        #endregion

    }
}
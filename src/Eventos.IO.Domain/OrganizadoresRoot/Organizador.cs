﻿using Eventos.IO.Domain.Core.Models;
using Eventos.IO.Domain.EventosRoot;
using System;
using System.Collections.Generic;

namespace Eventos.IO.Domain.OrganizadoresRoot
{
    public class Organizador : Entity<Organizador>
    {
        public string Nome { get; private set; }
        public string CPF { get; private set; }
        public string Email { get; private set; }

        public Organizador(Guid id, string nome, string cpf, string email)
        {
            Id = id;
            Nome = nome;
            CPF = cpf;
            Email = email;
        }

        // EF Construtor
        protected Organizador() { }

        // EF Propriedade de Navegação
        public virtual ICollection<Evento> Eventos { get; set; }

        public override bool EhValido()
        {
            return true;
        }
    }
}
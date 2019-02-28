using Eventos.IO.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace Eventos.IO.Domain.EventosRoot
{
    public class Categoria : Entity<Categoria>
    {
        public Categoria(Guid id)
        {
            Id = id;
        }

        public string Nome { get; private set; }

        // EF Propriedade de Navegação -- Uma categroria tem varios eventos - 1 para varios
        public virtual ICollection<Evento> Eventos { get; set; }

        // Construtor para o EF
        protected Categoria() { }

        public override bool EhValido()
        {
            return true;
        }
    }
}
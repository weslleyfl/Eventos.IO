using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.OrganizadoresRoot;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Infra.Data.Context
{
    public class EventosContext : DbContext
    {
        public DbSet<Evento> Eventos { get; set; }        
        public DbSet<Organizador> Organizadores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        // Utilizando Fluent API - Metodo que vai criar um modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>()
                .Property(e => e.Nome)
                .HasColumnType("varchar(150)")
                .IsRequired();


            base.OnModelCreating(modelBuilder);
        }


    }
}

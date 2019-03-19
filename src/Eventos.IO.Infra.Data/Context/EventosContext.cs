using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.OrganizadoresRoot;
using Eventos.IO.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
            modelBuilder.ApplyConfiguration(new EventoMapping());
            modelBuilder.ApplyConfiguration(new OrganizadorMapping());
            modelBuilder.ApplyConfiguration(new EnderecoMapping());
            modelBuilder.ApplyConfiguration(new CategoriaMapping());          

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = @"C:\Projetos\GitHubProjetos\ASPNetCoreLab\Eventos.IO\src\Eventos.IO.Site\";
            var config = new ConfigurationBuilder()
               .SetBasePath(path) // (Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

           
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        }
    
    }
}

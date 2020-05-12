using Eventos.IO.Domain.EventosRoot;
using Eventos.IO.Domain.OrganizadoresRoot;
using Eventos.IO.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace Eventos.IO.Infra.Data.Context
{
    public class EventosContext : DbContext
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public EventosContext(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public EventosContext(DbContextOptions<EventosContext> options)
        : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        // Utilizando Fluent API - Metodo que vai criar um modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new EventoMapping());
            //modelBuilder.ApplyConfiguration(new OrganizadorMapping());
            //modelBuilder.ApplyConfiguration(new EnderecoMapping());
            //modelBuilder.ApplyConfiguration(new CategoriaMapping());
            /*
             A new extension method, ApplyConfigurationsFromAssembly, was introduced in 2.2, which scans 
             a given assembly for all types that implement IEntityTypeConfiguration, and registers each 
             one automatically.
             */
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            string projectRootPath = _hostingEnvironment.ContentRootPath;

            var config = new ConfigurationBuilder()
               .SetBasePath(projectRootPath)
               //.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();
            
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        }

    }
}

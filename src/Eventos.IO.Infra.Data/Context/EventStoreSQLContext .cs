using System.IO;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Infra.Data.Extensions;
using Eventos.IO.Infra.Data.Mappings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Eventos.IO.Infra.Data.Context
{
    public class EventStoreSQLContext : DbContext
    {
        public DbSet<StoredEvent> StoredEvent { get; set; }

        private readonly IHostingEnvironment _hostingEnvironment;

        public EventStoreSQLContext(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddConfiguration(new StoredEventMap());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string projectRootPath = _hostingEnvironment.ContentRootPath;

            var config = new ConfigurationBuilder()
               .SetBasePath(projectRootPath)               
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
         
        }
    }
}
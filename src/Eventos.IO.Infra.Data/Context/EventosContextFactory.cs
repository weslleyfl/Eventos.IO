using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Infra.Data.Context
{
    public class EventosContextFactory : IDesignTimeDbContextFactory<EventosContext>
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        public EventosContextFactory(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        public EventosContext CreateDbContext(string[] args)
        {
            string projectRootPath = _hostingEnvironment.ContentRootPath;

            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(projectRootPath)
               .AddJsonFile("appsettings.json")
               .Build();

            var builder = new DbContextOptionsBuilder<EventosContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new EventosContext(builder.Options);

            //var optionsBuilder = new DbContextOptionsBuilder<EventosContext>();
            //optionsBuilder.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection"));

            //return new EventosContext(optionsBuilder.Options);
        }
    }
}

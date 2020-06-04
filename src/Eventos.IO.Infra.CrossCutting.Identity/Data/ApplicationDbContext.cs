using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;


namespace Eventos.IO.Infra.CrossCutting.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        //public ApplicationDbContext(IHostingEnvironment hostingEnvironment)
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    IHostingEnvironment hostingEnvironment) : base(options)
        {
            _hostingEnvironment = hostingEnvironment;
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

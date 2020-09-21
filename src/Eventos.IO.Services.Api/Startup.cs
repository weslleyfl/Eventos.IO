using AutoMapper;
using Eventos.IO.Application.AutoMapper;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Infra.CrossCutting.Identity.Authorization;
using Eventos.IO.Infra.CrossCutting.Identity.Data;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Eventos.IO.Infra.CrossCutting.IoC;
using Eventos.IO.Infra.Data.Context;
using Eventos.IO.Services.Api.AutoMapper;
using Eventos.IO.Services.Api.Configurations;
using Eventos.IO.Services.Api.Middlewares;
using Eventos.IO.Services.Api.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;         
        }
        

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Configurando o uso da classe de contexto para
            // acesso às tabelas do ASP.NET Identity Core
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                   sqlServerOptionsAction: (sqlOptions) =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                   }
               ));

            // Contexto do EF para uso do padrao Event Sourcing
            services.AddDbContext<EventStoreSQLContext>();
                     
           
            // Options para configurações customizadas
            services.AddOptions();

            // Configuraçoes customizadas
            LoadCustomMiddlers(services, Configuration);
            
            // cache em memoria
            services.AddMemoryCache();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperConfiguration));
            
            // MediatR
            services.AddMediatR(typeof(Startup));

            // Registrar todos os DI
            services.AddDIConfiguration();
            
        }

        private static void LoadCustomMiddlers(IServiceCollection services, IConfiguration configuration)
        {
            // Configura o modo de compressão Brotli
            services.AddResponseCompressionConfig();

            // Configurações do MVC Padrão e suas Options - AddMvc
            services.AddMvcConfig();

            // Configurações de Autenticação, Autorização e JWT.
            services.AddMvcSecurity(configuration);

            // Configurações para controle de Versao para a WebApi
            services.AddApiVersioningConfig();

            // Configurações do Swagger
            services.AddSwaggerConfig();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor accessor, ILoggerFactory loggerFactory)
        {
            // Middleware nativo para tratamento de exceptions,
            app.UseGlobalExceptionHandler(loggerFactory);

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();


            // app.UseSwaggerAuthorized();
            // app.UseSwaggerAuthorizedInterceptor();
            // https://discoverdot.net/projects/swashbuckle-aspnetcore --> exemplo
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Eventos.IO API v1.0");
                s.SwaggerEndpoint("/swagger/v2/swagger.json", "Eventos.IO API V2 Docs");

            });


            // Ativa a compressão
            app.UseResponseCompression();

            //app.UseMvc();   
            app.UseMvc()
              .UseApiVersioning();


            // criando uma copia do container
            //InMemoryBus.ContainerAccessor = () => accessor.HttpContext.RequestServices;

        }
              

       

    }
}

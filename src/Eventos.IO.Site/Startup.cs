using ElmahCore.Mvc;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.Services;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Infra.CrossCutting.AspNetFilters;
using Eventos.IO.Infra.CrossCutting.Bus;
using Eventos.IO.Infra.CrossCutting.Identity.Data;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Eventos.IO.Infra.CrossCutting.IoC;
using Eventos.IO.Site.Extensions;
using Eventos.IO.Site.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using AutoMapper;
//using Eventos.IO.Application.AutoMapper;

namespace Eventos.IO.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //var rootApp = Configuration[Microsoft.Extensions.Hosting.HostDefaults.ContentRootKey];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PodeLerEventos", policy => policy.RequireClaim("Eventos", "Ler"));
                options.AddPolicy("PodeGravar", policy => policy.RequireClaim("Eventos", "Gravar"));

            });


            services.Configure<IdentityOptions>(options =>
                {
                    // Default Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequiredUniqueChars = 1;

                    // Default SignIn settings.
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;

                    // Default Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = true;

                });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(600);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });


            // proprio para .net core - adicionei no pipeline do .core
            //Application.AutoMapper.AutoMapperConfiguration.RegisterMappings();
            //services.AddAutoMapper();           

            //services.AddAutoMapper(typeof(Startup));
            //services.AddAutoMapper();
            services.AddAutoMapperSetup();

            // TODO: Remover pos testes, mover para a camada de IoC
            //services.AddScoped<IUser, AspNetUser>();

            // Elmah Core capturar as exceções
            // services.AddElmah();
            services.AddElmah(options =>
            {
                options.Path = @"erros";
                options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated;
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(GlobalExceptionHandlingFilter)); // By type
                options.Filters.Add(new ServiceFilterAttribute(typeof(GlobalExceptionHandlingFilter))); // devido construtor
                // TODO: Implementar o serilog aqui
                // options.Filters.Add(typeof(GlobalActionLogger));
                //options.Filters.Add(new ServiceFilterAttribute(typeof(GlobalActionLogger)));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // Injeçao de dependencia 
            RegisterServices(services);
                       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor accessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler("/erro-de-aplicacao");
                app.UseStatusCodePagesWithReExecute("/erro-de-aplicacao/{0}");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            // Elmah Core capturar as exceções
            // Default elmah path ~/elmah. services.AddElmah(options => options.Path = "you_path_here")
            app.UseElmah();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            InMemoryBus.ContainerAccessor = () => accessor.HttpContext.RequestServices;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}

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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
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


            services.AddDbContext<EventStoreSQLContext>();

            //services.AddDefaultIdentity<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            // Ativando a utilização do ASP.NET Identity, a fim de
            // permitir a recuperação de seus objetos via injeção de
            // dependências
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


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
                options.ExpireTimeSpan = TimeSpan.FromMinutes(300);
                options.SlidingExpiration = true;

                //options.AccessDeniedPath =
                options.Cookie.Name = "Cookie_Do_Identity";
            });


            #region " Tokens JWT "

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(appSettingsSection);

            // SecretKey é criado aqui
            var secretKey = appSettingsSection[nameof(AppSettings.Secret)];
            // Credentials
            var _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // compressão de dados gzip
            //services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            //services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            // Configura o modo de compressão Brotli
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.AddOptions();
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
              .AddJsonOptions((options) =>
              {
                  // Remove null fields from API JSON response
                  options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
              });

            // cache em memoria
            services.AddMemoryCache();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("PodeLerEventos", policy => policy.RequireClaim("Eventos", "Ler"));
                options.AddPolicy("PodeGravar", policy => policy.RequireClaim("Eventos", "Gravar"));
            });

            // appsettings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtTokenOptions));

            // populando os options jwt para ser usado no accountController
            services.Configure<JwtTokenOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtTokenOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtTokenOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // configure
            // https://docs.microsoft.com/pt-br/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-3.1
            // if you face same problem JWT did not support cookie with token together so you must remove
            // when you need to authorize any action by token you can use this attribute
            // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            // to any actions. when you need to authorize any action by cookie you need to add - [Authorize]            
            services.AddAuthentication(authOptions =>
            {
                // JWT Bearer authentication scheme will use the token that is provided as part of the Authorization 
                // authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // should prompt the user to authenticate themselves.  This could for example mean that the user gets redirected to a login form
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                // bearerOptions.RequireHttpsMetadata = false; // disable so em desenvolviemtno
                // bearerOptions.SaveToken = true;

                var paramsValidation = bearerOptions.TokenValidationParameters;

                paramsValidation.ValidateIssuer = true;
                paramsValidation.ValidIssuer = jwtAppSettingOptions[nameof(JwtTokenOptions.Issuer)];

                paramsValidation.ValidateAudience = true;
                paramsValidation.ValidAudience = jwtAppSettingOptions[nameof(JwtTokenOptions.Audience)];

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.IssuerSigningKey = _signingKey;

                // Obrigar um tempo de expiraçao do token
                paramsValidation.RequireExpirationTime = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;


            });

            #endregion

            // Essa chamada era obrigatória para registrar os serviços necessários para usar o Options Pattern em 
            // versões anteriores do .Net Core. A partir das versões 2.x não é mais necessário fazer essa chamada 
            // explicitamente, pois o método services.Configure irá executá-lo internamente.
            // services.AddOptions();

            services.AddAutoMapper(typeof(AutoMapperConfiguration));

            // Add API Versioning to the service container to your project
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number  true to use
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
                // multiple “ IApiVersionReader” implementations are combined to work with all the styles.
                //config.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(),
                //                                                   new HeaderApiVersionReader("api-version"),
                //                                                   new MediaTypeApiVersionReader("v"));

                config.ApiVersionReader = ApiVersionReader.Combine(new MediaTypeApiVersionReader("v"));

            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


            services.AddSwaggerConfig();

            services.AddMediatR(typeof(Startup));

            // Registrar todos os DI
            RegisterServices(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor accessor)
        {

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
            //app.UseSwaggerAuthorizedInterceptor();
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

        private static void RegisterServices(IServiceCollection services)
        {
            NativeInjectorBootStrapper.RegisterServices(services);
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Version = description.ApiVersion.ToString(),
                Title = $"Eventos.IO API {description.ApiVersion}",
                Description = "API do site Eventos.IO",
                TermsOfService = new Uri("http://eventos.io/terms"),
                Contact = new OpenApiContact { Name = "Desenvolvedor X", Email = "email@eventos.io", Url = new Uri("http://eventos.io") },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("http://eventos.io/licensa") }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

    }
}

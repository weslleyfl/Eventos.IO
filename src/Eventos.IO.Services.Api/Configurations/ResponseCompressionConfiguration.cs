using Eventos.IO.Services.Api.CompressionProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Configurations
{
    public static class ResponseCompressionConfiguration
    {
        public static void AddResponseCompressionConfig(this IServiceCollection services)
        {
            // compressão de dados gzip
            //services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            //services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            // Configura o modo de compressão Brotli
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = true;
               
            });
           
        }
    }
}

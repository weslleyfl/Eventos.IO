using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eventos.IO.Services.Api.Helpers
{
    /// <summary>
    /// And then I use this where I need to get the data from the appsettings.json file:
    /// var appSettingsJson = AppSettingsJson.GetAppSettings();
    /// // appSettingsJson["keyName"]
    /// </summary>
    public static class AppSettingsJson
    {
        public static IConfigurationRoot GetAppSettings()
        {
            string applicationExeDirectory = ApplicationExeDirectory();

            var builder = new ConfigurationBuilder()
            .SetBasePath(applicationExeDirectory)
            .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static string ApplicationExeDirectory()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appRoot = Path.GetDirectoryName(location);

            return appRoot;
        }
    }
}

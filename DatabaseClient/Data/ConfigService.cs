using System;
using Microsoft.Extensions.Configuration;

namespace DatabaseClient.Data
{
    public static class ConfigService
    {
        private static IConfigurationRoot? _config;

        public static void Initialize()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string GetConnection(string name)
        {
            if (_config == null)
            {
                Initialize();
            }

            return _config!.GetConnectionString(name)
                ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
        }
    }
}

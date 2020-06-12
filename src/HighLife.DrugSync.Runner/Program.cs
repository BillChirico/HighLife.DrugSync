using System;
using Discord.WebSocket;
using HighLife.DrugSync.Domain;
using HighLife.DrugSync.Service.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HighLife.DrugSync.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Settings
                    services.Configure<AppSettings>(GetSettingsFile("appsettings.json", "Settings"));

                    // Discord
                    services.AddSingleton<IDiscordBot, DiscordBot>();
                    services.AddSingleton<DiscordSocketClient>();
                });
        }

        private static IConfigurationSection GetSettingsFile(string file, string section)
        {
            var builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(
                    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Production"
                        ? "appsettings.json"
                        : "appsettings.Development.json", false,
                    true);

            var configuration = builder.Build();

            return configuration.GetSection(section);
        }
    }
}
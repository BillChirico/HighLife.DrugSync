using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using HighLife.DrugSync.Domain;
using HighLife.DrugSync.Service.Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HighLife.DrugSync.Runner
{
    public class Worker : BackgroundService
    {
        private readonly IDiscordBot _discordBot;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Worker> _logger;
        private readonly AppSettings _settings;

        public Worker(ILogger<Worker> logger, IDiscordBot discordBot,
            DiscordSocketClient discordSocketClient, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _discordBot = discordBot;
            _discordSocketClient = discordSocketClient;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Reset event
            var mre = new ManualResetEvent(false);

            await _discordBot.Connect(_settings.DiscordBotToken);

            _discordSocketClient.Ready += async () => { _logger.LogInformation("Discord client is ready"); };

            _discordSocketClient.GuildAvailable += guild =>
            {
                _logger.LogInformation("Discord guild is ready");

                mre.Set();

                return Task.CompletedTask;
            };

            // Wait for all connectable services to be ready
            mre.WaitOne();

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
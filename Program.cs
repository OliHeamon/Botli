using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Botli
{
    public class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;

        private Events events;

        private CommandHandler commandHandler;

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();

            await Initialise();

            await client.LoginAsync(TokenType.Bot, Constants.DiscordAPIKey);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Initialise()
        {
            CommandService commandService = new CommandService();

            events = new Events(client, commandService);
            events.InitialiseEvents();

            commandHandler = new CommandHandler(client, commandService, BuildServiceProvider());
            await commandHandler.InstallCommandsAsync();
        }

        private IServiceProvider BuildServiceProvider()
            => new ServiceCollection()
            .AddSingleton(client)
            .BuildServiceProvider();
    }
}

using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Botli
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;

        private readonly CommandService commands;

        private readonly IServiceProvider services;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            this.commands = commands;
            this.client = client;
            this.services = services;
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message))
            {
                return;
            }

            int argPos = 0;

            if (!(message.HasCharPrefix(Constants.Prefix, ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(context, argPos, services);
        }
    }
}

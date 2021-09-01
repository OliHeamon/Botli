using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Botli.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService commandService;

        public HelpModule(CommandService commandService)
        {
            this.commandService = commandService;
        }

        [Command("help")]
        [Summary("Lists all available commands.")]
        public Task GiveHelp()
        {
            EmbedBuilder builder = new EmbedBuilder();

            foreach (CommandInfo command in commandService.Commands)
            {
                string text = command.Summary ?? "Summary not found.\n";

                string parameters = "";

                foreach (ParameterInfo parameter in command.Parameters)
                {
                    parameters += $" <{parameter.Name}>";
                }

                string commandName;

                if (command.Aliases[0] == command.Module.Group)
                {
                    commandName = command.Module.Group;
                }
                else
                {
                    string groupName = $"{command.Module.Group} " ?? "";

                    commandName = $"{groupName}{command.Name}";
                }

                builder.AddField($"{commandName}{parameters}", text, true);
            }

            builder
                .WithTitle($"Available commands (with prefix '{Constants.Prefix}'):")
                .WithColor(Constants.EmbedBlue);

            return ReplyAsync(embed: builder.Build());
        }
    }
}

using Discord.Commands;
using System.Threading.Tasks;

namespace Botli.Modules
{
    public class RepeatModule : ModuleBase<SocketCommandContext>
    {
        [Command("repeat")]
        [Summary("Repeats the input text.")]
        public Task TranslateText(params string[] text)
        {
            string message = "";

            foreach (string word in text)
            {
                message += $" {word}";
            }

            Context.Message.DeleteAsync();

            return ReplyAsync(message.TrimStart());
        }
    }
}

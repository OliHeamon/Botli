using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace Botli.Modules
{
    public class RandomWordModule : ModuleBase<SocketCommandContext>
    {
        private readonly string[] words;

        public RandomWordModule()
        {
            string file = File.ReadAllText("words.json");

            string[] entries = file.Split(',');

            for (int i = 0; i < entries.Length; i++)
            {
                entries[i] = entries[i].TrimStart('[', '\\', '\"').TrimEnd('\\', '\"', ']', '\n');
            }

            words = entries;
        }

        [Command("randomword")]
        [Summary("Outputs a random word from a database of roughly 275,000.")]
        public Task GetRandomWord()
            => ReplyAsync($"{words.GetRandomElement(Utils.Random.Value).CapitaliseFirstLetter()}.");
    }
}

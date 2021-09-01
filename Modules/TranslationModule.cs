using Discord.Commands;
using Google.Cloud.Translation.V2;
using System.Threading.Tasks;

namespace Botli.Modules
{
    public class Translationmodule : ModuleBase<SocketCommandContext>
    {
        [Command("translate")]
        [Summary("Translates the input text into the given target language using the given source language. Ensure the text is enclosed in quotation marks.")]
        public Task TranslateText(string source, string target, string text)
        {
            string translated = Utils.Translate(text, target, source);

            return ReplyAsync(translated);
        }

        [Command("translate")]
        [Summary("Translates the input text into the given target language using Google Translate's auto-detect. Ensure the text is enclosed in quotation marks.")]
        public Task TranslateText(string target, string text)
        {
            string translated = Utils.Translate(text, target, null);

            return ReplyAsync(translated);
        }

        [Command("translate")]
        [Summary("Translates the input text into English using Google Translate's auto-detect.")]
        public Task TranslateText([Remainder] string text)
        {
            string translated = Utils.Translate(text, LanguageCodes.English, null);

            return ReplyAsync(translated);
        }
    }
}

using Discord;
using System;

namespace Botli
{
    public static class Constants
    {
        public const char Prefix = '-';

        public static readonly Color EmbedBlue = new Color(0, 255, 255);

        public static readonly string DiscordAPIKey = Environment.GetEnvironmentVariable("DISCORD_API_KEY");

        public static readonly string TranslateAPIKey = Environment.GetEnvironmentVariable("TRANSLATE_API_KEY");
    }
}

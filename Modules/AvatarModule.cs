using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Botli.Modules
{
    public class AvatarModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient client;

        public AvatarModule(DiscordSocketClient client)
        {
            this.client = client;
        }

        [Command("avatar")]
        [Summary("Gets the sender's avatar.")]
        public Task GetAvatar()
            => SendAvatar(Context.User);

        [Command("avatar")]
        [Summary("Gets the specified user's avatar.")]
        public Task GetAvatar(IUser user)
            => SendAvatar(user);

        [Command("avatar")]
        [Summary("Gets the specified user's avatar by user ID (works for those outside this server).")]
        public Task GetAvatar(ulong userId)
            => SendAvatar(client.Rest.GetUserAsync(userId).Result);

        private Task SendAvatar(IUser user)
        {
            if (user == null)
            {
                return ReplyAsync("Could not find the specified user.");
            }

            string url = user.GetAvatarUrl();

            if (url == null)
            {
                return ReplyAsync("User avatar could not be found.");
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder
                    .WithTitle("Profile picture successfully found:")
                    .WithImageUrl(url)
                    .WithDescription($"Avatar of {user.Mention} requested by {Context.User.Mention}.")
                    .WithColor(Constants.EmbedBlue);

                return ReplyAsync(embed: builder.Build());
            }
        }

        [Command("servericon")]
        [Summary("Gets the server icon of the server the request was sent in.")]
        public Task GetServerIcon()
            => SendServerIcon(Context.Guild);

        [Command("servericon")]
        [Summary("Gets the specified server's icon by server ID (does not work for those that this bot is not in).")]
        public Task GetServerIcon(ulong id)
        {
            IGuild guild;

            try
            {
                guild = client.GetGuild(id);
            }
            catch
            {
                guild = null;
            }

            return SendServerIcon(guild);
        }

        private Task SendServerIcon(IGuild guild)
        {
            if (guild == null)
            {
                return ReplyAsync("I am not in that server.");
            }

            string url = guild.IconUrl;

            if (url == null)
            {
                return ReplyAsync("Server icon could not be found.");
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder
                    .WithTitle("Server icon successfully found:")
                    .WithImageUrl(url)
                    .WithDescription($"Icon of {guild.Name} requested by {Context.User.Mention}.")
                    .WithColor(Constants.EmbedBlue);

                return ReplyAsync(embed: builder.Build());
            }
        }
    }
}

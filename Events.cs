using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Cloud.Translation.V2;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botli
{
    public class Events
    {
        private readonly DiscordSocketClient client;

        private readonly CommandService commandService;

        public Events(DiscordSocketClient client, CommandService commandService)
        {
            this.client = client;
            this.commandService = commandService;
        }

        public void InitialiseEvents()
        {
            client.MessageReceived += MessageReceived;

            client.Ready += Ready;

            client.ChannelCreated += ChannelCreated;

            client.Log += Log;
            commandService.Log += Log;
        }

        private Task MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot)
            {
                return Task.CompletedTask;
            }

            string contents = message.Content;

            if (message.Attachments.Count > 0 || message.Embeds.Count > 0)
            {
                IEnumerable<string> attachmentUrls = message.Attachments.Select(attachment => attachment.Url);
                IEnumerable<string> embedUrls = message.Embeds.Where(embed => embed.Type == EmbedType.Image).Select(embed => embed.Url);
                IEnumerable<string> richEmbedUrls = message.Embeds.Where(embed => embed.Type == EmbedType.Rich && embed.Image.HasValue).Select(embed => embed.Image.Value.Url);

                IEnumerable<(string, EntityAnnotation)> annotations = Utils.InspectImages(attachmentUrls.Concat(embedUrls).Concat(richEmbedUrls));

                foreach ((string, EntityAnnotation) annotation in annotations)
                {
                    EmbedBuilder builder = new EmbedBuilder();

                    builder
                        .WithTitle("Attempted transcription:")
                        .WithUrl(annotation.Item1)
                        .WithColor(Constants.EmbedBlue);

                    string translatedText = Utils.Translate(annotation.Item2.Description, LanguageCodes.English, null);

                    string text = $"{translatedText}";

                    if (text.Length < 1024)
                    {
                        builder.AddField("Text:", $"{translatedText}");

                        message.Channel.SendMessageAsync(embed: builder.Build());
                    }
                    else
                    {
                        string newMessage = $"Translated text, but it is too large to embed. Attempted transcription: {translatedText}";

                        message.Channel.SendMessageAsync(newMessage);
                    }
                }

                return Task.CompletedTask;
            }
            else if (contents.Contains("no sex"))
            {
                return message.Channel.SendMessageAsync("h");
            }
            else if (contents.ToLower().Contains("zoop"))
            {
                return message.Channel.SendMessageAsync("ZOOP :-|");
            }
            else if (message.MentionedUsers.Any(user => user.Id == client.CurrentUser.Id))
            {
                return message.Channel.SendMessageAsync($"{message.Author.Mention} STOP BEING GAY!!!");
            }

            return Task.CompletedTask;
        }

        private Task Ready()
        {
            return client.SetGameAsync($"with your balls ({Constants.Prefix}help)", null, ActivityType.Playing);
        }

        private Task ChannelCreated(SocketChannel channel)
        {
            if (channel is IMessageChannel messageChannel)
            {
                return messageChannel.SendMessageAsync("First!");
            }

            return Task.CompletedTask;
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());

            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.WebSocket;

namespace StatBot
{
    class Program
    {
        /// <summary>
        /// The Discord client
        /// </summary>
        DiscordSocketClient _client;
        MessageHandler messageHandler;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Starts the application
        /// </summary>
        /// <returns>The task.</returns>
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.MessageReceived += MessageReceived;

            string token = Bot.Default.Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            messageHandler = new MessageHandler(_client);

            // Block this task until the program is closed.
            await Task.Delay(-1);

        }

        /// <summary>
        /// Handles the received message
        /// </summary>
        /// <param name="message">The message.</param>
        private async Task MessageReceived(SocketMessage message)
        {
            var file = FileHelper.CheckAndGetFilePath(message);
            if (!message.Author.IsBot)
            {
                using (StreamWriter text = File.AppendText(file))
                {
                    string textMessage = string.Empty;
                    if (message.Embeds != null &&
                        message.Embeds.Count != 0)
                    {
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Embeds.FirstOrDefault().Url)
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username}#{message.Author.Discriminator}> {message.Content} - {message.Embeds.FirstOrDefault().Url}";
                        }
                    }
                    else if (message.Attachments != null &&
                        message.Attachments.Count != 0)
                    {
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Attachments.FirstOrDefault().Url)
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username}#{message.Author.Discriminator}> {message.Content} - {message.Attachments.FirstOrDefault().Url}";
                        }
                    }
                    else
                    {
                        textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username}#{message.Author.Discriminator}> {messageHandler.HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}")}";
                    }
                    text.WriteLine(textMessage);
                    Console.WriteLine($"#{message.Channel} - {textMessage}");
                }
            }
        }
    }
}

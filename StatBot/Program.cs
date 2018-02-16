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
        private static bool logToDebugChannel = !string.IsNullOrEmpty(Bot.Default.DebugChannelId);
        private static ulong channelId = 0;

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
            Console.WriteLine("Starting...");
            _client = new DiscordSocketClient();

            _client.MessageReceived += MessageReceived;
            _client.Disconnected += _client_Disconnected;
            
            await _client.LoginAsync(TokenType.Bot, Bot.Default.Token);
            await _client.StartAsync();

            messageHandler = new MessageHandler(_client);
            LogMessage("Ready to log.");

            // Block this task until the program is closed.
            await Task.Delay(-1);

        }

        /// <summary>
        /// Handles the event when the client disconnects. This is untested, since it's hard to fake an outage.
        /// </summary>
        private async Task _client_Disconnected(Exception arg)
        {
            Console.WriteLine($"The connection to the server has been lost at {DateTime.Now}.");
            while(_client.ConnectionState == ConnectionState.Disconnected)
            {
                await _client.LoginAsync(TokenType.Bot, Bot.Default.Token);
                await _client.StartAsync();
                System.Threading.Thread.Sleep(50000);
            }
            LogMessage("Reconnected.");
        }

        /// <summary>
        /// Handles the received message
        /// </summary>
        /// <param name="message">The message.</param>
        private Task MessageReceived(SocketMessage message)
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
            return null;
        }

        private void LogMessage(string message)
        {
            Console.WriteLine(message);
            if (logToDebugChannel)
            {
                if (channelId == 0)
                {
                    ulong.TryParse(Bot.Default.DebugChannelId, out channelId);
                }
                //Make sure the client is connected before trying to send the message.
                 for(int i = 0; i < 25; i++)
                {
                    if (_client.ConnectionState == ConnectionState.Connected)
                    {
                        try { 
                            ((ISocketMessageChannel)_client.GetChannel(channelId)).SendMessageAsync(message);
                        }
                        catch
                        {
                            Console.WriteLine("It looks like the DebugChannelId is invalid.");
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}

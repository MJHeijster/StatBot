using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using StatBot.PushoverMessaging;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot
{
    internal class Worker
    {
        private BotSettings botSettings;
        private readonly IConfiguration configuration;
        /// <summary>
        /// The Discord client
        /// </summary>
        DiscordSocketClient _client;

        /// <summary>
        /// The message handler
        /// </summary>
        MessageHandler messageHandler;
        /// <summary>
        /// A status on if discord is reconnecting
        /// </summary>
        private bool isReconnecting = false;
        private PushoverMessageHandler pushoverMessageHandler;
        /// <summary>
        /// The channel identifier
        /// </summary>
        private static ulong channelId = 0;

        public Worker(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        public void DoWork()
        {
            botSettings = new BotSettings(configuration);
            _client = new DiscordSocketClient();

            _client.MessageReceived += MessageReceived;
            _client.Disconnected += _client_Disconnected;
            if (botSettings.Application.PushOver.UsePushover)
            {
                pushoverMessageHandler = new PushoverMessageHandler(botSettings.Application.PushOver.ApiKey, botSettings.Application.PushOver.UserKey);

            }
            _client.LoginAsync(TokenType.Bot, botSettings.Discord.Token);
            _client.StartAsync();

            messageHandler = new MessageHandler(_client, botSettings);
            LogMessage($"Connected to the server at {DateTime.Now}.");

        }


        /// <summary>
        /// Handles the event when the client disconnects.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private async Task _client_Disconnected(Exception arg)
        {
            if (!isReconnecting)
            {
                isReconnecting = true;
                var disconnectTime = DateTime.Now;
                _ = Task.Run(() => LogDisconnect(disconnectTime)).ConfigureAwait(false);
                while (_client.ConnectionState == ConnectionState.Disconnected ||
                    _client.ConnectionState == ConnectionState.Disconnecting)
                {
                    var task = ReConnect();
                    if (await Task.WhenAny(task, Task.Delay(50000)) == task)
                    {
                        if ((_client.ConnectionState == ConnectionState.Connecting))
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                        if ((_client.ConnectionState == ConnectionState.Connected))
                        {
                            isReconnecting = false;
                            if (DateTime.Now >= disconnectTime.AddMilliseconds(botSettings.Application.NotificationDelay))
                                LogMessage($"Reconnected with the server at {DateTime.Now}.");
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        /// <summary>
        /// Logs the disconnect.
        /// </summary>
        /// <param name="disconnectTime">The disconnect time.</param>
        private void LogDisconnect(DateTime disconnectTime)
        {
            if (botSettings.Application.NotificationDelay <= 0)
            {
                LogMessage($"The connection to the server has been lost at {disconnectTime}.");
            }
            else
            {
                System.Threading.Thread.Sleep(botSettings.Application.NotificationDelay);
                if ((_client.ConnectionState != ConnectionState.Connected))
                {
                    LogMessage($"The connection to the server has been lost at {disconnectTime}.");
                }
            }
        }

        /// <summary>
        /// Attempts to reconnect to the server.
        /// </summary>
        private async Task ReConnect()
        {
            _client = new DiscordSocketClient();

            await _client.LoginAsync(TokenType.Bot, botSettings.Discord.Token);
            await _client.StartAsync();
            System.Threading.Thread.Sleep(10000);
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
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {messageHandler.HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel)} - {message.Embeds.FirstOrDefault().Url}";
                        }
                    }
                    else if (message.Attachments != null &&
                        message.Attachments.Count != 0)
                    {
                        if ((string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Attachments.FirstOrDefault().Url) && message.Embeds.Any())
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {messageHandler.HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel)} - {message.Attachments.FirstOrDefault().Url}";
                        }
                    }
                    else if (message.Stickers != null &&
                        message.Stickers.Count != 0)
                    {
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Stickers.FirstOrDefault().GetStickerUrl())
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Stickers.FirstOrDefault().GetStickerUrl()}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {messageHandler.HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel)} - {message.Stickers.FirstOrDefault().GetStickerUrl()}";
                        }
                    }
                    else
                    {
                        textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {messageHandler.HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel)}";
                    }
                    text.WriteLine(textMessage);
                    Console.WriteLine($"#{message.Channel} - {textMessage}");
                }
            }
            return null;
        }

        /// <summary>
        /// Logs the message to both the server and the console.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(string message)
        {
            Console.WriteLine(message);
            if (botSettings.Discord.LogToDebugChannel && _client.ConnectionState == ConnectionState.Connected)
            {
                if (channelId == 0)
                {
                    ulong.TryParse(botSettings.Discord.DebugChannelId, out channelId);
                }
                //Make sure the client is connected before trying to send the message.
                for (int i = 0; i < 25; i++)
                {
                    if (_client.ConnectionState == ConnectionState.Connected)
                    {
                        try
                        {
                            ((ISocketMessageChannel)_client.GetChannel(channelId)).SendMessageAsync(message);
                        }
                        catch
                        {
                            Console.WriteLine("It looks like the DebugChannelId is invalid.");
                            botSettings.Discord.LogToDebugChannel = false;
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            if (botSettings.Application.PushOver.UsePushover)
            {
                pushoverMessageHandler.PushMessage(message);
            }
        }
    }
}

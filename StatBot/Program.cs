﻿// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 11-05-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Jeroen Heijster">
//     Copyright ©  2017
// </copyright>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.WebSocket;
using NPushover;
using NPushover.RequestObjects;
using StatBot.Pushover;

namespace StatBot
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The Discord client
        /// </summary>
        DiscordSocketClient _client;
        
        /// <summary>
        /// The message handler
        /// </summary>
        MessageHandler messageHandler;
        
        /// <summary>
        /// Should the bot log to the debug channel or not?
        /// </summary>
        private static bool logToDebugChannel = !string.IsNullOrEmpty(Bot.Default.DebugChannelId);
        
        /// <summary>
        /// The channel identifier
        /// </summary>
        private static ulong channelId = 0;
        
        /// <summary>
        /// The use pushover or not
        /// </summary>
        private static bool usePushover = !(string.IsNullOrEmpty(Bot.Default.PushoverApi) || string.IsNullOrEmpty(Bot.Default.PushoverUserKey));

        /// <summary>
        /// The pushover user key
        /// </summary>
        private static string pushoverUserKey = Bot.Default.PushoverUserKey;

        /// <summary>
        /// The delay before sending a disconnect/reconnect notification
        /// </summary>
        private static int notificationDelay = Bot.Default.NotificationDelay;

        /// <summary>
        /// A status on if discord is reconnecting
        /// </summary>
        private bool isReconnecting = false;

        private PushoverMessageHandler pushoverMessageHandler;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

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
            if (usePushover)
            {
                pushoverMessageHandler = new PushoverMessageHandler(Bot.Default.PushoverApi, pushoverUserKey);

            }
                await _client.LoginAsync(TokenType.Bot, Bot.Default.Token);
            await _client.StartAsync();

            messageHandler = new MessageHandler(_client);
            LogMessage($"Connected to the server at {DateTime.Now}.");

            // Block this task until the program is closed.
            await Task.Delay(-1);

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
                            if (DateTime.Now >= disconnectTime.AddMilliseconds(notificationDelay))
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
        private void LogDisconnect(DateTime disconnectTime) {
            if (notificationDelay <= 0) {
                LogMessage($"The connection to the server has been lost at {disconnectTime}.");
            }
            else {
                System.Threading.Thread.Sleep(notificationDelay);
                if ((_client.ConnectionState != ConnectionState.Connected)) {
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

            await _client.LoginAsync(TokenType.Bot, Bot.Default.Token);
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
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Attachments.FirstOrDefault().Url)
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
            if (logToDebugChannel && _client.ConnectionState == ConnectionState.Connected)
            {
                if (channelId == 0)
                {
                    ulong.TryParse(Bot.Default.DebugChannelId, out channelId);
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
                            logToDebugChannel = false;
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            if (usePushover)
            {
                pushoverMessageHandler.PushMessage(message);
            }
        }
    }
}

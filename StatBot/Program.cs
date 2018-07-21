// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 22-02-2018
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
using PushoverClient;
using StatBot.Logging;

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
        /// The pushover client
        /// </summary>
        private static Pushover pclient = new Pushover(Bot.Default.PushoverApi);

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

            await _client.LoginAsync(TokenType.Bot, Bot.Default.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("with the gemstones");

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
            LogMessage($"The connection to the server has been lost at {DateTime.Now}.");
            while (_client.ConnectionState == ConnectionState.Disconnected)
            {
                var task = ReConnect();
                if (await Task.WhenAny(task, Task.Delay(50000)) == task)
                {
                    Logger.LogMessage(_client.ConnectionState.ToString());
                    if ((_client.ConnectionState == ConnectionState.Connecting))
                    {
                        System.Threading.Thread.Sleep(10000);
                    }
                    if ((_client.ConnectionState == ConnectionState.Connected))
                    {
                        LogMessage($"Reconnected with the server at {DateTime.Now}.");
                        break;
                    }
                }
                System.Threading.Thread.Sleep(5000);
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
                messageHandler.HandleMessage(message.Content, message.Author, message.Channel);
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
                PushResponse response = pclient.Push(
              "Statbot",
              message,
              Bot.Default.PushoverUserKey
          );
            }
        }
    }
}

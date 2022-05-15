﻿// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 13-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 14-05-2022
// ***********************************************************************
// <copyright file="ConnectionHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord;
using Discord.WebSocket;
using StatBot.Settings;
using System;
using System.Threading.Tasks;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class ConnectionHandler.
    /// </summary>
    internal class ConnectionHandler
    {
        /// <summary>
        /// The client
        /// </summary>
        DiscordSocketClient _client;

        /// <summary>
        /// The is reconnecting
        /// </summary>
        bool isReconnecting = false;

        /// <summary>
        /// The log handler
        /// </summary>
        private LogHandler _logHandler;

        /// <summary>
        /// The bot settings
        /// </summary>
        BotSettings _botSettings;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionHandler" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="logHandler">The log handler.</param>
        /// <param name="botSettings">The bot settings.</param>
        public ConnectionHandler(DiscordSocketClient client, LogHandler logHandler, BotSettings botSettings)
        {
            _client = client;
            _logHandler = logHandler;
            _botSettings = botSettings;
        }

        /// <summary>
        /// Handles the event when the client disconnects.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public async Task Client_Disconnected(Exception arg)
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
                            if (DateTime.Now >= disconnectTime.AddMilliseconds(_botSettings.Application.NotificationDelay))
                                _logHandler.LogMessage($"Reconnected with the server at {DateTime.Now}.", _client);
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
        public void LogDisconnect(DateTime disconnectTime)
        {
            if (_botSettings.Application.NotificationDelay <= 0)
            {
                _logHandler.LogMessage($"The connection to the server has been lost at {disconnectTime}.", _client);
            }
            else
            {
                System.Threading.Thread.Sleep(_botSettings.Application.NotificationDelay);
                if ((_client.ConnectionState != ConnectionState.Connected))
                {
                    _logHandler.LogMessage($"The connection to the server has been lost at {disconnectTime}.", _client);
                }
            }
        }

        /// <summary>
        /// Attempts to reconnect to the server.
        /// </summary>
        public async Task ReConnect()
        {
            _client = new DiscordSocketClient();

            await _client.LoginAsync(TokenType.Bot, _botSettings.Discord.Token);
            await _client.StartAsync();
            System.Threading.Thread.Sleep(10000);
        }
    }
}

// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 05-13-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 15-05-2022
// ***********************************************************************
// <copyright file="Worker.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using StatBot.Handlers;
using StatBot.PushoverMessaging;
using StatBot.Settings;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StatBot
{
    /// <summary>
    /// Class Worker.
    /// </summary>
    internal class Worker
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private BotSettings _botSettings;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The Discord client
        /// </summary>
        private DiscordSocketClient _client;

        /// <summary>
        /// The message handler
        /// </summary>
        private MessageHandler _messageHandler;

        /// <summary>
        /// The log handler
        /// </summary>
        private LogHandler _logHandler;

        /// <summary>
        /// The connection handler
        /// </summary>
        ConnectionHandler _connectionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Worker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        public void DoWork()
        {
            _botSettings = new BotSettings(_configuration);
            _client = new DiscordSocketClient();
            _logHandler = new LogHandler(_botSettings);
            _connectionHandler = new ConnectionHandler(_client, _logHandler, _botSettings);
            _messageHandler = new MessageHandler(_client, _botSettings);
            _client.MessageReceived += _messageHandler.MessageReceived;
            _client.Disconnected += _connectionHandler.Client_Disconnected;
            _client.LoginAsync(TokenType.Bot, _botSettings.Discord.Token);
            _client.StartAsync();
            _logHandler.LogMessage($"Connected to the server at {DateTime.Now}.", _client);
        }
    }
}

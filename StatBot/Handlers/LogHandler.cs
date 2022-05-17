// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 13-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 16-05-2022
// ***********************************************************************
// <copyright file="LogHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord;
using Discord.WebSocket;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class LogHandler.
    /// </summary>
    internal class LogHandler
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private BotSettings _botSettings;
        /// <summary>
        /// The channel identifier
        /// </summary>
        private static ulong channelId = 0;
        /// <summary>
        /// The pushover message handler
        /// </summary>
        private PushoverMessageHandler _pushoverMessageHandler;
        /// <summary>
        /// Initializes a new instance of the <see cref="LogHandler" /> class.
        /// </summary>
        /// <param name="botSettings">The bot settings.</param>
        public LogHandler(BotSettings botSettings)
        {
            _botSettings = botSettings;
            if (botSettings.Application.PushOver.UsePushover)
            {
                _pushoverMessageHandler = new PushoverMessageHandler(botSettings.Application.PushOver.ApiKey, botSettings.Application.PushOver.UserKey);

            }
        }

        /// <summary>
        /// Logs the message to both the server and the console.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="client">The client.</param>
        /// <param name="delayedMessage">if set to <c>true</c> [delayed message].</param>
        public void LogMessage(string message, DiscordSocketClient client, bool delayedMessage = false)
        {
            Console.WriteLine(message);
            if (_botSettings.Discord.LogToDebugChannel && client.ConnectionState == ConnectionState.Connected)
            {
                if (channelId == 0)
                {
                    ulong.TryParse(_botSettings.Discord.DebugChannelId, out channelId);
                }
                //Make sure the client is connected before trying to send the message.
                for (int i = 0; i < 25; i++)
                {
                    if (client.ConnectionState == ConnectionState.Connected)
                    {
                        try
                        {
                            ((ISocketMessageChannel)client.GetChannel(channelId)).SendMessageAsync(message);
                        }
                        catch
                        {
                            Console.WriteLine("It looks like the DebugChannelId is invalid.");
                            _botSettings.Discord.LogToDebugChannel = false;
                        }
                        break;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            if (_botSettings.Application.PushOver.UsePushover)
            {
                _pushoverMessageHandler.PushMessage(message);
            }
        }
    }
}

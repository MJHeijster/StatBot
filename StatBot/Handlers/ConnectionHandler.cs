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
    internal class ConnectionHandler
    {
        DiscordSocketClient _client;
        bool isReconnecting = false;
        /// <summary>
        /// The log handler
        /// </summary>
        private LogHandler _logHandler;
        BotSettings _botSettings;
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
        public async Task _client_Disconnected(Exception arg)
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

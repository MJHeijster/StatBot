// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 13-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 13-05-2022
// ***********************************************************************
// <copyright file="BotSettings.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Settings
{
    /// <summary>
    /// Class BotSettings.
    /// </summary>
    public class BotSettings
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BotSettings" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public BotSettings(IConfiguration configuration)
        {
            Application = new Application(configuration);
            mIRCStats = new MIRCStats(configuration);
            Discord = new Discord(configuration);
            VerifySettings();
        }

        private void VerifySettings()
        {
            if (String.IsNullOrEmpty(Discord.Token))
                throw new Exception("Discord bot token missing.");
            if (String.IsNullOrEmpty(Application.LoggingFileName))
                throw new Exception("Logging file name missing.");
        }

        /// <summary>
        /// Gets the discord.
        /// </summary>
        /// <value>The discord.</value>
        public Discord Discord { get; }
        /// <summary>
        /// Gets the m irc stats.
        /// </summary>
        /// <value>The m irc stats.</value>
        public MIRCStats mIRCStats { get; }
        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        public Application Application { get; }
    }

    /// <summary>
    /// Class Application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Application(IConfiguration configuration)
        {
            LoggingFileName = configuration.GetValue<string>("Application:LoggingFileName");
            NotificationDelay = configuration.GetValue<int?>("Application:NotificationDelay") ?? 30000;
            PushOver = new PushOver(configuration);
        }

        /// <summary>
        /// Gets the name of the logging file.
        /// </summary>
        /// <value>The name of the logging file.</value>
        public string LoggingFileName { get; }
        /// <summary>
        /// Gets the notification delay.
        /// </summary>
        /// <value>The notification delay.</value>
        public int NotificationDelay { get; }
        /// <summary>
        /// Gets the push over.
        /// </summary>
        /// <value>The push over.</value>
        public PushOver PushOver { get; }
    }

    /// <summary>
    /// Class Commands.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Commands" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Commands(IConfiguration configuration)
        {
            Prefix = configuration.GetValue<string>("Discord:Commands:Prefix");
            Exclude = configuration.GetValue<string>("Discord:Commands:Exclude");
            Include = configuration.GetValue<string>("Discord:Commands:Include");
            Stats = new Stats(configuration);
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix { get; }
        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <value>The exclude.</value>
        public string Exclude { get; }
        /// <summary>
        /// Gets the include.
        /// </summary>
        /// <value>The include.</value>
        public string Include { get; }
        /// <summary>
        /// Gets the stats.
        /// </summary>
        /// <value>The stats.</value>
        public Stats Stats { get; }
    }

    /// <summary>
    /// Class Discord.
    /// </summary>
    public class Discord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Discord" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Discord(IConfiguration configuration)
        {
            Token = configuration.GetValue<string>("Discord:Token");
            DebugChannelId = configuration.GetValue<string>("Discord:DebugChannelId");
            Commands = new Commands(configuration);
            LogToDebugChannel = !string.IsNullOrEmpty(DebugChannelId);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; }
        /// <summary>
        /// Gets the debug channel identifier.
        /// </summary>
        /// <value>The debug channel identifier.</value>
        public string DebugChannelId { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [log to debug channel].
        /// </summary>
        /// <value><c>true</c> if [log to debug channel]; otherwise, <c>false</c>.</value>
        public bool LogToDebugChannel { get; set; }
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public Commands Commands { get; }
    }

    /// <summary>
    /// Class MIRCStats.
    /// </summary>
    public class MIRCStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MIRCStats" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MIRCStats(IConfiguration configuration)
        {
            Path = configuration.GetValue<string>("MIRCStats:Path");
            NicksFile = configuration.GetValue<string>("MIRCStats:NicksFile");
            NickSection = configuration.GetValue<string>("MIRCStats:NickSection");
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; }
        /// <summary>
        /// Gets the nicks file.
        /// </summary>
        /// <value>The nicks file.</value>
        public string NicksFile { get; }
        /// <summary>
        /// Gets the nick section.
        /// </summary>
        /// <value>The nick section.</value>
        public string NickSection { get; }
    }

    /// <summary>
    /// Class PushOver.
    /// </summary>
    public class PushOver
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PushOver" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PushOver(IConfiguration configuration)
        {
            ApiKey = configuration.GetValue<string>("Application:PushOver:ApiKey");
            UserKey = configuration.GetValue<string>("Application:PushOver:UserKey");
            UsePushover = !(string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(UserKey));
        }

        /// <summary>
        /// Gets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; }
        /// <summary>
        /// Gets the user key.
        /// </summary>
        /// <value>The user key.</value>
        public string UserKey { get; }
        /// <summary>
        /// Gets a value indicating whether [use pushover].
        /// </summary>
        /// <value><c>true</c> if [use pushover]; otherwise, <c>false</c>.</value>
        public bool UsePushover { get; }
    }


    /// <summary>
    /// Class Stats.
    /// </summary>
    public class Stats
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Stats" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Stats(IConfiguration configuration)
        {
            Command = configuration.GetValue<string>("Discord:Commands:Stats:Command");
            Url = configuration.GetValue<string>("Discord:Commands:Stats:Url");
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public string Command { get; }
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; }
    }
}

// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 13-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="BotSettings.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
        }

        /// <summary>
        /// Verifies the settings.
        /// </summary>
        /// <exception cref="System.Exception">Discord bot token missing.</exception>
        /// <exception cref="System.Exception">Logging file name missing.</exception>
        public void VerifySettings()
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
        public Discord Discord { get; set; }
        /// <summary>
        /// Gets the m irc stats.
        /// </summary>
        /// <value>The m irc stats.</value>
        public MIRCStats mIRCStats { get; set; }
        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        public Application Application { get; set; }
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
            CreateNicksFileAutomatically = configuration.GetValue<bool>("Application:CreateNicksFileAutomatically");
            ShowDiscrim = configuration.GetValue<bool>("Application:ShowDiscrim");
            ShowAvatar = configuration.GetValue<bool>("Application:ShowAvatar");
            NicksFileManual = configuration.GetValue<string>("Application:NicksFileManual");
        }

        /// <summary>
        /// Gets the name of the logging file.
        /// </summary>
        /// <value>The name of the logging file.</value>
        public string LoggingFileName { get; set; }
        /// <summary>
        /// Gets the notification delay.
        /// </summary>
        /// <value>The notification delay.</value>
        public int NotificationDelay { get; set; }
        /// <summary>
        /// Gets the push over.
        /// </summary>
        /// <value>The push over.</value>
        public PushOver PushOver { get; set; }
        /// <summary>
        /// Gets a value indicating whether [create nicks file automatically].
        /// </summary>
        /// <value><c>true</c> if [create nicks file automatically]; otherwise, <c>false</c>.</value>
        public bool CreateNicksFileAutomatically { get; set; }
        /// <summary>
        /// Gets a value indicating whether [show discrim].
        /// </summary>
        /// <value><c>true</c> if [show discrim]; otherwise, <c>false</c>.</value>
        public bool ShowDiscrim { get; set; }
        /// <summary>
        /// Gets a value indicating whether [show avatar].
        /// </summary>
        /// <value><c>true</c> if [show avatar]; otherwise, <c>false</c>.</value>
        public bool ShowAvatar { get; set; }
        /// <summary>
        /// Gets the nicks file manual.
        /// </summary>
        /// <value>The nicks file manual.</value>
        public string NicksFileManual { get; set; }
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
        public string Prefix { get; set; }
        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <value>The exclude.</value>
        public string Exclude { get; set; }
        /// <summary>
        /// Gets the include.
        /// </summary>
        /// <value>The include.</value>
        public string Include { get; set; }
        /// <summary>
        /// Gets the stats.
        /// </summary>
        /// <value>The stats.</value>
        public Stats Stats { get; set; }
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
        public string Token { get; set; }
        /// <summary>
        /// Gets the debug channel identifier.
        /// </summary>
        /// <value>The debug channel identifier.</value>
        public string DebugChannelId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [log to debug channel].
        /// </summary>
        /// <value><c>true</c> if [log to debug channel]; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool LogToDebugChannel { get; set; }
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public Commands Commands { get; set; }
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
            LaunchEveryMinutes = configuration.GetValue<int>("MIRCStats:LaunchEveryMinutes");
            GeneratorFile = configuration.GetValue<string>("MIRCStats:GeneratorFile");
            WaitUntilCompleted = configuration.GetValue<bool>("MIRCStats:WaitUntilCompleted");
            UseInternalTimer = !String.IsNullOrEmpty(GeneratorFile);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }
        /// <summary>
        /// Gets the nicks file.
        /// </summary>
        /// <value>The nicks file.</value>
        public string NicksFile { get; set; }
        /// <summary>
        /// Gets the nick section.
        /// </summary>
        /// <value>The nick section.</value>
        public string NickSection { get; set; }
        /// <summary>
        /// Gets how often it should launch mIRCStats in minutes.
        /// </summary>
        /// <value>The value of how often it should launch mIRCStats in minutes.</value>
        public int LaunchEveryMinutes { get; set; }
        /// <summary>
        /// Gets the generator file.
        /// </summary>
        /// <value>The generator file.</value>
        public string GeneratorFile { get; set; }
        /// <summary>
        /// Gets a value indicating whether [wait until completed].
        /// </summary>
        /// <value><c>true</c> if [wait until completed]; otherwise, <c>false</c>.</value>
        public bool WaitUntilCompleted { get; set; }
        /// <summary>
        /// Gets a value indicating whether to [use the internal timer].
        /// </summary>
        /// <value><c>true</c> if [use the internal timer]; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool UseInternalTimer { get; set; }
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
        public string ApiKey { get; set; }
        /// <summary>
        /// Gets the user key.
        /// </summary>
        /// <value>The user key.</value>
        public string UserKey { get; set; }
        /// <summary>
        /// Gets a value indicating whether [use pushover].
        /// </summary>
        /// <value><c>true</c> if [use pushover]; otherwise, <c>false</c>.</value>

        [JsonIgnore] 
        public bool UsePushover { get; set; }
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
        public string Command { get; set; }
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }
}

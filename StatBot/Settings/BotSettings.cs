using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Settings
{
    public class BotSettings
    {

        public BotSettings(IConfiguration configuration)
        {
            Application = new Application(configuration);
            mIRCStats = new MIRCStats(configuration);
            Discord = new Discord(configuration);
        }

        public Discord Discord { get; }
        public MIRCStats mIRCStats { get; }
        public Application Application { get; }
    }

    public class Application
    {
        public Application(IConfiguration configuration)
        {
            LoggingFileName = configuration.GetValue<string>("Application:LoggingFileName");
            NotificationDelay = configuration.GetValue<int>("Application:NotificationDelay");
            PushOver = new PushOver(configuration);
        }

        public string LoggingFileName { get; }
        public int NotificationDelay { get; }
        public PushOver PushOver { get; }
    }

    public class Commands
    {
        public Commands(IConfiguration configuration)
        {
            Prefix = configuration.GetValue<string>("Discord:Commands:Prefix");
            Exclude = configuration.GetValue<string>("Discord:Commands:Exclude");
            Include = configuration.GetValue<string>("Discord:Commands:Include");
            Stats = new Stats(configuration);
        }

        public string Prefix { get; }
        public string Exclude { get; }
        public string Include { get; }
        public Stats Stats { get; }
    }

    public class Discord
    {
        public Discord(IConfiguration configuration)
        {
            Token = configuration.GetValue<string>("Discord:Token");
            DebugChannelId = configuration.GetValue<string>("Discord:DebugChannelId");
            Commands = new Commands(configuration);
            LogToDebugChannel = !string.IsNullOrEmpty(DebugChannelId);
        }

        public string Token { get; }
        public string DebugChannelId { get; }
        public bool LogToDebugChannel { get; set; }
        public Commands Commands { get; }
    }

    public class MIRCStats
    {
        public MIRCStats(IConfiguration configuration)
        {
            Path = configuration.GetValue<string>("MIRCStats:Path");
            NicksFile = configuration.GetValue<string>("MIRCStats:NicksFile");
            NickSection = configuration.GetValue<string>("MIRCStats:NickSection");
        }

        public string Path { get; }
        public string NicksFile { get; }
        public string NickSection { get; }
    }

    public class PushOver
    {

        public PushOver(IConfiguration configuration)
        {
            ApiKey = configuration.GetValue<string>("Application:PushOver:ApiKey");
            UserKey = configuration.GetValue<string>("Application:PushOver:UserKey");
            UsePushover = !(string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(UserKey));
        }

        public string ApiKey { get; }
        public string UserKey { get; }
        public bool UsePushover { get; }
    }


    public class Stats
    {

        public Stats(IConfiguration configuration)
        {
            Command = configuration.GetValue<string>("Discord:Commands:Stats:Command");
            Url = configuration.GetValue<string>("Discord:Commands:Stats:Url");
        }

        public string Command { get; }
        public string Url { get; }
    }
}

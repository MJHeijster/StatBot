using Microsoft.Extensions.Configuration;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBotConfiguration.Handlers
{
    internal class SettingsHandler
    {
        public BotSettings ReadSettings(IConfiguration configuration)
        {
            return new BotSettings(configuration);
        }
        public BotSettings SaveSettings()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot
{
    public static class CommandHandler
    {
        private static readonly string commandExclude = Bot.Default.CommandExclude;
        private static readonly string commandPrefix = Bot.Default.CommandPrefix;
        private static readonly string nickFile = $"{Bot.Default.MircStatsPath}\\{Bot.Default.MircStatsNicksFile}";
        private static readonly string exclusionSection = Bot.Default.ExclusionSection;
        public static void HandleCommand(string command, string user)
        {
            if (command == $"{commandPrefix}{commandExclude}")
            {
                string excludeString = $"{user}; MODE=ISEXCLUDED";
                if (!File.ReadLines(nickFile).Any(line => line.Contains("excludeString")))
                {
                    File.WriteAllText(nickFile, File.ReadAllText(nickFile).Replace(exclusionSection, $"{exclusionSection}{Environment.NewLine}{excludeString}"));
                }
            }
        }
    }
}

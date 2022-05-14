// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 01-13-2018
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 14-05-2022
// ***********************************************************************
// <copyright file="CommandHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord.WebSocket;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class CommandHandler.
    /// </summary>
    public class CommandHandler
    {
        /// <summary>
        /// The command exclude
        /// </summary>
        private readonly string commandExclude;
        /// <summary>
        /// The command include
        /// </summary>
        private readonly string commandInclude;
        /// <summary>
        /// The stats command
        /// </summary>
        private readonly string statsCommand;
        /// <summary>
        /// The stats URL
        /// </summary>
        private readonly string statsUrl;
        /// <summary>
        /// The command prefix
        /// </summary>
        private readonly string commandPrefix;
        /// <summary>
        /// The nick file
        /// </summary>
        private readonly string nickFile;
        /// <summary>
        /// The nick section
        /// </summary>
        private readonly string nickSection;
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler" /> class.
        /// </summary>
        /// <param name="botSettings">The bot settings.</param>
        public CommandHandler(BotSettings botSettings)
        {
            commandExclude = botSettings.Discord.Commands.Exclude;
            commandInclude = botSettings.Discord.Commands.Include;
            statsCommand = botSettings.Discord.Commands.Stats.Command;
            statsUrl = botSettings.Discord.Commands.Stats.Url;
            commandPrefix = botSettings.Discord.Commands.Prefix;
            nickFile = $"{botSettings.mIRCStats.Path}\\{botSettings.mIRCStats.NicksFile}";
            nickSection = botSettings.mIRCStats.NickSection;
        }
        /// <summary>
        /// Handles the commands that are available.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="user">The user who initiated the command.</param>
        /// <param name="channel">The channel.</param>
        public void HandleCommand(string command, string user, ISocketMessageChannel channel)
        {
            string excludeString = $"{user}; MODE=ISEXCLUDED";
            string includeString = $"{user};";
            if (!string.IsNullOrEmpty(commandPrefix))
            {
                if (!string.IsNullOrEmpty(commandExclude) && command == $"{commandPrefix}{commandExclude}")
                {
                    if (!File.ReadLines(nickFile).Any(line => line.Contains(excludeString)))
                    {
                        File.WriteAllText(nickFile, File.ReadAllText(nickFile).Replace(nickSection, $"{nickSection}{Environment.NewLine}{excludeString}"));
                    }
                    if (!File.ReadLines(nickFile).Any(line => line.Contains(includeString)))
                    {
                        File.WriteAllText(nickFile, File.ReadAllText(nickFile).Replace(includeString, string.Empty));
                    }
                }
                if (!string.IsNullOrEmpty(commandInclude) && command == $"{commandPrefix}{commandInclude}")
                {
                    if (!File.ReadLines(nickFile).Any(line => line.Contains(includeString)))
                    {
                        File.WriteAllText(nickFile, File.ReadAllText(nickFile).Replace(nickSection, $"{nickSection}{Environment.NewLine}{includeString}"));
                    }
                    if (File.ReadLines(nickFile).Any(line => line.Contains(excludeString)))
                    {
                        File.WriteAllText(nickFile, File.ReadAllText(nickFile).Replace(excludeString, string.Empty));
                    }
                }
                if (!string.IsNullOrEmpty(statsCommand) && !string.IsNullOrEmpty(statsUrl) && command == $"{commandPrefix}{statsCommand}")
                {
                    channel.SendMessageAsync(statsUrl);
                }
            }
        }
    }
}

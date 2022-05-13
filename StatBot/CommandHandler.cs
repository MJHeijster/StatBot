// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 01-13-2018
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-02-2018
// ***********************************************************************
// <copyright file="CommandHandler.cs" company="Jeroen Heijster">
//     Copyright ©  2017
// </copyright>
// ***********************************************************************
using Discord.WebSocket;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot
{
    public class CommandHandler
    {
        private static BotSettings _botSettings;
        private readonly string commandExclude;
        private readonly string commandInclude;
        private readonly string statsCommand;
        private readonly string statsUrl;
        private readonly string commandPrefix;
        private readonly string nickFile;
        private readonly string nickSection;
        public CommandHandler(BotSettings botSettings)
        {
            _botSettings = botSettings;
            commandExclude = _botSettings.Discord.Commands.Exclude;
            commandInclude = _botSettings.Discord.Commands.Include;
            statsCommand = _botSettings.Discord.Commands.Stats.Command;
            statsUrl = _botSettings.Discord.Commands.Stats.Url;
            commandPrefix = _botSettings.Discord.Commands.Prefix;
            nickFile = $"{_botSettings.mIRCStats.Path}\\{_botSettings.mIRCStats.NicksFile}";
            nickSection = _botSettings.mIRCStats.NickSection;
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
                    if (!File.ReadLines(nickFile).Any(line => line.Contains(excludeString)))
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

﻿// ***********************************************************************
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
        private static readonly string commandInclude = Bot.Default.CommandInclude;
        private static readonly string commandPrefix = Bot.Default.CommandPrefix;
        private static readonly string nickFile = $"{Bot.Default.MircStatsPath}\\{Bot.Default.MircStatsNicksFile}";
        private static readonly string nickSection = Bot.Default.NickSection;

        /// <summary>
        /// Handles the commands that are available.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="user">The user who initiated the command.</param>
        public static void HandleCommand(string command, string user)
        {
            string excludeString = $"{user}; MODE=ISEXCLUDED";
            string includeString = $"{user};";
            if (command == $"{commandPrefix}{commandExclude}")
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
            if (command == $"{commandPrefix}{commandInclude}")
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
        }
    }
}

// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 01-13-2018
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 04-06-2022
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
        /// The client
        /// </summary>
        internal DiscordSocketClient _client;
        /// <summary>
        /// The bot settings
        /// </summary>
        private readonly BotSettings _botSettings;
        /// <summary>
        /// Gets or sets the admin user identifier.
        /// </summary>
        /// <value>The admin user identifier.</value>
        private readonly ulong adminUserId;
        /// <summary>
        /// Gets or sets a value indicating whether [server admins are allowed to use these commands].
        /// </summary>
        /// <value><c>true</c> if [server admins are allowed to use these commands]; otherwise, <c>false</c>.</value>
        private readonly bool allowServerAdmins;
        /// <summary>
        /// Gets or sets the link user command.
        /// </summary>
        /// <value>The link user command.</value>
        private readonly string linkUserCommand;
        /// <summary>
        /// Gets or sets the override username command.
        /// </summary>
        /// <value>The override username command.</value>
        private readonly string overrideUsernameCommand;
        /// <summary>
        /// The remove override username command
        /// </summary>
        private readonly string removeOverrideUsernameCommand;
        /// <summary>
        /// The use command include
        /// </summary>
        private readonly bool useCommandInclude;
        /// <summary>
        /// The use command exclude
        /// </summary>
        private readonly bool useCommandExclude;
        /// <summary>
        /// The use command stats
        /// </summary>
        private readonly bool useCommandStats;
        /// <summary>
        /// The use link user command
        /// </summary>
        private readonly bool useLinkUserCommand;
        /// <summary>
        /// The use override username command
        /// </summary>
        private readonly bool useOverrideUsernameCommand;
        /// <summary>
        /// The use remove override username command
        /// </summary>
        private readonly bool useRemoveOverrideUsernameCommand;
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="botSettings">The bot settings.</param>
        public CommandHandler(DiscordSocketClient client, BotSettings botSettings)
        {
            _botSettings = botSettings;
            _client = client;
            commandExclude = botSettings.Discord.Commands.Exclude;
            commandInclude = botSettings.Discord.Commands.Include;
            statsCommand = botSettings.Discord.Commands.Stats.Command;
            statsUrl = botSettings.Discord.Commands.Stats.Url;
            adminUserId = botSettings.Discord.Commands.AdminCommands.AdminUserId;
            allowServerAdmins = botSettings.Discord.Commands.AdminCommands.AllowServerAdmins;
            linkUserCommand = botSettings.Discord.Commands.AdminCommands.LinkUserCommand;
            overrideUsernameCommand = botSettings.Discord.Commands.AdminCommands.OverrideUsernameCommand;
            removeOverrideUsernameCommand = botSettings.Discord.Commands.AdminCommands.RemoveOverrideUsernameCommand;
            commandPrefix = botSettings.Discord.Commands.Prefix;
            nickFile = $"{botSettings.mIRCStats.Path}\\{botSettings.mIRCStats.NicksFile}";
            nickSection = botSettings.mIRCStats.NickSection;
            useCommandInclude = !string.IsNullOrEmpty(commandInclude);
            useCommandExclude = !string.IsNullOrEmpty(commandExclude);
            useCommandStats = !string.IsNullOrEmpty(statsCommand) && !string.IsNullOrEmpty(statsUrl);
            useLinkUserCommand = !string.IsNullOrEmpty(linkUserCommand);
            useOverrideUsernameCommand = !string.IsNullOrEmpty(overrideUsernameCommand);
            useRemoveOverrideUsernameCommand = !string.IsNullOrEmpty(removeOverrideUsernameCommand);
        }
        /// <summary>
        /// Handles the commands that are available.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="user">The user who initiated the command.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="author">The author.</param>
        public void HandleCommand(string command, string[] fullCommand, string user, ISocketMessageChannel channel, SocketUser author)
        {
            string excludeString = $"{user}; MODE=ISEXCLUDED";
            string includeString = $"{user};";
            if (!string.IsNullOrEmpty(commandPrefix))
            {
                if (useCommandInclude && command == $"{commandPrefix}{commandInclude}")
                    HandleIncludeCommand(command, user, channel, author.Id, excludeString, includeString);
                else if (useCommandExclude && command == $"{commandPrefix}{commandExclude}")
                    HandleExcludeCommand(command, user, channel, author.Id, excludeString, includeString);
                else if (useCommandStats && command == $"{commandPrefix}{statsCommand}")
                    channel.SendMessageAsync(statsUrl);
                else if (useLinkUserCommand && command == $"{commandPrefix}{linkUserCommand}")
                    HandleLinkUserCommand(author.Id, user);
                else if (useOverrideUsernameCommand && command == $"{commandPrefix}{overrideUsernameCommand}")
                    HandleOverrideUsernameCommand(fullCommand, author.Id, user, author);
                else if (useRemoveOverrideUsernameCommand && command == $"{commandPrefix}{removeOverrideUsernameCommand}")
                    HandleRemoveOverrideUsernameCommand(fullCommand, user, channel, author.Id, excludeString, includeString, author);
            }
        }

        /// <summary>
        /// Handles the remove override username command.
        /// </summary>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="user">The user.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="excludeString">The exclude string.</param>
        /// <param name="includeString">The include string.</param>
        /// <param name="author">The author.</param>
        private void HandleRemoveOverrideUsernameCommand(string[] fullCommand, string user, ISocketMessageChannel channel, ulong userid, string excludeString, string includeString, SocketUser author)
        {
            var guildUser = (_client.GetUser(author.Id) as SocketGuildUser);
            if (_botSettings.Application.CreateNicksFileAutomatically &&
                ((guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                _botSettings.Discord.Commands.AdminCommands.AdminUserId == author.Id))
            {
                Database.DatabaseHandlers.UserHandler.OverrideUsername(Convert.ToUInt64(fullCommand[1]), null);
            }
        }

        /// <summary>
        /// Handles the override username command.
        /// </summary>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="user">The user.</param>
        /// <param name="author">The author.</param>
        private void HandleOverrideUsernameCommand(string[] fullCommand, ulong userid, string user, SocketUser author)
        {
            var guildUser = (_client.GetUser(author.Id) as SocketGuildUser);
            if (_botSettings.Application.CreateNicksFileAutomatically &&
                ((guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                _botSettings.Discord.Commands.AdminCommands.AdminUserId == author.Id))
            {
                Database.DatabaseHandlers.UserHandler.OverrideUsername(Convert.ToUInt64(fullCommand[1]), user);
            }
        }

        /// <summary>
        /// Handles the link user command.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="user">The user.</param>
        private void HandleLinkUserCommand(ulong userid, string user)
        {
            if (_botSettings.Application.CreateNicksFileAutomatically)
            {
                Database.DatabaseHandlers.UserHandler.AddOldUsername(userid, user);
            }
        }

        /// <summary>
        /// Handles the exclude command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="user">The user.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="excludeString">The exclude string.</param>
        /// <param name="includeString">The include string.</param>
        private void HandleExcludeCommand(string command, string user, ISocketMessageChannel channel, ulong userid, string excludeString, string includeString)
        {
            if (_botSettings.Application.CreateNicksFileAutomatically)
            {
                Database.DatabaseHandlers.UserHandler.ExcludeFromStats(userid, true);
            }
            else
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
        }

        /// <summary>
        /// Handles the include command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="user">The user.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="excludeString">The exclude string.</param>
        /// <param name="includeString">The include string.</param>
        private void HandleIncludeCommand(string command, string user, ISocketMessageChannel channel, ulong userid, string excludeString, string includeString)
        {
            if (_botSettings.Application.CreateNicksFileAutomatically)
            {
                Database.DatabaseHandlers.UserHandler.ExcludeFromStats(userid, false);
            }
            else
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
        }
    }
}

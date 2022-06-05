// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 01-13-2018
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 05-06-2022
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
        /// The create old user command
        /// </summary>
        private readonly string createOldUserCommand;


        /// <summary>
        /// The log handler
        /// </summary>
        private LogHandler _logHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="botSettings">The bot settings.</param>
        /// <param name="logHandler">The log handler.</param>
        public CommandHandler(DiscordSocketClient client, BotSettings botSettings, LogHandler logHandler)
        {
            _botSettings = botSettings;
            _client = client;
            _logHandler = logHandler;
            commandExclude = botSettings.Discord.Commands.Exclude;
            commandInclude = botSettings.Discord.Commands.Include;
            statsCommand = botSettings.Discord.Commands.Stats.Command;
            statsUrl = botSettings.Discord.Commands.Stats.Url;
            adminUserId = botSettings.Discord.Commands.AdminCommands.AdminUserId;
            allowServerAdmins = botSettings.Discord.Commands.AdminCommands.AllowServerAdmins;
            linkUserCommand = botSettings.Discord.Commands.AdminCommands.LinkUserCommand;
            overrideUsernameCommand = botSettings.Discord.Commands.AdminCommands.OverrideUsernameCommand;
            removeOverrideUsernameCommand = botSettings.Discord.Commands.AdminCommands.RemoveOverrideUsernameCommand;
            createOldUserCommand = botSettings.Discord.Commands.AdminCommands.CreateOldUserCommand;
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
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="user">The user.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="author">The author.</param>
        public void HandleCommand(string command, string[] fullCommand, string user, ISocketMessageChannel channel, SocketUser author)
        {
            string excludeString = $"{user}; MODE=ISEXCLUDED";
            string includeString = $"{user};";
            if (!string.IsNullOrEmpty(commandPrefix))
            {
                if (useCommandInclude && command == $"{commandPrefix}{commandInclude}")
                    HandleIncludeCommand(channel, author.Id, excludeString, includeString);
                else if (useCommandExclude && command == $"{commandPrefix}{commandExclude}")
                    HandleExcludeCommand(channel, author.Id, excludeString, includeString);
                else if (useCommandStats && command == $"{commandPrefix}{statsCommand}")
                    channel.SendMessageAsync(statsUrl);
                else if (useLinkUserCommand && command == $"{commandPrefix}{linkUserCommand}")
                    HandleLinkUserCommand(author.Id, fullCommand, channel);
                else if (useOverrideUsernameCommand && command == $"{commandPrefix}{overrideUsernameCommand}")
                    HandleOverrideUsernameCommand(fullCommand, author, channel);
                else if (useRemoveOverrideUsernameCommand && command == $"{commandPrefix}{removeOverrideUsernameCommand}")
                    HandleRemoveOverrideUsernameCommand(fullCommand, author, channel);
                else if (command == $"{commandPrefix}{createOldUserCommand}")
                    HandleCreateOldUserCommand(fullCommand, author, channel);
            }
        }

        /// <summary>
        /// Handles the create old user command.
        /// </summary>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="author">The author.</param>
        /// <param name="channel">The channel.</param>
        private async void HandleCreateOldUserCommand(string[] fullCommand, SocketUser author, ISocketMessageChannel channel)
        {
            try
            {
                if (fullCommand.Length < 3 || 
                    fullCommand[2].Split('#').Length < 2 ||                    
                    string.IsNullOrEmpty(fullCommand[1]))
                {
                    channel.SendMessageAsync($"Command Usage: {commandPrefix}{createOldUserCommand} <userid> <username#discrim>");
                    return;
                }
                var user = fullCommand[2].Split('#');
                SocketGuildUser guildUser = GetGuildUser(author.Id);
                if (_botSettings.Application.CreateNicksFileAutomatically &&
                    ((guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                    _botSettings.Discord.Commands.AdminCommands.AdminUserId == author.Id))
                {
                    //object id, object username, object discrim, object avatarUri, object isBot, object isExcludedFromStats, object overrideName
                    Database.DatabaseHandlers.UserHandler.InsertOrUpdateUser(new Database.Entities.User(
                        fullCommand[1],
                         user[0],
                        user[1],
                        "",
                        false,
                        false,
                        null
                    ));
                    channel.SendMessageAsync($"Added user with userid: {fullCommand[1]}, {user[0]}#{user[1]}");
                }
                else
                {
                    channel.SendMessageAsync($"You don't have permission to use this command.");
                }
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }

        /// <summary>
        /// Handles the remove override username command.
        /// </summary>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="author">The author.</param>
        /// <param name="channel">The channel.</param>
        private void HandleRemoveOverrideUsernameCommand(string[] fullCommand, SocketUser author, ISocketMessageChannel channel)
        {
            try
            {
                if (fullCommand.Length < 2 ||
                    string.IsNullOrEmpty(fullCommand[1]))
                {
                    channel.SendMessageAsync($"Command Usage: {commandPrefix}{removeOverrideUsernameCommand} <userid>");
                    return;
                }
                SocketGuildUser guildUser = GetGuildUser(author.Id);
                if (_botSettings.Application.CreateNicksFileAutomatically &&
                    ((guildUser != null && guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                    _botSettings.Discord.Commands.AdminCommands.AdminUserId == author.Id))
                {
                    Database.DatabaseHandlers.UserHandler.OverrideUsername(Convert.ToUInt64(fullCommand[1]), null);
                    channel.SendMessageAsync($"Removing username override for user id: {fullCommand[1]}");
                }
                else
                {
                    channel.SendMessageAsync($"You don't have permission to use this command.");
                }
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }

        /// <summary>
        /// Handles the override username command.
        /// </summary>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="author">The author.</param>
        /// <param name="channel">The channel.</param>
        private void HandleOverrideUsernameCommand(string[] fullCommand, SocketUser author, ISocketMessageChannel channel)
        {
            try
            {
                if (fullCommand.Length < 3 ||
                    string.IsNullOrEmpty(fullCommand[1]) ||
                    string.IsNullOrEmpty(fullCommand[2]))
                {
                    channel.SendMessageAsync($"Command Usage: {commandPrefix}{overrideUsernameCommand} <userid> <username>");
                    return;
                }
                SocketGuildUser guildUser = GetGuildUser(author.Id);
                if (_botSettings.Application.CreateNicksFileAutomatically &&
                    ((guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                    _botSettings.Discord.Commands.AdminCommands.AdminUserId == author.Id))
                {
                    Database.DatabaseHandlers.UserHandler.OverrideUsername(Convert.ToUInt64(fullCommand[1]), fullCommand[2]);
                channel.SendMessageAsync($"Setting username {fullCommand[2]} for user id: {fullCommand[1]}");
                }
                else
                {
                    channel.SendMessageAsync($"You don't have permission to use this command.");
                }
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }

        /// <summary>
        /// Handles the link user command.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="fullCommand">The full command.</param>
        /// <param name="channel">The channel.</param>
        private void HandleLinkUserCommand(ulong userid, string[] fullCommand, ISocketMessageChannel channel)
        {
            try
            {
                if (fullCommand.Length < 3 ||
                    string.IsNullOrEmpty(fullCommand[1]) ||
                    string.IsNullOrEmpty(fullCommand[2]) ||
                    fullCommand[2].Split('#').Length < 2)
                {
                    channel.SendMessageAsync($"Command Usage: {commandPrefix}{linkUserCommand} <userid> <username#discrim>");
                    return;
                }
                SocketGuildUser guildUser = GetGuildUser(userid);
                if (_botSettings.Application.CreateNicksFileAutomatically &&
                    ((guildUser.GuildPermissions.Administrator && _botSettings.Discord.Commands.AdminCommands.AllowServerAdmins) ||
                    _botSettings.Discord.Commands.AdminCommands.AdminUserId == userid))
                {
                    Database.DatabaseHandlers.UserHandler.AddOldUsername(Convert.ToUInt64(fullCommand[1]), fullCommand[2]);
                channel.SendMessageAsync($"Linking {fullCommand[2]} with user id: {fullCommand[1]}");
                }
                else
                {
                    channel.SendMessageAsync($"You don't have permission to use this command.");
                }
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }

        /// <summary>
        /// Handles the exclude command.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="excludeString">The exclude string.</param>
        /// <param name="includeString">The include string.</param>
        private void HandleExcludeCommand(ISocketMessageChannel channel, ulong userid, string excludeString, string includeString)
        {
            try
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
                channel.SendMessageAsync("You will be excluded in the stats.");
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }

        /// <summary>
        /// Handles the include command.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="excludeString">The exclude string.</param>
        /// <param name="includeString">The include string.</param>
        private void HandleIncludeCommand(ISocketMessageChannel channel, ulong userid, string excludeString, string includeString)
        {
            try
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
                channel.SendMessageAsync("You will be included in the stats.");
            }
            catch (Exception e)
            {
                channel.SendMessageAsync("Something went wrong executing this command.");
                LogMethodAndException(e);
            }
        }
        /// <summary>
        /// Gets the guild user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>SocketGuildUser.</returns>
        private SocketGuildUser GetGuildUser(ulong id)
        {
            foreach (var guild in _client.Guilds)
            {
                if (guild.Users.Any(c => c.Id == id))
                {
                    return guild.Users.First(c => c.Id == id);
                }
            }
            return null;
        }
        /// <summary>
        /// Logs the method and exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="caller">The caller.</param>
        private void LogMethodAndException(Exception e, [System.Runtime.CompilerServices.CallerMemberName] string caller = null)
        {
            _logHandler.LogMessage($"{caller} went wrong with exception {e.Message}.", _client);
        }
    }
}

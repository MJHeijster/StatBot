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
using Discord;
using Discord.WebSocket;
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
        private static readonly string commandPrefix = Bot.Default.CommandPrefix;
        private static readonly string oddSnapRoleString = Bot.Default.OddSnapRole;
        private static readonly string evenSnapRoleString = Bot.Default.EvenSnapRole;
        private static bool evenRole = true;
        private static readonly string snapCommand = Bot.Default.SnapCommand;
        private static readonly string unsnapCommand = Bot.Default.UnsnapCommand;
        private static readonly string secretSnapCommand = Bot.Default.SecretSnapCommand;
        private static readonly string thanosEventRoleString = Bot.Default.ThanosEventRole;
        private static SocketRole oddSnapRole;
        private static SocketRole evenSnapRole;
        private static SocketRole thanosEventRole;



        /// <summary>
        /// Handles the commands that are available.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="user">The user who initiated the command.</param>
        /// <param name="channel">The channel.</param>
        public static async void HandleCommand(string command, SocketUser user, ISocketMessageChannel channel)
        {
            var guild = (channel as SocketGuildChannel)?.Guild;
            if (oddSnapRole == null ||
                evenSnapRole == null)
            {
                oddSnapRole = guild.Roles.FirstOrDefault(x => x.Name == oddSnapRoleString);
                evenSnapRole = guild.Roles.FirstOrDefault(x => x.Name == evenSnapRoleString);
                thanosEventRole = guild.Roles.FirstOrDefault(x => x.Name == thanosEventRoleString);
            }
            if (!string.IsNullOrEmpty(commandPrefix))
            {
                if (!string.IsNullOrEmpty(unsnapCommand) && command == $"{commandPrefix}{unsnapCommand}")
                {
                    await (user as IGuildUser).RemoveRoleAsync(oddSnapRole);
                    await (user as IGuildUser).RemoveRoleAsync(evenSnapRole);
                    await (user as IGuildUser).RemoveRoleAsync(thanosEventRole);

                }
                if (!string.IsNullOrEmpty(snapCommand) && command == $"{commandPrefix}{snapCommand}")
                {
                    if (thanosEventRole != null)
                    await (user as IGuildUser).AddRoleAsync(thanosEventRole);
                    if (evenRole)
                    {
                        await (user as IGuildUser).AddRoleAsync(evenSnapRole);
                        await (user as IGuildUser).RemoveRoleAsync(oddSnapRole);
                        await channel.SendMessageAsync("You have been judged with a role.");
                    }
                    else
                    {
                        await (user as IGuildUser).AddRoleAsync(oddSnapRole);
                        await (user as IGuildUser).RemoveRoleAsync(evenSnapRole);
                        await channel.SendMessageAsync("You have been judged with a role.");
                    }
                    evenRole = !evenRole;
                }
                if (!string.IsNullOrEmpty(secretSnapCommand) && command == $"{commandPrefix}{secretSnapCommand}")
                {
                    await channel.SendMessageAsync("Time to bring balance...");
                    System.Threading.Thread.Sleep(5000);
                    await channel.SendMessageAsync("The following role is snapped out of existence and you won't feel so good....");
                    Random rand = new Random();
                    if (rand.Next(0, 2) == 0)
                        await channel.SendMessageAsync(MentionUtils.MentionRole(oddSnapRole.Id));
                    else
                        await channel.SendMessageAsync(MentionUtils.MentionRole(evenSnapRole.Id));
                    throw new Exception("My work is done.");
                }
            }

        }
    }
}

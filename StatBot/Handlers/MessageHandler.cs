// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-12-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="MessageHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CSharpVerbalExpressions;
using Discord.WebSocket;
using StatBot.Database.DatabaseHandlers;
using StatBot.Database.Entities;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class MessageHandler.
    /// </summary>
    public class MessageHandler
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private static BotSettings _botSettings;

        /// <summary>
        /// The command prefix
        /// </summary>
        private readonly string commandPrefix;

        //example: <:phew:19095755581184>
        /// <summary>
        /// The emoji expression
        /// </summary>
        private readonly VerbalExpressions emojiExpression = new VerbalExpressions().StartOfLine().Anything().Then("<:").Anything().Then(":").Anything().Then(">").Anything();

        //example: <a:phew:19095755581184>
        /// <summary>
        /// The animated emoji expression
        /// </summary>
        private readonly VerbalExpressions animatedEmojiExpression = new VerbalExpressions().StartOfLine().Anything().Then("<a:").Anything().Then(":").Anything().Then(">").Anything();

        //example: <@19095755581184>
        /// <summary>
        /// The user mention expression
        /// </summary>
        private readonly VerbalExpressions userMentionExpression = new VerbalExpressions().StartOfLine().Anything().Then("<@").Anything().Then(">").Anything();

        //example: <#19095755581184>
        /// <summary>
        /// The channel expression
        /// </summary>
        private readonly VerbalExpressions channelExpression = new VerbalExpressions().StartOfLine().Anything().Then("<#").Anything().Then(">").Anything();

        //example: !command
        /// <summary>
        /// The command expression
        /// </summary>
        private readonly VerbalExpressions commandExpression;

        /// <summary>
        /// The client
        /// </summary>
        internal DiscordSocketClient _client;

        /// <summary>
        /// The command handler
        /// </summary>
        private CommandHandler _commandHandler;


        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler" /> class.
        /// </summary>
        /// <param name="client">The Discord client.</param>
        /// <param name="botSettings">The bot settings.</param>
        public MessageHandler(DiscordSocketClient client, BotSettings botSettings)
        {
            _client = client;
            _botSettings = botSettings;
            _commandHandler = new CommandHandler(botSettings);
            commandPrefix = _botSettings.Discord.Commands.Prefix;
            commandExpression = new VerbalExpressions().StartOfLine().Then(commandPrefix).Anything();
        }

        /// <summary>
        /// Handles the received message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task MessageReceived(SocketMessage message)
        {
            var file = FileHandler.CheckAndGetFilePath(message);
            if (!message.Author.IsBot)
            {
                var user = new User(message.Author);
                UserHandler.InsertOrUpdateUser(user);
                using (StreamWriter text = File.AppendText(file))
                {
                    string textMessage = string.Empty;
                    if (message.Embeds != null &&
                        message.Embeds.Count != 0)
                    {
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Embeds.FirstOrDefault().Url)
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel, message.Author.Id)} - {message.Embeds.FirstOrDefault().Url}";
                        }
                    }
                    else if (message.Attachments != null &&
                        message.Attachments.Count != 0)
                    {
                        if ((string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Attachments.FirstOrDefault().Url) && message.Embeds.Any())
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Embeds.FirstOrDefault().Url}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel, message.Author.Id)} - {message.Attachments.FirstOrDefault().Url}";
                        }
                    }
                    else if (message.Stickers != null &&
                        message.Stickers.Count != 0)
                    {
                        if (string.IsNullOrEmpty(message.Content) ||
                            message.Content == message.Stickers.FirstOrDefault().GetStickerUrl())
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {message.Stickers.FirstOrDefault().GetStickerUrl()}";
                        }
                        else
                        {
                            textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel, message.Author.Id)} - {message.Stickers.FirstOrDefault().GetStickerUrl()}";
                        }
                    }
                    else
                    {
                        textMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")}] <{message.Author.Username.Replace(' ', '_')}#{message.Author.Discriminator}> {HandleMessage(message.Content, $"{message.Author.Username}#{message.Author.Discriminator}", message.Channel, message.Author.Id)}";
                    }
                    text.WriteLine(textMessage);
                    Console.WriteLine($"#{message.Channel} - {textMessage}");
                }
            }
            return null;
        }

        /// <summary>
        /// Handles the incoming message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="userid">The userid.</param>
        /// <returns>The parsed message.</returns>
        public string HandleMessage(string message, string userName, ISocketMessageChannel channel, ulong userid)
        {
            message = Regex.Replace(message, @"\r\n?|\n", " ");
            StringBuilder returnMessage = new StringBuilder();
            if (emojiExpression.IsMatch(message) ||
                animatedEmojiExpression.IsMatch(message) ||
                userMentionExpression.IsMatch(message) ||
                channelExpression.IsMatch(message) ||
                commandExpression.IsMatch(message))
            {
                string[] messageParts = message.Split(' ');
                bool firstPart = true;
                foreach (string messagePart in messageParts)
                {
                    if (firstPart &&
                        commandExpression.IsMatch(messagePart))
                    {
                        _commandHandler.HandleCommand(messagePart, userName.Replace(' ', '_'), channel, userid);
                    }
                    if (emojiExpression.IsMatch(messagePart) ||
                        animatedEmojiExpression.IsMatch(messagePart))
                    {
                        string[] splitString = messagePart.Split('>');
                        returnMessage.Append($":{splitString[0].Split(':')[1]}: ");
                        if (splitString.Length > 1)
                        {
                            returnMessage.Append(splitString[1]);
                        }
                    }
                    else if (userMentionExpression.IsMatch(messagePart))
                    {
                        returnMessage.Append($"{ResolveUserMention(messagePart).Replace(' ', '_')} ");
                    }
                    else if (channelExpression.IsMatch(messagePart))
                    {
                        returnMessage.Append(ResolveChannelName(messagePart));
                    }
                    else
                    {
                        returnMessage.Append($"{messagePart} ");
                    }
                    firstPart = false;
                }
            }
            return returnMessage.Length > 0 ? returnMessage.ToString() : message;
        }

        /// <summary>
        /// Resolves the user mention.
        /// </summary>
        /// <param name="messagePart">The part of the message that needs to be resolved.</param>
        /// <returns>A StringBuilder with the resolved username.</returns>
        private StringBuilder ResolveUserMention(string messagePart)
        {
            StringBuilder returnMessage = new StringBuilder();
            int leadingCharacters = messagePart.TakeWhile(c => c == '<').Count();
            string[] messagePartPart = messagePart.Split('>');
            ulong userId = 0;
            int minusLength = 2;
            string substr = string.Empty;
            if (messagePartPart.Length >= 2 &&
                !string.IsNullOrEmpty(messagePartPart[1]))
            {
                minusLength++;
            }
            else
            {
                substr = messagePartPart[0].Substring(1 + leadingCharacters, messagePart.Length - minusLength - leadingCharacters);
                ulong.TryParse(substr, out userId);
            }

            if (userId == 0)
            {
                minusLength++;
                substr = messagePartPart[0].Substring(2 + leadingCharacters, messagePart.Length - minusLength - leadingCharacters);
                ulong.TryParse(substr, out userId);
            }
            if (userId > 0)
            {
                SocketUser user = ResolveUser(userId);
                returnMessage.Append($"@{user.Username}#{user.Discriminator}");
                if (messagePartPart.Length >= 2 &&
                !string.IsNullOrEmpty(messagePartPart[1]))
                {
                    returnMessage.Append(messagePartPart[1]);
                }
            }
            else
            {
                returnMessage.Append($"{messagePart} ");
            }
            return returnMessage;
        }

        /// <summary>
        /// Resolves the name of the channel.
        /// </summary>
        /// <param name="messagePart">The part of the message that needs to be resolved.</param>
        /// <returns>A StringBuilder with the resolved channel name.</returns>
        private StringBuilder ResolveChannelName(string messagePart)
        {
            StringBuilder returnMessage = new StringBuilder();
            int leadingCharacters = messagePart.TakeWhile(c => c == '<').Count();
            string[] channelPartPart = messagePart.Split('>');
            ulong channelId = 0;
            int minusLength = 2;
            string substr = string.Empty;
            if (channelPartPart.Length >= 2 &&
                !string.IsNullOrEmpty(channelPartPart[1]))
            {
                minusLength++;
            }
            else
            {
                substr = channelPartPart[0].Substring(1 + leadingCharacters, messagePart.Length - minusLength - leadingCharacters);
                ulong.TryParse(substr, out channelId);
            }

            if (channelId == 0)
            {
                substr = channelPartPart[0].Substring(1 + leadingCharacters, messagePart.Length - minusLength - leadingCharacters);
                ulong.TryParse(substr, out channelId);
            }
            if (channelId > 0)
            {
                var channel = _client.GetChannel(channelId);
                returnMessage.Append($"#{channel} ");
                if (channelPartPart.Length >= 2 &&
                !string.IsNullOrEmpty(channelPartPart[1]))
                {
                    returnMessage.Append(channelPartPart[1]);
                }
            }
            else
            {
                returnMessage.Append($"{messagePart} ");
            }
            return returnMessage;
        }

        /// <summary>
        /// Resolves the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user that's been resolved.</returns>
        private SocketUser ResolveUser(ulong userId)
        {
            var user = _client.GetUser(userId);
            //Search in the guild if it cannot be found.
            if (user == null)
            {
                List<SocketGuildUser> users = new List<SocketGuildUser>();
                foreach (SocketGuild guild in _client.Guilds)
                {
                    users.AddRange(guild.Users);
                }

                user = users.FirstOrDefault(x => x.Id == userId);
            }
            return user;
        }
    }
}

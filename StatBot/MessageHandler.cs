using CSharpVerbalExpressions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot
{
    internal class MessageHandler
    {
        //example: <:phew:19095755581184>
        private readonly VerbalExpressions emojiExpression = new VerbalExpressions().StartOfLine().Anything().Then("<:").Anything().Then(":").Anything();
        //example: <@19095755581184>
        private readonly VerbalExpressions userMentionExpression = new VerbalExpressions().StartOfLine().Anything().Then("<@").Anything().Then(">").Anything();
        //example: <#19095755581184>
        private readonly VerbalExpressions channelExpression = new VerbalExpressions().StartOfLine().Anything().Then("<#").Anything().Then(">").Anything();
        internal DiscordSocketClient _client;

        internal MessageHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        internal string CleanMessage(string message)
        {
            StringBuilder returnMessage = new StringBuilder();
            if (emojiExpression.IsMatch(message) ||
                userMentionExpression.IsMatch(message) ||
                channelExpression.IsMatch(message))
            {
                string[] messageParts = message.Split(' ');
                foreach (string messagePart in messageParts)
                {
                    if (emojiExpression.IsMatch(messagePart))
                    {
                        returnMessage.Append($":{messagePart.Split(':')[1]}: ");
                    }
                    else if (userMentionExpression.IsMatch(messagePart))
                    {
                        ulong userId = 0;
                        ulong.TryParse(messagePart.Substring(1, messagePart.Length - 2), out userId);

                        if (userId == 0)
                        {
                            ulong.TryParse(messagePart.Substring(2, messagePart.Length - 3), out userId);
                        }
                        if (userId > 0)
                        {
                            var user = _client.GetUser(userId);
                            returnMessage.Append($"@{user.Username}#{user.Discriminator} ");
                        }
                        else
                        {
                            returnMessage.Append($"{messagePart} ");
                        }
                    }
                    else if (channelExpression.IsMatch(messagePart))
                    {
                        ulong channelID = 0;
                        ulong.TryParse(messagePart.Substring(1, messagePart.Length - 2), out channelID);

                        if (channelID == 0)
                        {
                            ulong.TryParse(messagePart.Substring(2, messagePart.Length - 3), out channelID);
                        }
                        if (channelID > 0)
                        {
                            var channel = _client.GetChannel(channelID);
                            returnMessage.Append($"#{channel} ");
                        }
                        else
                        {
                            returnMessage.Append($"{messagePart} ");
                        }
                    }
                    else
                    {
                        returnMessage.Append($"{messagePart} ");
                    }
                }
            }
            return returnMessage.Length > 0 ? returnMessage.ToString() : message;
        }
    }
}

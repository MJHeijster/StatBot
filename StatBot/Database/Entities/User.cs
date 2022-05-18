// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="User.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Database.Entities
{
    /// <summary>
    /// Class User.
    /// </summary>
    public class User
	{
        /// <summary>
        /// The identifier
        /// </summary>
        public ulong Id;
        /// <summary>
        /// The username
        /// </summary>
        public string Username;
        /// <summary>
        /// The discrim
        /// </summary>
        public string Discrim;
        /// <summary>
        /// The avatar URI
        /// </summary>
        public string AvatarUri;
        /// <summary>
        /// The is bot
        /// </summary>
        public bool IsBot;
        /// <summary>
        /// The is excluded from stats
        /// </summary>
        public bool IsExcludedFromStats;
        /// <summary>
        /// The old users
        /// </summary>
        public List<OldUser> OldUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="author">The author.</param>
        public User(SocketUser author)
        {
			Id = author.Id;
			Username = author.Username;
			Discrim = author.Discriminator;
			AvatarUri = author.GetAvatarUrl();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="discrim">The discrim.</param>
        /// <param name="avatarUri">The avatar URI.</param>
        /// <param name="isBot">The is bot.</param>
        /// <param name="isExcludedFromStats">The is excluded from stats.</param>
        /// <param name="oldUsers">The old users.</param>
        public User(object id, object username, object discrim, object avatarUri, object isBot, object isExcludedFromStats, List<OldUser> oldUsers = null)
        {
            Id = Convert.ToUInt64(id);
            Username = (string)username;
            Discrim = (string)discrim;
            AvatarUri = (string)avatarUri;
            IsBot = (Int64)isBot == 1;
            IsExcludedFromStats = (Int64)isExcludedFromStats == 1;
            OldUsers = oldUsers;
        }
    }
}

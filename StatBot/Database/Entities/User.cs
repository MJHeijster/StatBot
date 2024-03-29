﻿// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 05-06-2022
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
        /// The override name
        /// </summary>
        public string OverrideName;
        /// <summary>
        /// The old users
        /// </summary>
        public List<OldUser> OldUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
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
        public User()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="discrim">The discrim.</param>
        /// <param name="avatarUri">The avatar URI.</param>
        /// <param name="isBot">if set to <c>true</c> [is bot].</param>
        /// <param name="isExcludedFromStats">if set to <c>true</c> [is excluded from stats].</param>
        /// <param name="overrideName">Name of the override.</param>
        public User(string id, string username, string discrim, string avatarUri, bool isBot, bool isExcludedFromStats, string overrideName)
        {
            Id = Convert.ToUInt64(id);
            Username = username;
            Discrim = discrim;
            AvatarUri = avatarUri;
            IsBot = isBot;
            IsExcludedFromStats = isExcludedFromStats;
            OverrideName = overrideName == null ? OverrideName : null;
            OldUsers = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="discrim">The discrim.</param>
        /// <param name="avatarUri">The avatar URI.</param>
        /// <param name="isBot">The is bot.</param>
        /// <param name="isExcludedFromStats">The is excluded from stats.</param>
        /// <param name="overrideName">The name to use as an override.</param>
        /// <param name="oldUsers">The old users.</param>
        public User(object id, object username, object discrim, object avatarUri, object isBot, object isExcludedFromStats, object overrideName, List<OldUser> oldUsers = null)
        {
            Id = ConvertFromDBVal<ulong>(id);
            Username = ConvertFromDBVal<string>(username);
            Discrim = ConvertFromDBVal<string>(discrim);
            AvatarUri = ConvertFromDBVal<string>(avatarUri);
            IsBot = (Int64)isBot == 1;
            IsExcludedFromStats = (Int64)isExcludedFromStats == 1;
            OverrideName = ConvertFromDBVal<string>(overrideName);
            OldUsers = oldUsers;
        }
        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
    }
}

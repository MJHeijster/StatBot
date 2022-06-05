// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 17-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 05-06-2022
// ***********************************************************************
// <copyright file="UserHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Data.Sqlite;
using StatBot.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Database.DatabaseHandlers
{
    /// <summary>
    /// Class UserHandler.
    /// </summary>
    public static class UserHandler
    {
        /// <summary>
        /// The user cache
        /// </summary>
        private static List<User> userCache;
        /// <summary>
        /// Inserts the or update user.
        /// </summary>
        /// <param name="user">The user.</param>
        public static void InsertOrUpdateUser(User user)
        {
            if (userCache == null)
                userCache = GetUsers();
            if (!userCache.Contains(user))
            {
                string command = $"REPLACE INTO Users(Id, Username, Discrim, AvatarUri, IsBot) VALUES({user.Id},'{user.Username}','{user.Discrim}','{user.AvatarUri}',{user.IsBot})";
                using (var connection = new SqliteConnection("Data Source=Database\\Statbot.db;"))
                {
                    connection.Open();
                    using (var cmd = new SqliteCommand(command, connection))
                    {
                        cmd.ExecuteNonQuery();

                    }
                }
                userCache.Add(user);
            }
        }
        /// <summary>
        /// Excludes from stats.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="exclude">if set to <c>true</c> [exclude].</param>
        public static void ExcludeFromStats(ulong userId, bool exclude)
        {
            string command = $"UPDATE Users SET IsExcludedFromStats = {exclude} WHERE Id={userId}";
            using (var connection = new SqliteConnection("Data Source=Database\\Statbot.db;"))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(command, connection))
                {
                    cmd.ExecuteNonQuery();

                }
            }
        }
        /// <summary>
        /// Overrides the username.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="username">The username.</param>
        public static void OverrideUsername(ulong userId, string username)
        {
            string command = $"UPDATE Users SET OverrideName = '{username}' WHERE Id={userId}";
            using (var connection = new SqliteConnection("Data Source=Database\\Statbot.db;"))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(command, connection))
                {
                    cmd.ExecuteNonQuery();

                }
            }
        }

        /// <summary>
        /// Adds the old username.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="username">The username.</param>
        public static void AddOldUsername(ulong userId, string username)
        {
            var user = username.Split('#');
            var oldUsers = GetOldUsers(userId);
            if (!oldUsers.Any(c => c.UserName == user[0] && c.Discrim == user[1]))
            {
                string command = $"INSERT into OldUsers (Id, Username, Discrim, DateTimeChanged) " +
                    $"values ({userId},'{user[0]}','{user[1]}',DATETIME('now'))";
                using (var connection = new SqliteConnection("Data Source=Database\\Statbot.db;"))
                {
                    connection.Open();
                    using (var cmd = new SqliteCommand(command, connection))
                    {
                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }
        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="includingOld">if set to <c>true</c> [including old users].</param>
        /// <returns>List&lt;User&gt;.</returns>
        public static List<User> GetUsers(bool includingOld = false)
        {
            var users = new List<User>();
            string command = "SELECT * FROM Users;";
            using (var connection = new SqliteConnection("Data Source=Database\\Statbot.db;"))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(command, connection))
                {
                    using SqliteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var id = Convert.ToUInt64(rdr[0]);
                        List<OldUser> oldUsers = new List<OldUser>();
                        if (includingOld)
                        {
                            oldUsers = GetOldUsers(id);
                        }
                        users.Add(new User(id, rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], oldUsers));

                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Gets the old users.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List&lt;OldUser&gt;.</returns>
        private static List<OldUser> GetOldUsers(ulong id)
        {
            List<OldUser> oldUsers = new List<OldUser>();
            string commandOldUsers = $"SELECT * FROM OldUsers WHERE Id = {id};";
            using (var connection2 = new SqliteConnection("Data Source=Database\\Statbot.db;"))
            {
                connection2.Open();
                using (var cmd2 = new SqliteCommand(commandOldUsers, connection2))
                {
                    using SqliteDataReader rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        var v1 = rdr2[0];
                        var v2 = rdr2[1];
                        var v3 = rdr2[2];
                        oldUsers.Add(new OldUser(rdr2[0], rdr2[1], rdr2[2]));
                    }
                }
            }
            return oldUsers;
        }
    }
}

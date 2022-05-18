// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 17-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="DatabaseHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Database.DatabaseHandlers
{
    /// <summary>
    /// Class DatabaseHandler.
    /// </summary>
    internal class DatabaseHandler
    {
        /// <summary>
        /// Creates the database.
        /// </summary>
        internal static void CreateDatabase()
        {
            if (!File.Exists("Database\\Statbot.db"))
            {
                try
                {
                   new SqliteConnection("Data Source=Database\\Statbot.db");
                }
                catch { }
                using var con = new SqliteConnection("Data Source=Database\\Statbot.db");
                con.Open();
                var sqlScript = File.ReadAllText("Database\\Statbot.db.sql");
                using (var cmd = new SqliteCommand(sqlScript, con))
                {
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }
}

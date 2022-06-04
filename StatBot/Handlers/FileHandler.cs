// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 20-05-2022
// ***********************************************************************
// <copyright file="FileHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Discord.WebSocket;
using StatBot.Classes;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class FileHelper.
    /// </summary>
    public class FileHandler
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private static BotSettings _botSettings;
        /// <summary>
        /// The log files
        /// </summary>
        static List<LogFile> logFiles = new List<LogFile>();
        /// <summary>
        /// The logging file type
        /// </summary>
        static bool loggingFileType;
        /// <summary>
        /// The single log file
        /// </summary>
        static bool singleLogFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandler" /> class.
        /// </summary>
        /// <param name="botSettings">The bot settings.</param>
        public FileHandler(BotSettings botSettings)
        {
            _botSettings = botSettings;
            loggingFileType = _botSettings.Application.LoggingFileName == "channelname";
            singleLogFile = _botSettings.Application.LoggingFileName == "single";
        }

        /// <summary>
        /// Checks and gets file path.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The complete file path including the file.</returns>
        internal string CheckAndGetFilePath(SocketMessage message)
        {
            LogFile logFile = GetLogFile(message);
            string target = Directory.GetCurrentDirectory() + logFile.Folder;
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            return target + logFile.FileName;
        }

        /// <summary>
        /// Gets the log file settings. If it doesn't exist in the log file list yet, add it and write that the session is started.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The log file settings.</returns>
        internal LogFile GetLogFile(SocketMessage message)
        {

            var channel = message.Channel;
            var guild = (channel as SocketGuildChannel)?.Guild;
            LogFile logFile = logFiles.Find(x => x.Channel == channel && x.Guild == guild);
            if (logFile == null)
            {
                string fileName = string.Empty;
                if (singleLogFile)
                {
                    fileName = "server.log";
                }
                else
                {
                    if (loggingFileType)
                    {
                        fileName = channel.Name + ".log";
                    }
                    else
                    {
                        fileName = channel.Id + ".log";
                    }
                }
                logFile = new LogFile
                {
                    Channel = channel,
                    Guild = guild,
                    FileName = fileName,
                    Folder = "\\" + guild.Id + "\\"
                };
                logFiles.Add(logFile);
                string file = Directory.GetCurrentDirectory() + logFile.Folder + logFile.FileName;
                if (!Directory.Exists(Directory.GetCurrentDirectory() + logFile.Folder))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + logFile.Folder);
                }
                using (StreamWriter text = File.AppendText(file))
                {
                    text.WriteLine("");
                    text.WriteLine($"Session Start: at {DateTime.Now.ToString("MMM d HH:mm:ss yyyy")}");
                }
            }

            return logFile;
        }
    }
}

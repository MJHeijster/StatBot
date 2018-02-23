// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-02-2018
// ***********************************************************************
// <copyright file="FileHelper.cs" company="Jeroen Heijster">
//     Copyright ©  2017
// </copyright>
// ***********************************************************************
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;

namespace StatBot
{
    public static class FileHelper
    {
        /// <summary>
        /// The log files
        /// </summary>
        static List<LogFile> logFiles = new List<LogFile>();
        static bool loggingFileType = Bot.Default.LoggingFileName == "channelname";

        /// <summary>
        /// Checks and gets file path.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The complete file path including the file.</returns>
        internal static string CheckAndGetFilePath(SocketMessage message)
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
        internal static LogFile GetLogFile(SocketMessage message)
        {
            var channel = message.Channel;
            var guild = (channel as SocketGuildChannel)?.Guild;
            LogFile logFile = logFiles.Find(x => x.Channel == channel && x.Guild == guild);
            if (logFile == null)
            {
                string fileName = string.Empty;
                if (loggingFileType)
                {
                    fileName = channel.Name + ".log";
                }
                else
                {
                    fileName = channel.Id + ".log";
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

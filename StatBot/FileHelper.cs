using Discord.WebSocket;
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
        /// Gets the log file settings.
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
                logFile = new LogFile
                {
                    Channel = channel,
                    Guild = guild,
                    FileName = channel.Name + ".txt",
                    Folder = "\\" + guild.Id + "\\"
                };
                logFiles.Add(logFile);
            }

            return logFile;
        }
    }
}

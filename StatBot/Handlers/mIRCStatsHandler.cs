// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 15-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 25-05-2022
// ***********************************************************************
// <copyright file="mIRCStatsHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class mIRCStatsHandler.
    /// </summary>
    internal class mIRCStatsHandler
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private static BotSettings _botSettings;
        /// <summary>
        /// The folder
        /// </summary>
        private static string folder;
        /// <summary>
        /// The sleeping time
        /// </summary>
        private static int sleepingTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="mIRCStatsHandler" /> class.
        /// </summary>
        /// <param name="botSettings">The bot settings.</param>
        public mIRCStatsHandler(BotSettings botSettings)
        {
            _botSettings = botSettings;
        }
        /// <summary>
        /// Generate stats as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task GenerateStatsAsync()
        {
            if (_botSettings.mIRCStats.UseInternalTimer)
            {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(_botSettings.mIRCStats.GeneratorFile);
                folder = fileinfo.Directory.FullName;
                sleepingTime = _botSettings.mIRCStats.LaunchEveryMinutes * 60000; //convert from minutes to ms.
                while (true)
                    if (StatsGenerator().IsFaulted)
                        System.Threading.Thread.Sleep(1800000); // Sleep for half an hour if the process couldn't be started.
            }
            else if (_botSettings.Application.CreateNicksFileAutomatically)
            {
                while (true)
                {
                    GenerateNicksFile();
                    System.Threading.Thread.Sleep(1800000);
                }
            }
        }

        /// <summary>
        /// Generates the nicks file.
        /// </summary>
        private void GenerateNicksFile()
        {
            if (!string.IsNullOrEmpty(_botSettings.mIRCStats.NicksFile) && !Directory.Exists($"{_botSettings.mIRCStats.Path}"))
            {
                Directory.CreateDirectory($"{_botSettings.mIRCStats.Path}");
            }
            var users = Database.DatabaseHandlers.UserHandler.GetUsers(true);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[common]");
            foreach (var user in users)
            {
                if (!_botSettings.Application.ShowDiscrim || user.IsBot || user.IsExcludedFromStats || user.OldUsers.Any())
                {
                    sb.Append($"{user.Username.Replace(' ', '_')}#{user.Discrim}");
                    foreach (var olduser in user.OldUsers.Where(c => !string.IsNullOrEmpty(c.UserName) && !string.IsNullOrEmpty(c.Discrim) && !(c.UserName == user.Username && c.Discrim == user.Discrim)))
                    {
                        sb.Append("; ");
                        sb.Append($"{olduser.UserName.Replace(' ', '_')}#{olduser.Discrim}");
                    }
                    if (_botSettings.Application.ShowAvatar)
                    {
                        sb.Append("; ");
                        sb.Append($"IMAGE={user.AvatarUri.Replace("?size=128", "")}");
                    }
                    if (!string.IsNullOrEmpty(user.OverrideName))
                    {
                        sb.Append("; ");
                        sb.Append($"NAME={user.OverrideName.Replace(' ', '_')}");
                    }
                    else if (!_botSettings.Application.ShowDiscrim)
                    {
                        sb.Append("; ");
                        sb.Append($"NAME={user.Username.Replace(' ', '_')}");
                    }
                    if (user.IsExcludedFromStats)
                    {
                        sb.Append("; ");
                        sb.Append($"MODE=ISEXCLUDED");
                }
                    sb.AppendLine("");
                }
            }

            using (StreamWriter text = File.CreateText($"{_botSettings.mIRCStats.Path}\\{_botSettings.mIRCStats.NicksFile}"))
            {
                text.WriteLine(sb);
                if (File.Exists($"{_botSettings.mIRCStats.Path}\\{_botSettings.Application.NicksFileManual}"))
                {
                    text.WriteLine(File.ReadAllText($"{_botSettings.mIRCStats.Path}\\{_botSettings.Application.NicksFileManual}"));
                }
            }

        }

        /// <summary>
        /// Runs the statistics generator
        /// </summary>
        /// <returns>Task.</returns>
        private Task StatsGenerator()
        {
            System.Threading.Thread.Sleep(sleepingTime);
            try
            {
                if (_botSettings.Application.CreateNicksFileAutomatically)
                {
                    GenerateNicksFile();
                }
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.FileName = _botSettings.mIRCStats.GeneratorFile;
                pInfo.WorkingDirectory = folder;
                pInfo.WindowStyle = ProcessWindowStyle.Maximized;
                //Start the process.
                Process p = Process.Start(pInfo);
                Console.WriteLine("");

                if (_botSettings.mIRCStats.WaitUntilCompleted)
                {
                    //Wait for the process to end.
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("mIRCStats couldn't launch with GeneratorFile. Exception: " + ex.Message);
                return Task.FromException(ex);
            }
            return Task.CompletedTask;
        }
    }
}

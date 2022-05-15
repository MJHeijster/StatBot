// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 15-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 15-05-2022
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
        /// Initializes a new instance of the <see cref="mIRCStatsHandler"/> class.
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
            if (!string.IsNullOrEmpty(_botSettings.mIRCStats.GeneratorFile))
            {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(_botSettings.mIRCStats.GeneratorFile);
                folder = fileinfo.Directory.FullName;
                sleepingTime = _botSettings.mIRCStats.LaunchEveryMinutes * 60000; //convert from minutes to ms.
                while (true)
                    if (StatsGenerator().IsFaulted)
                        System.Threading.Thread.Sleep(1800000); // Sleep for half an hour if the process couldn't be started.
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

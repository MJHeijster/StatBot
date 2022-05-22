// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 15-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 15-05-2022
// ***********************************************************************
// <copyright file="mIRCStatsGenerator.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Configuration;
using StatBot.Handlers;
using StatBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot
{
    /// <summary>
    /// Class mIRCStatsGenerator.
    /// </summary>
    internal class mIRCStatsGenerator
    {
        /// <summary>
        /// The bot settings
        /// </summary>
        private BotSettings _botSettings;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public mIRCStatsGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        public void DoWork()
        {
            _botSettings = new BotSettings(_configuration);
            mIRCStatsHandler mircStatsHandler = new mIRCStatsHandler(_botSettings);
            _ = mircStatsHandler.GenerateStatsAsync();
        }
    }
}
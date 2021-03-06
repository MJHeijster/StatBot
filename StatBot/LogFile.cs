﻿// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 11-12-2017
// ***********************************************************************
// <copyright file="LogFile.cs" company="Jeroen Heijster">
//     Copyright ©  2017
// </copyright>
// ***********************************************************************
using Discord;

namespace StatBot
{
    public class LogFile
    {
        /// <summary>
        /// Gets or sets the guild (server).
        /// </summary>
        /// <value>The guild (server).</value>
        public IGuild Guild { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public IChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets the folder structure.
        /// </summary>
        /// <value>The folder structure.</value>
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }
    }
}

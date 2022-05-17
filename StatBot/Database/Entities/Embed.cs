// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="Embed.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Database.Entities
{
    /// <summary>
    /// Class Embed.
    /// </summary>
    public class Embed
	{
        /// <summary>
        /// The identifier
        /// </summary>
        public long Id;
        /// <summary>
        /// The message identifier
        /// </summary>
        public long MessageId;
        /// <summary>
        /// The URI
        /// </summary>
        public string Uri;
        /// <summary>
        /// The embed type
        /// </summary>
        public long EmbedType;
        /// <summary>
        /// The thumbnail
        /// </summary>
        public string Thumbnail;
	}
}

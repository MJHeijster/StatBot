// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 18-05-2022
// ***********************************************************************
// <copyright file="Channel.cs">
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
    /// Class Channel.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public long Id;
        /// <summary>
        /// The name
        /// </summary>
        public string Name;
        /// <summary>
        /// The server identifier
        /// </summary>
        public long ServerId;
    }
}

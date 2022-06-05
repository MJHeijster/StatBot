// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 18-05-2022
// ***********************************************************************
// <copyright file="Message.cs">
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
    /// Class Message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public long Id;
        /// <summary>
        /// The text
        /// </summary>
        public string Text;
        /// <summary>
        /// The user
        /// </summary>
        public User User;
    
    }
}

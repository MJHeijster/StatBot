// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 17-05-2022
// ***********************************************************************
// <copyright file="EmojiUsed.cs">
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
    /// Class EmojiUsed.
    /// </summary>
    public class EmojiUsed
    {
        /// <summary>
        /// The message identifier
        /// </summary>
        public long MessageId;
        /// <summary>
        /// The emoji identifier
        /// </summary>
        public long EmojiId;
    }
}

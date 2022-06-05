// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 16-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 18-05-2022
// ***********************************************************************
// <copyright file="Attachment.cs">
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
    /// Class Attachment.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public long Id;
        /// <summary>
        /// The filename
        /// </summary>
        public long Filename;
        /// <summary>
        /// The URI
        /// </summary>
        public long Uri;
        /// <summary>
        /// The message identifier
        /// </summary>
        public long MessageId;
    }
}

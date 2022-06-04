// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 17-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 18-05-2022
// ***********************************************************************
// <copyright file="OldUser.cs">
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
    /// Class OldUser.
    /// </summary>
    public class OldUser
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public ulong Id;
        /// <summary>
        /// The user name
        /// </summary>
        public string UserName;
        /// <summary>
        /// The discrim
        /// </summary>
        public string Discrim;

        /// <summary>
        /// Initializes a new instance of the <see cref="OldUser" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="discrim">The discrim.</param>
        public OldUser(object id, object userName, object discrim)
        {
            Id = Convert.ToUInt64(id);
            UserName = (string)userName;
            Discrim = (string)discrim;
        }
    }
}

// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-05-2022
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 16-05-2022
// ***********************************************************************
// <copyright file="PushoverMessageHandler.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Net;
using System.Net.Http;

namespace StatBot.Handlers
{
    /// <summary>
    /// Class PushoverMessageHandler.
    /// </summary>
    internal class PushoverMessageHandler
    {
        /// <summary>
        /// The pushover API
        /// </summary>
        private string PushoverApi;
        /// <summary>
        /// The pushover user key
        /// </summary>
        private string PushoverUserKey;
        /// <summary>
        /// Initializes a new instance of the <see cref="PushoverMessageHandler" /> class.
        /// </summary>
        /// <param name="pushoverApi">The pushover API.</param>
        /// <param name="pushoverUserKey">The pushover user key.</param>
        public PushoverMessageHandler(string pushoverApi, string pushoverUserKey)
        {
            PushoverApi = pushoverApi;
            PushoverUserKey = pushoverUserKey;
        }
        /// <summary>
        /// Pushes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async void PushMessage(string message)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(PushoverApi), "\"token\"");
                form.Add(new StringContent(PushoverUserKey), "\"user\"");
                form.Add(new StringContent(message), "\"message\"");
                // Remove content type that is not in the docs
                foreach (var param in form)
                    param.Headers.ContentType = null;

                await httpClient.PostAsync("https://api.pushover.net/1/messages.json", form);

            }
        }

    }
}

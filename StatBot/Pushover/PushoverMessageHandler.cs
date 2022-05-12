using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StatBot.Pushover
{
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
        /// Initializes a new instance of the <see cref="PushoverMessageHandler"/> class.
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
        public void PushMessage(string message)
        {
            var parameters = new NameValueCollection {
            { "token", PushoverApi },
            { "user", PushoverUserKey },
            { "message", message }
};

            using (var client = new WebClient())
            {
                client.UploadValues("https://api.pushover.net/1/messages.json", parameters);
            }
        }
        
    }
}

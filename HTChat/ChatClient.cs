using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using jabber;

namespace HTChat
{
    class ChatClient
    {
        public jabber.client.JabberClient Client { get; private set; }

        public ChatClient()
        {
            Client = new jabber.client.JabberClient();
            Client.OnInvalidCertificate += OnInvalidCertificate;
        }

        private bool OnInvalidCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

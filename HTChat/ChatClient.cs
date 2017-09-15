using HTChat.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace HTChat
{
    class ChatClient: INotifyPropertyChanged
    {
        XmppClientConnection _xmppClient;
        private bool _loginFlag;

        public XmppClientConnection XmppClient => _xmppClient;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Host { get; set; }
        public int Port { get; set; } = 5222;
        public bool UseTLS { get; set; } = true;

        public bool IsLoggedOn { get; private set; }
        public Contact CurrentUser { get; private set; }
        public string LastErrorMessage { get; private set; }


        public ChatClient()
        {
            
        }

        public async Task<bool> Login(string username, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    IsLoggedOn = false;
                    _xmppClient = new XmppClientConnection(Host);
                    _xmppClient.AutoResolveConnectServer = false;
                    _xmppClient.UseCompression = false;
                    _xmppClient.Resource = "Spark";
                    _xmppClient.ConnectServer = Host;
                    _xmppClient.OnReadXml += OnReadXml;
                    _xmppClient.OnWriteXml += OnWriteXml;
                    _xmppClient.OnAuthError += OnAuthError;
                    _xmppClient.OnError += OnError;
                    _xmppClient.OnLogin += OnLogin;
                    _xmppClient.OnIq += OnIq;
                    _xmppClient.OnPresence += OnPresence;

                    _xmppClient.AutoPresence = true;
                    _xmppClient.AutoRoster = false;
                    _xmppClient.AutoAgents = false;
                    _xmppClient.Resource = "Spark";
                    
                    _xmppClient.Open(username, password);
                    


                    var time = DateTime.Now;
                    _loginFlag = false;
                    while(!_loginFlag)
                    {
                        if(DateTime.Now - time > TimeSpan.FromSeconds(20))
                        {
                            LastErrorMessage = "Timeout occured.";
                            return false;
                        }
                        Task.Delay(100);
                    }
                    
                    IsLoggedOn = true;
                    SendPresence();

                    //_xmppClient.Send(new Message("Sheila Mae Degamo@openfire.alliance.com.ph", MessageType.chat, "Muhahahaha it works!"));
                    return true;
                }
                catch (Exception ex)
                {
                    LastErrorMessage = "XML error";
                    return false;
                }
            });
        }

        internal void SendMessage(Message msg)
        {
            _xmppClient.Send(msg);
            throw new NotImplementedException();
        }

        public void SendPresence()
        {
            var presence = new Presence(ShowType.chat, "Online (HTCHAT)");
            presence.Type = PresenceType.available;
            //_xmppClient.SendMyPresence();
            _xmppClient.Send(presence);
        }

        private void OnPresence(object sender, Presence pres)
        {

        }

        private void OnIq(object sender, IQ iq)
        {
        }

        private void OnAuthError(object sender, Element e)
        {
            Debug.WriteLine("Auth error");
            Debug.WriteLine(e.ToString());
        }

        private void OnLogin(object sender)
        {
            _loginFlag = true;
        }

        private void OnError(object sender, Exception ex)
        {
            
        }

        private void OnWriteXml(object sender, string xml)
        {
            Debug.Print("Send XML");
            Debug.Print(xml);
        }

        private void OnReadXml(object sender, string xml)
        {
            Debug.Print("Receive XML");
            Debug.Print(xml);
        }
        
        public async Task Logout()
        {
            //if (!IsLoggedOn) return;
            //await new Task(() => {
            //   _xmppClient.
            //});
        }

        private bool OnCertValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        
    }
}

using agsXMPP;
using HTChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.disco;
using System.IO;
using agsXMPP.protocol.iq.vcard;
using System.Diagnostics;

namespace HTChat
{
    public class ChatClient
    {
        public Jid MyJid => XmppClient.MyServerJID;
        public Contact MyContactInfo { get; private set; }
        public ObservableCollection<Chat> Chats { get; private set; } = new ObservableCollection<Chat>();
        public ObservableCollection<Contact> Contacts { get; private set; } = new ObservableCollection<Contact>();
        public ObservableCollection<Room> ChatRooms { get; private set; } = new ObservableCollection<Room>();

        private readonly Dictionary<Jid, Contact> _jidToContactMap = new Dictionary<Jid, Contact>();
        private XmppClientConnection _xmppClient;
        private bool _loginFlag;
        private string _presence;

        public XmppClientConnection XmppClient => _xmppClient;

        public static ChatClient Instance { get; internal set; }
        public DiscoManager DiscoManager { get; private set; }

        public bool IsLoggedOn { get; private set; } = false;
        public string Host { get; private set; }
        public string LastErrorMessage { get; private set; }
        public string Presence
        {
            get => _presence;
            set
            {
                _presence = value;
                SendPresence(_presence);
            }
        }

        public ChatClient()
        {
            Instance = this;
        }

        public async Task<bool> Login(string host, string username, string password)
        {
            try
            {
                IsLoggedOn = false;
                Host = host;
                _xmppClient = new XmppClientConnection(Host);

                _xmppClient.ConnectServer = Host;
                _xmppClient.OnReadXml += OnReadXml;
                _xmppClient.OnWriteXml += OnWriteXml;
                _xmppClient.OnAuthError += OnAuthError;
                _xmppClient.OnError += OnError;
                _xmppClient.OnLogin += OnLogin;
                _xmppClient.OnIq += OnIq;
                _xmppClient.OnPresence += OnPresence;
                _xmppClient.OnMessage += OnMessage;

                _xmppClient.AutoPresence = true;
                _xmppClient.AutoRoster = false;
                _xmppClient.AutoAgents = false;
                _xmppClient.AutoResolveConnectServer = false;
                _xmppClient.UseCompression = false;
                _xmppClient.Resource = "Spark";
                _xmppClient.Open(username, password);

                DiscoManager = new DiscoManager(_xmppClient);

                var time = DateTime.Now;
                _loginFlag = false;
                while (!_loginFlag)
                {
                    if (DateTime.Now - time > TimeSpan.FromMilliseconds(App.Timeout * 2))
                    {
                        LastErrorMessage = "Timeout occured.";
                        return false;
                    }
                    await Task.Delay(500);
                }

                IsLoggedOn = true;
                SetDiscoInfo();
                Presence = "Online";
                Dispatch.InvokeUI(()=>MyContactInfo = new Contact(XmppClient.MyServerJID));
                await GetVcard(MyContactInfo);

                return true;
            }
            catch (Exception)
            {
                LastErrorMessage = "XML error";
                return false;
            }
        }

        private void OnPresence(object sender, Presence pres)
        {
            Dispatch.InvokeUI(async () => {
                if (_jidToContactMap.TryGetValue(pres.From, out var contact))
                {
                    contact.Status = pres.Status;
                }
                else
                {
                    contact = new Contact { Jid = pres.From, Status = pres.Status };
                    if (pres.From.Bare == XmppClient.MyServerJID.Bare)
                    {
                        return;
                    }

                    _jidToContactMap[pres.From] = contact;

                    int index;
                    for (index = 0; index < Contacts.Count; index++)
                    {
                        if (String.Compare(Contacts[index].Jid.User, contact.Jid.User) > 0)
                            break;
                    }
                    Contacts.Insert(index, contact);
                    await GetVcard(contact);
                }
            });
        }

        private void OnMessage(object sender, Message msg)
        {
            Dispatch.InvokeUI(async () =>
            {
                var chat = Chats.FirstOrDefault(c => c.TheirJid.Bare == msg.From.Bare);
                if (chat == null)
                {
                    chat = new SingleChat(this, msg.From);
                    await chat.OnMessage(msg);
                    Chats.Add(chat);
                }
                this.FireEvent(new Events.MessageReceivedEvent(message: msg, chat: chat));
            });
        }

        private void SetDiscoInfo()
        {
            _xmppClient.DiscoInfo.AddIdentity(new DiscoIdentity("pc", "HTChat", "client"));
            _xmppClient.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_INFO));
            _xmppClient.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_ITEMS));
            //_xmppClient.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.BYTESTREAMS));
            //_xmppClient.DiscoInfo.AddFeature(new agsXMPP.protocol.iq.disco.DiscoFeature(agsXMPP.Uri.IBB));
            _xmppClient.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.CHATSTATES));
            //_xmppClient.DiscoInfo.AddFeature(new agsXMPP.protocol.iq.disco.DiscoFeature(agsXMPP.Uri.MUC));
        }

        public async Task<bool> GetVcard(Contact contact)
        {
            return await Task.Run(() =>
            {
                var result = XmppClient.IqGrabber.SendIq(new VcardIq(IqType.get, contact.Jid.Bare), App.Timeout);
                if (result != null)
                {
                    if (result.Type == IqType.result)
                    {
                        Dispatch.InvokeUI(() =>
                        {
                            contact.Vcard = result.Vcard;
                            //contact.Photo = iq.Vcard.Photo;

                            if (result.Vcard?.Photo?.Image != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    result.Vcard.Photo.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                    ms.Seek(0, SeekOrigin.Begin);

                                    var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                                    bitmapImage.BeginInit();
                                    bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                                    bitmapImage.StreamSource = ms;
                                    bitmapImage.EndInit();

                                    contact.Avatar = bitmapImage;
                                }
                            }
                        });
                        return true;
                    }
                }
                return false;
            });
        }

        public void SendPresence(string presenceStr)
        {
            var presence = new Presence(ShowType.chat, presenceStr);
            presence.Type = PresenceType.available;
            //_xmppClient.SendMyPresence();
            _xmppClient.Send(presence);
        }

        private void OnIq(object sender, IQ iq)
        {
            //throw new NotImplementedException();
        }

        private void OnLogin(object sender)
        {
            _loginFlag = true;
        }

        private void OnError(object sender, Exception ex)
        {
            //throw new NotImplementedException();
        }

        private void OnWriteXml(object sender, string xml)
        {
            //Debug.Print("SEND: " + xml);
        }

        private void OnReadXml(object sender, string xml)
        {
            //Debug.Print("RECEIVE: " + xml);
        }

        private void OnAuthError(object sender, Element e)
        {
            //throw new NotImplementedException();
        }

        public Contact JidToContact(Jid jid)
        {
            if (!_jidToContactMap.TryGetValue(jid, out var contact))
            {
                contact = new Contact(jid);
            }
            return contact;
        }
    }
}

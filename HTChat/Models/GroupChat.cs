using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.x.muc;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HTChat.Models
{
    public class GroupChatMember: INotifyPropertyChanged
    {
        public string Nick { get; set; }
        public Jid UserJid { get; set; }
        public Jid Jid { get; set; }
        public string Status { get; set; }
        public string Affiliation { get; set; }
        public string Role { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class GroupChat : Chat
    {
        public ObservableCollection<GroupChatMember> Members { get; private set; } = new ObservableCollection<GroupChatMember>();
        public GroupChat(ChatClient client, Jid roomJid) : base(client, roomJid)
        {
            Client.XmppClient.PresenceGrabber.Add(roomJid, new BareJidComparer(), (s, presence, d) => Dispatch.InvokeUI(() => OnPresence(presence)), null);
        }

        protected virtual void OnPresence(Presence presence)
        {
            var member = Members.FirstOrDefault(m => m.Jid == presence.From);
            if(member != null)
            {
                if (presence.Type == PresenceType.unavailable)
                {
                    Members.Remove(member);
                }
                else
                {
                    member.Status = presence.Status;
                    var user = presence.SelectSingleElement<User>();
                    if(user != null)
                    {
                        member.UserJid = user.Item.Jid;
                        member.Affiliation = user.Item.Affiliation.ToString();
                        member.Role = user.Item.Role.ToString();
                    }
                }
            }
            else
            {
                member = new GroupChatMember();
                member.Jid = presence.From;
                member.Nick = presence.From.Resource;
                var user = presence.SelectSingleElement<User>();
                if (user != null)
                {
                    member.UserJid = user.Item.Jid;
                    member.Affiliation = user.Item.Affiliation.ToString();
                    member.Role = user.Item.Role.ToString();
                }
                Members.Add(member);
            }            
        }

        public override async Task ShareFile(string path)
        {
            await Task.Delay(0);
        }
    }
}

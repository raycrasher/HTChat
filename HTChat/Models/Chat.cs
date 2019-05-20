using agsXMPP;
using agsXMPP.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP.protocol.client;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HTChat.Models
{
    public abstract class Chat: INotifyPropertyChanged
    {
        public Jid MyJid { get; set; }
        public Jid TheirJid => Contact.Jid;
        public FlowDocument Document { get; private set; } = new FlowDocument();
        public Contact Contact { get; set; }
        
        public int NumUnreadMessages { get; set; }
        public ChatClient Client { get; private set; }

        public List<(Block block, IChatItem item)> Items { get; private set; } = new List<(Block block, IChatItem item)>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<BitmapSource> BitmapsToBeSent { get; } = new ObservableCollection<BitmapSource>();

        public Chat() {
            if (!Utilities.IsDesignMode)
                throw new InvalidOperationException("Chat cannot be instantiated using parameterless constructor outside design mode.");
        }

        private void DisplayPreviousChats()
        {
            foreach (var msg in ChatLogger.Instance.GetPreviousChats(MyJid, TheirJid, 0, 100).Reverse())
            {
                var item = new StoredMessageChatItem { Author = msg.from, Body = msg.message, Timestamp = msg.timestamp };
                var block = item.RenderBlock();
                Items.Add((block, item));
                Document.Blocks.Add(block);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
        }

        protected Chat(ChatClient client, Jid theirJid)
        {
            Contact = client.JidToContact(theirJid);
            Client = client;
            MyJid = client.MyJid;
            //TheirJid = theirJid;
            Client.XmppClient.MessageGrabber.Add(TheirJid, new BareJidComparer(), (s, m, d) => Dispatch.InvokeUI(async () => await OnMessage(m)), null);
            DisplayPreviousChats();
        }

        public void SendBitmap(BitmapSource bmpSource)
        {
            BitmapsToBeSent.Add(bmpSource);
        }

        public virtual async Task<Message> SendMessage(string message)
        {
            return await Task.Run(() =>
            {
                var msg = new Message(to: TheirJid, body: message, type: MessageType.chat);
                Client.XmppClient.Send(msg);
                return msg;
            });
        }

        public async virtual Task OnMessage(Message msg)
        {
            if (!string.IsNullOrEmpty(msg.Body))
            {
                var chatItem = new MessageChatItem(msg);
                var block = chatItem.RenderBlock();
                Items.Add((block, chatItem));
                Document.Blocks.Add(block);
                await ChatLogger.Instance.LogMessage(msg.From, msg.To, DateTime.Now, msg.Body);
            }
        }

        public abstract Task ShareFile(string path);
    }
}

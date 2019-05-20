using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;

namespace HTChat.Models
{
    class SingleChat : Chat
    {
        public SingleChat() : base() { }
        public SingleChat(ChatClient client, Jid theirJid) : base(client, theirJid)
        {
        }

        public override async Task<Message> SendMessage(string message)
        {
            var msg = await base.SendMessage(message);
            var chatItem = new MessageChatItem(msg, MyJid);
            var block = chatItem.RenderBlock();
            Document.Blocks.Add(block);
            Items.Add((block, chatItem));
            if(!string.IsNullOrEmpty(message))
                await ChatLogger.Instance.LogMessage(MyJid, TheirJid, DateTime.Now, message);
            return msg;
        }

        public override async Task ShareFile(string path)
        {
            await Task.Delay(0);
            //(var result, var link) = await FtpFileSharer.ShareFile(TheirJid, path);
            //if (result)
            //{
            //    SendMessage(link);
            //}
        }
    }
}

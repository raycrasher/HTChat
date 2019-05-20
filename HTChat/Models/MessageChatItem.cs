using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using agsXMPP.protocol.client;

namespace HTChat.Models
{
    class StoredMessageChatItem : IChatItem
    {
        public Jid Author { get; set; }
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }

        public Block RenderBlock()
        {
            return ChatRenderer.RenderChatMessage(Author, Body, Timestamp);
        }
    }

    class MessageChatItem : IChatItem
    {

        public MessageChatItem(Message msg, Jid author = null)
        {
            Message = msg;
            Author = author ?? msg.From;
        }

        public Jid Author { get; set; }
        public Jid To => Message.To;
        public DateTime Timestamp { get; set; }
        public string Body => Message.Body;
        public Message Message { get; set; }

        public Block RenderBlock()
        {
            return ChatRenderer.RenderChatMessage(Author, Body, Timestamp);
        }
    }
}

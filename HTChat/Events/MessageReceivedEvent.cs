using agsXMPP.protocol.client;
using HTChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Events
{
    class MessageReceivedEvent: EventArgs
    {
        public MessageReceivedEvent(Message message, Chat chat)
        {
            Message = message;
            Chat = chat;
        }

        public Message Message { get; set; }
        public Chat Chat { get; set; }
    }
}

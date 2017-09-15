using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using HTChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HTChat.ViewModels
{
    class SingleChatViewModel: ChatViewModel
    {
        public Contact Contact { get; private set; }

        public SingleChatViewModel()
        {
            if (IsInDesignMode)
            {
                Contact = new Contact { Jid = "George@test.com", Status = "Online" };
            }
            else throw new InvalidOperationException("Cannot instantiate SingleChatViewModel without parameter outside design time.");
        }

        public SingleChatViewModel(Contact contact)
        {
            Contact = contact ?? throw new ArgumentNullException("contact");
        }

        public override void SendMessage(FlowDocument document)
        {
            var client = SimpleIoc.Default.GetInstance<ChatClient>();
            List<Message> messagesSent = new List<Message>();
            foreach(var block in document.Blocks.ToArray())
            {
                document.Blocks.Remove(block);

                switch (block)
                {
                    case Paragraph p:
                        StringBuilder sb = new StringBuilder();
                        foreach(var inl in p.Inlines)
                        {
                            switch (inl)
                            {
                                case Run run:
                                    sb.Append(run.Text);
                                    break;
                                case LineBreak lb:
                                    sb.Append(Environment.NewLine);
                                    break;
                                case InlineUIContainer iui:

                                    if(sb.Length > 0)
                                    {
                                        messagesSent.Add(new Message(to: Contact.Jid, type: MessageType.chat, body: sb.ToString()));
                                        sb.Clear();
                                    }

                                    break;
                            }
                        }
                        if (sb.Length > 0)
                        {
                            messagesSent.Add(new Message(to: Contact.Jid, type: MessageType.chat, body: sb.ToString()));
                            sb.Clear();
                        }
                        break;
                    
                }

                ChatDocument.Blocks.Add(block);
            }

            foreach(var msg in messagesSent)
            {
                client.SendMessage(msg);
            }
        }
    }
}

using HTChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTChat.Events;
using System.Windows.Input;

namespace HTChat.ViewModels
{
    class MainViewModel : INotifyPropertyChanged, IEventHandler<Events.MessageReceivedEvent>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Chat ActiveChat { get; set; }

        public ObservableCollection<Chat> Chats { get; private set; } = new ObservableCollection<Chat>();
        public ObservableCollection<Contact> Contacts { get; private set; } = new ObservableCollection<Contact>();
        public ChatClient Client { get; private set; }

        public MainViewModel()
        {
            if (Utilities.IsDesignMode)
            {
                var contact1 = new Contact("test@example.com") { Status = "Test status" };
                var contact2 = new Contact("test_2@example2.com/Resource");
                Chats.Add(new SingleChat() { NumUnreadMessages = 0, Contact = contact1 });
                Chats.Add(new SingleChat() { NumUnreadMessages = 10, Contact = contact2 });
                Contacts.Add(contact1);
                Contacts.Add(contact2);
                
            }
            else
            {
                this.RegisterEventHandler();
                Client = ChatClient.Instance;
                Contacts = Client.Contacts;
                Chats = Client.Chats;
            }
        }

        public void HandleEvent(object from, MessageReceivedEvent args)
        {
            if (!string.IsNullOrEmpty(args.Message.Body))
            {
                if (ActiveChat == null)
                {
                    ActiveChat = args.Chat;
                    if (!Chats.Contains(args.Chat))
                        Chats.Add(args.Chat);
                    args.Chat.NumUnreadMessages++;
                }
                else if (args.Chat != ActiveChat)
                {
                    if (!Chats.Contains(args.Chat))
                        Chats.Add(args.Chat);
                    args.Chat.NumUnreadMessages++;
                }
            }
        }

        public ICommand ShowChat => new DelegateCommand(o =>
        {
            ActiveChat = (Chat)o;
            ActiveChat.NumUnreadMessages = 0;
        });
        public ICommand StartChatWithContact => new DelegateCommand(o =>
        {
            var contact = (Contact)o;
            var chat = Chats.FirstOrDefault(c => c.TheirJid.Bare == contact.Jid.Bare);
            if (chat != null)
            {
                ActiveChat = chat;
                ActiveChat.NumUnreadMessages = 0;
            }
            else
            {
                chat = new SingleChat(Client, ((Contact)o).Jid);
                Chats.Add(chat);
                ActiveChat = chat;
            }
        });
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTChat.Events;

namespace HTChat.ViewModels
{
    public enum MainWindowState { Login, Main, Options }

    class MainWindowModel : INotifyPropertyChanged, IEventHandler<LoginEvent>, IEventHandler<MessageReceivedEvent>
    {
        public MainWindowState State { get; set; } = MainWindowState.Login;
        public ChatClient Client { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowModel()
        {
            this.RegisterEventHandler<LoginEvent>();
            this.RegisterEventHandler<MessageReceivedEvent>();
        }

        public void HandleEvent(object from, LoginEvent args)
        {
            Client = args.Client;
            State = MainWindowState.Main;
        }

        public void HandleEvent(object from, MessageReceivedEvent args)
        {
            
        }
    }
}

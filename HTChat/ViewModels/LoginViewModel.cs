using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HTChat.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool LoginEnabled { get; set; } = true;
        public bool RememberLogin { get; set; } = Properties.Settings.Default.RememberLogin;
        public string Username { get; set; }
        public string Host { get; set; } = Properties.Settings.Default.LastHost;
        public string ErrorMessage { get; private set; }

        public LoginViewModel()
        {
            Client = new ChatClient();
        }

        public ICommand DoLogin => new DelegateCommand(async o => {
            
            LoginEnabled = false;
            ErrorMessage = "Logging In...";
            var result = await Client.Login(Host, Username, ((PasswordBox)o).Password);
            if (result)
            {
                Properties.Settings.Default.RememberLogin = RememberLogin;
                if (RememberLogin)
                {
                    Properties.Settings.Default.LastHost = Host;
                    CredentialManager.WriteCredential(Assembly.GetExecutingAssembly().GetName().Name, Username, ((PasswordBox)o).Password);
                }
                Properties.Settings.Default.Save();
                
                this.FireEvent(new Events.LoginEvent { Client = Client });
            }
            else
            {
                ErrorMessage = "Login failed.";
                LoginEnabled = true;
            }
        });

        public ChatClient Client { get; private set; }
    }
}

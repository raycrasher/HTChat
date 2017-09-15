using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HTChat.ViewModels
{
    class LoginViewModel: ViewModelBase, INotifyPropertyChanged
    {
        public string Title => "HTChat > Login";
        public string Username { get; set; } = Properties.Settings.Default.LastUsername;
        public string Domain { get; set; } = Properties.Settings.Default.LastHost;
        public bool RememberLogin { get; set; }

        public ICommand CmdLogin => new AsyncDelegateCommand(
            async o =>
            {
                await Login(((PasswordBox)o).Password);
            });

        public string ErrorMessage { get; set; }
        
        public async Task Login(string password)
        {
            Properties.Settings.Default.LastUsername = Username;
            Properties.Settings.Default.LastHost = Domain;
            Properties.Settings.Default.LastPass = password;
            Properties.Settings.Default.Save();

            ErrorMessage = "Logging in...";
            var client = SimpleIoc.Default.GetInstance<ChatClient>();
            client.Host = Domain;
            var result = await client.Login(Username, password);

            if (result)
            {
                ViewModelLocator.Navigate<MainViewModel>();
            }
            else
            {
                ErrorMessage = "Login failed: " + client.LastErrorMessage;
            }
            
        }
    }
}

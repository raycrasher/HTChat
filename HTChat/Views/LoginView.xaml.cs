using HTChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HTChat.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            var cred = CredentialManager.ReadCredential(Assembly.GetExecutingAssembly().GetName().Name);
            if (cred != null)
            {
                ((LoginViewModel)FindResource("Model")).Username = cred.UserName;
                //uName.Text = cred.UserName;
                pwBox.Password = cred.Password;
            }
        }
    }
}

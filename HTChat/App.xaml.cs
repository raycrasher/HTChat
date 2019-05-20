using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HTChat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ChatClient CurrentSession { get; private set; }
        public static int Timeout { get; set; } = 20000;
        public static string ReceivedFilesFolder { get; internal set; }

        public App()
        {
            Utilities.Init();
        }
    }
}

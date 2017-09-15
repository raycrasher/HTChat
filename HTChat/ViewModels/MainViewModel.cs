using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        public static MainViewModel Instance { get; private set; }
        public ChatViewModel ActiveChatView { get; set; }
        public ViewModelBase RightPopupView { get; set; }

        public MainViewModel()
        {
            if (Instance != null)
                throw new NotSupportedException("Only one MainViewModel is allowed.");
            Instance = this;
        }
    }
}

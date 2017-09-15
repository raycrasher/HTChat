using agsXMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HTChat.Models
{
    public class Contact: INotifyPropertyChanged
    {
        public Jid Jid { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using agsXMPP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Models
{
    public class Room : INotifyPropertyChanged
    {
        public Jid Jid { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Jid> Members { get; set; } = new ObservableCollection<Jid>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

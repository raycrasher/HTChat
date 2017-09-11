using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HTChat.Controls
{
    public class ChatControlModel : INotifyPropertyChanged
    {
        public List<ChatItem> ChatItems { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

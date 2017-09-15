using GalaSoft.MvvmLight;
using System.Windows.Documents;

namespace HTChat.ViewModels
{
    public abstract class ChatViewModel: ViewModelBase
    {
        public FlowDocument ChatDocument { get; internal set; }
        public FlowDocument InputDocument { get; internal set; }

        public abstract void SendMessage(FlowDocument document);
    }
}
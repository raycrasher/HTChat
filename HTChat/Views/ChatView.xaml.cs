using HTChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        private ChatViewModel _model;

        public ChatView()
        {
            InitializeComponent();
            _model = (ChatViewModel)DataContext;
            ChatBox.Document = new FlowDocument();
            _model.ChatDocument = ChatBox.Document;
        }
        
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                _model.SendMessage(InputBox.Document);
                e.Handled = true;
                ChatBox.ScrollToEnd();
            }
        }
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            foreach (var fmt in e.Data.GetFormats())
            {
                ChatBox.Document.Blocks.Add(new Paragraph(new Run(fmt)));
            }
        }

        private void Grid_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}

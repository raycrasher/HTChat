using HTChat.Models;
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
    /// Interaction logic for SingleChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        private Chat _model;

        public ChatView()
        {
            InitializeComponent();
            //_model = (Chat)FindResource("Model");
            DataObject.AddPastingHandler(ChatInputBox, OnPaste);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // if is bitmap
            if (e.SourceDataObject.GetDataPresent(DataFormats.Bitmap, true))
            {
                var bitmap = e.SourceDataObject.GetData(DataFormats.Dib, true) as System.Drawing.Bitmap;
                IntPtr hBitmap = bitmap.GetHbitmap();
                try
                {
                    var bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    _model.SendBitmap(bmpSource);
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _model = DataContext as Chat;
            if (_model != null)
            {
                if (_model.Document.Parent != null)
                    ((RichTextBox)_model.Document.Parent).Document = new FlowDocument();
                this.ChatMessagesBox.Document = _model.Document;
            }
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (_model == null)
                return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                await _model.ShareFile(file);
                //if(!_model.FilesToSend.Contains(file, StringComparer.InvariantCultureIgnoreCase))
                //    _model.FilesToSend.Add(file);
            }
        }

        private void Grid_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                _model.SendMessage(ChatInputBox.Text);
                e.Handled = true;
                ChatMessagesBox.ScrollToEnd();
                ChatInputBox.Clear();
            }
        }
    }
}

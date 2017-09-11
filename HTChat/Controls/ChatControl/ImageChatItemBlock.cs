using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HTChat.Controls
{
    class ImageChatItemBlock: ChatItemBlock
    {
        public bool LoadSuccess { get; set; }
        public string Link { get; set; }
        public ImageSource Image { get; set; }
    }
}

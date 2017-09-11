using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Controls
{
    public class ChatItem
    {
        public string Text { get; set; }
        public ChatItemBlock[] ContentBlocks { get; set; }
    }
}

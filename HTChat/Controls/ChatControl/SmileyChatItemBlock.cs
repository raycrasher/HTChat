using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HTChat.Controls
{
    class SmileyChatItemBlock: ChatItemBlock
    {
        public static ImageSource[] Smileys { get; set; }

        public int SmileyIndex { get; set; }
    }
}

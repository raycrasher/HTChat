using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HTChat.Models
{
    public interface IChatItem
    {
        Jid Author { get; }
        DateTime Timestamp { get; }
        Block RenderBlock();
    }
}

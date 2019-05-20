using agsXMPP.protocol.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Events
{
    class PresenceEvent: EventArgs
    {
        public Presence Presence { get; set; }
    }
}

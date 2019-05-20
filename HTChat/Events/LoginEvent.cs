using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Events
{
    public class LoginEvent: EventArgs
    {
        public ChatClient Client { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTChat
{
    public static class Dispatch
    {
        public static void InvokeUI(Action a)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(a);
        }
    }
}

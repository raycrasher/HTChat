using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HTChat
{
    public static class Utilities
    {
        public static void Init()
        {
            _dpObj = new DependencyObject();
        }
        private static DependencyObject _dpObj = new DependencyObject();
        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(_dpObj);
    }
}

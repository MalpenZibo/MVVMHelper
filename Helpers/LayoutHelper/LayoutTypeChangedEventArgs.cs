using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Helpers.LayoutHelper
{
    public class LayoutTypeChangedEventArgs : EventArgs
    {
        public LayoutTypeChangedEventArgs( LayoutType args )
        {
            NewLayout = args;
        }

        public LayoutType NewLayout { get; private set; }
    }
}

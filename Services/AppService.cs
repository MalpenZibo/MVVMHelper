using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Services
{
    public class AppService
    {
        private ContentNavigationService _cns;
        private DialogService _ds;
        private SnackbarService _sbs;

        public AppService( ContentNavigationService cns, DialogService ds, SnackbarService sbs )
        {
            _cns = cns;
            _ds = ds;
            _sbs = sbs;
        }

        public ContentNavigationService Navigation { get { return _cns; } }

        public DialogService Dialog { get { return _ds; } }

        public SnackbarService Snackbar { get { return _sbs; } }
    }
}

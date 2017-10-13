using GalaSoft.MvvmLight;
using MVVMHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Moldels
{
    /// <summary>
    /// Voce di menu
    /// </summary>
    public class MenuItem : BaseModel
    {
        private string _name;
        private string _pageKey;
        private object _param;

        public MenuItem( string name, string pageKey, object parameter )
        {
            _name = name;
            _pageKey = pageKey;
            _param = parameter;
        }

        public MenuItem( string name, string pageKey ) : this( name, pageKey, null ) { }

        /// <summary>
        /// Nome testuale della voce del menu
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                SetValue<string>( ref _name, value, "Name" );
            }
        }

        /// <summary>
        /// Chiave della pagina gestita dalla voce del menu
        /// </summary>
        public string PageKey
        {
            get { return _pageKey; }
            set
            {
                SetValue<string>( ref _pageKey, value, "PageKey" );
            }
        }

        /// <summary>
        /// Parametro da passare alla pagina
        /// </summary>
        public object Parameter
        {
            get { return _param; }
            set
            {
                SetValue<object>( ref _param, value, "Parameter" );
            }
        }
    }
}

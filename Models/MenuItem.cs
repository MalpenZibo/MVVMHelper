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
    public class MenuItem : ObservableObject
    {
        private string name;
        private string pageKey;
        private object param;

        public MenuItem( string name, string pageKey, object parameter )
        {
            this.name = name;
            this.pageKey = pageKey;
            this.param = parameter;
        }

        public MenuItem( string name, string pageKey ) : this( name, pageKey, null ) { }

        /// <summary>
        /// Nome testuale della voce del menu
        /// </summary>
        public string Name 
        {
            get => name;
            set => Set( "Name", ref name, value ); 
        }

        /// <summary>
        /// Chiave della pagina gestita dalla voce del menu
        /// </summary>
        public string PageKey
        {
            get => pageKey;
            set => Set( "PageKey", ref pageKey, value ); 
        }

        /// <summary>
        /// Parametro da passare alla pagina
        /// </summary>
        public object Parameter
        {
            get => param;
            set => Set( "Parameter", ref param, value ); 
        }
    }
}

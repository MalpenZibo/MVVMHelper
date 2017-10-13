using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.ViewModels
{
    /// <summary>
    /// Interfaccia per rendere un controllo navigabile
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// Metodo chiamato appena il controllo viene istanziato
        /// </summary>
        /// <param name="parameter"></param>
        void Initialized( object parameter );

        /// <summary>
        /// Metodo chiamato dopo il loaded del controllo
        /// </summary>
        /// <param name="parameter"></param>
        void Activated( object parameter );

        /// <summary>
        /// Metodo chiamato dopo l'unloaded della pagina
        /// </summary>
        /// <param name="parameter"></param>
        void Deactivated( object parameter );
    }
}

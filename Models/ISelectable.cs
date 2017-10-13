using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Models
{
    /// <summary>
    /// Interfaccia per gli oggetti selezionabili
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Indica se l'oggetto è selezionato o no
        /// </summary>
        bool IsSelected { get; set; }
    }
}

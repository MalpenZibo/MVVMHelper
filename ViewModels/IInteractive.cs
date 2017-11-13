using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.ViewModels
{
    public interface IInteractive
    {
        /// <summary>
        /// Metodo chiamato in caso di conferma
        /// </summary>
        /// <param name="parameter"></param>
        Task<bool> Confirm();

        /// <summary>
        /// Metodo chiamato in caso di annullamento
        /// </summary>
        /// <param name="parameter"></param>
        Task<bool> Abort();


        Task<bool> Leave();
    }
}

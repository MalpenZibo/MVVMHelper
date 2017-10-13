using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.ViewModels
{
    /// <summary>
    /// Interface for ViewModel that wrap a single model
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IWrapViewModel<TModel>
        where TModel : class, INotifyPropertyChanged
    {
        TModel Model { get; set; }
    }
}

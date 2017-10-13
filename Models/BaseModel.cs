using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Models
{
    /// <summary>
    /// Model di base, eredita l'interfaccia INotifyPropertyChanged dall'ObservableObject del MVVM Light
    /// </summary>
    public class BaseModel : ObservableObject
    {
        /// <summary>
        /// Imposta il valore al field di una property solo se il valore passato è diverso e lancia il PropertyChanged 
        /// </summary>
        /// <typeparam name="T">tipo su cui si opererà</typeparam>
        /// <param name="field">field da modificare</param>
        /// <param name="value">valore da impostare al field</param>
        /// <param name="propertyName">nome della proprietà</param>
        protected void SetValue<T>( ref T field, T value, [CallerMemberName]string propertyName = null )
        {
            if( !EqualityComparer<T>.Default.Equals(field, value) )
            {
                field = value;
                RaisePropertyChanged( propertyName );
            }
        }
    }
}

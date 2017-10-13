using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MVVMHelper.Converters
{
    /// <summary>
    /// Convertitore per trasformare un booleano in un valore di visibilità
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Inverte la logica del convertitore
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Converte un valore booleano in un valore di visibilità
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if( !IsInverted )
                return ( (bool)value ? Visibility.Visible : Visibility.Collapsed ); 
            else
                return ( !(bool)value ? Visibility.Visible : Visibility.Collapsed ); 
        }

        /// <summary>
        /// Restituisce un valore booleano a partire da un valore di visibilità
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if( !IsInverted )
                return ( (Visibility)value == Visibility.Visible ? true : false );
            else
                return ( (Visibility)value != Visibility.Visible ? true : false );
        }
    }
}

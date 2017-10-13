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
    /// Convertitore per invertire un valore di visibilità
    /// </summary>
    public class VisibilityInverterConverter : IValueConverter
    {
        /// <summary>
        /// Converte un Visible in un Collapsed e viceversa
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return ( (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible );
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}

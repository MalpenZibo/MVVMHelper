using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MVVMHelper.Converters
{
    /// <summary>
    /// Convertitore per invertire un valore booleano
    /// </summary>
    public class BoolInverterConverter : IValueConverter
    {
        /// <summary>
        /// Converte un valore booleano nel suo opposto
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return !(bool)value;
        }

        /// <summary>
        /// Converte l'opposto di un valore booleano nel suo valore originale
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return !(bool)value;
        }
    }
}

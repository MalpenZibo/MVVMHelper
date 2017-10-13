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
    /// Converte un confronto tra due valore in un valore booleano se il primo valore è più piccolo del valore in input
    /// </summary>
    public class LessThan : IValueConverter
    {
        /// <summary>
        /// Ritorna true se il valore è più piccolo del parametro passato al convertitore
        /// </summary>
        /// <param name="value">valore di riferimento</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">parametro con cui confrontare il valore di riferimento</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            double input, param;

            if( double.TryParse( value.ToString(), out input ) && double.TryParse( parameter.ToString(), out param ) )
                return input < param;
            else
                return false;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}

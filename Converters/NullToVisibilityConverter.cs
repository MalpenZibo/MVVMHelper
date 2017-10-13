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
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Inverte la logica del convertitore
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Converte un valore null in un valore di visibilità
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if( !IsInverted )
                return ( value == null ? Visibility.Collapsed : Visibility.Visible );
            else
                return ( value == null ? Visibility.Visible : Visibility.Collapsed );
        }
       
        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}

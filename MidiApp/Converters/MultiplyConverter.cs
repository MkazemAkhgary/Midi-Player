using System;
using System.Globalization;
using System.Windows.Data;

namespace MidiApp.Converters
{
    [ValueConversion(typeof(double[]), typeof(double))]
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return 0;
            double result = 1.0;
            for (int i = 0; i < values.Length; i++)
            {
                if(values[i] is double)
                    result *= (double) values[i];
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

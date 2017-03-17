using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace MidiApp.Converters
{
    /// <summary>
    /// If used on binding will multiply value with parameter.
    /// If used on multibinding will multiply values and parameter together.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double), ParameterType = typeof(double))]
    [ValueConversion(typeof(double[]), typeof(double), ParameterType = typeof(double))]
    public sealed class MultiplyConverter : IMultiValueConverter, IValueConverter
    {
        [NotNull]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return 0d;

            double result = parameter as double? ?? 1d;

            for (int i = 0; i < values.Length; i++)
            {
                if(values[i] is double)
                    result *= (double) values[i];
            }

            return result;
        }

        [NotNull]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && parameter is double)
                return (double) value*(double) parameter;
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

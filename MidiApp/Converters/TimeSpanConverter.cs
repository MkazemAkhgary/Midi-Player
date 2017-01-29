using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace MidiApp.Converters
{
    /// <summary>
    /// Converts double in milliseconds into well fromatted timespan.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public sealed class TimeSpanConverter : IValueConverter
    {
        [NotNull]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                var time = TimeSpan.FromMilliseconds((double) value);
                return $@"{time.Days:#0:;;\}{time.Hours:#0:;;\}{time.Minutes:00:}{time.Seconds:00}";
            }
            else return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
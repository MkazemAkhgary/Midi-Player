using System;
using System.Globalization;
using System.Windows.Data;

namespace MidiApp.Converters
{
    /// <summary>
    /// Converts double in milliseconds into well fromatted timespan.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    internal class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as double?;
            if (!d.HasValue) return null;

            var time = TimeSpan.FromMilliseconds(d.Value);
            return $@"{time.Days:#0:;;\}{time.Hours:#0:;;\}{time.Minutes:00:}{time.Seconds:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;
using MidiApp.Styles;

namespace MidiApp.Converters
{
    /// <summary>
    /// returns thickness and sets given value to uniform length,
    /// array of thickness with length of two can be passed as parameter, 
    /// first element of parameter is used for thickness multiplication and second element is used for addition.
    /// note that multiplication has higher priority.
    /// </summary>
    [ValueConversion(typeof(double), typeof(Thickness), ParameterType = typeof(Thickness))]
    [ValueConversion(typeof(double), typeof(Thickness), ParameterType = typeof(ThicknessList))]
    public sealed class ThicknessConverter : IValueConverter
    {
        [NotNull]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                var val = (double)value;
                var param = parameter as ThicknessList;

                if (param?.Count >= 1)
                {
                    if (param.Count >= 2)
                    {
                        return new Thickness(
                            val*param[0].Left + param[1].Left,
                            val*param[0].Top + param[1].Top,
                            val*param[0].Right + param[1].Right,
                            val*param[0].Bottom + param[1].Bottom);
                    }
                    else
                    {
                        return new Thickness(
                            val*param[0].Left,
                            val*param[0].Top,
                            val*param[0].Right,
                            val*param[0].Bottom);
                    }

                }
                else
                {
                    if (parameter is Thickness)
                    {
                        var mul = (Thickness)parameter;
                        return new Thickness(
                            val*mul.Left,
                            val*mul.Top,
                            val*mul.Right,
                            val*mul.Bottom);
                    }
                    return new Thickness(val);
                }
            }
            else return default(Thickness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

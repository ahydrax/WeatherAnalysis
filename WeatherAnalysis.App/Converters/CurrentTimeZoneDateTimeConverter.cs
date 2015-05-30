using System;
using System.Globalization;
using System.Windows.Data;

namespace WeatherAnalysis.App.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class CurrentTimeZoneDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentTimeZone = TimeZone.CurrentTimeZone;
            var datetime = (DateTime) value;
            return currentTimeZone.ToLocalTime(datetime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentTimeZone = TimeZone.CurrentTimeZone;
            var datetimeString = (string)value;
            var datetime = DateTime.Parse(datetimeString);
            return currentTimeZone.ToUniversalTime(datetime);
        }
    }
}

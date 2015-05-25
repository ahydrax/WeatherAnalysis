using System;

namespace WeatherAnalysis.Core.Service.OpenWeather
{
    public static class DateTimeHelper
    {
        public static int GetUnixTimestamp(this DateTime dateTime)
        {
            return Convert.ToInt32(dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static DateTime CreateFromTimestamp(int unixTimesamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(unixTimesamp);
        }
    }
}

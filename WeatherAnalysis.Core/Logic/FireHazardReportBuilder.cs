using System;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Logic
{
    public sealed class FireHazardReportBuilder
    {
        private readonly IWeatherRecordManager _weatherRecordManager;

        public FireHazardReportBuilder(IWeatherRecordManager weatherRecordManager)
        {
            _weatherRecordManager = weatherRecordManager;
        }

        public FireHazardReport BuildReport(WeatherRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (record.Location == null) throw new ArgumentException("record.Location");
            if (record.LocationId == null) throw new ArgumentException("record.LocationId");

            var report = new FireHazardReport
            {
                Created = DateTime.Now,
                Weather = record
            };

            var lastRainyWeather = _weatherRecordManager.GetLastRainyWeatherRecord(record);

            var daysWithoutRain = DaysWithoutRain(record, lastRainyWeather);

            report.FireHazardCoefficient = CalculateFireHazardCoefficient(record, daysWithoutRain);

            return report;
        }

        private static decimal CalculateFireHazardCoefficient(WeatherRecord record, int daysWithoutRain)
        {
            return daysWithoutRain*(record.Temperature - record.DewPoint)*record.Temperature;
        }

        private static int DaysWithoutRain(WeatherRecord currentWeather, WeatherRecord lastRainyWeather)
        {
            return Convert.ToInt32(Math.Ceiling((currentWeather.Created - lastRainyWeather.Created).TotalDays));
        }
    }
}
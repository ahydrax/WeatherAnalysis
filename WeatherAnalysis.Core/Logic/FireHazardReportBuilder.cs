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

            var lastRainyWeather = _weatherRecordManager.GetLastRainyDay(record.LocationId.Value, record.Created.AddMonths(-2), record.Created.Date);
            var daysWithoutRain = DaysWithoutRain(record, lastRainyWeather);

            var report = new FireHazardReport
            {
                Created = DateTime.Now,
                WeatherRecordId = record.Id,
                Weather = record,
                LocationId = record.LocationId,
                Location = record.Location,
                LastRainyDate = lastRainyWeather,
                FireHazardCoefficient = CalculateFireHazardCoefficient(record, daysWithoutRain)
            };

            return report;
        }

        private static decimal CalculateFireHazardCoefficient(WeatherRecord record, int daysWithoutRain)
        {
            return daysWithoutRain * (record.Temperature - record.DewPoint) * record.Temperature;
        }

        private static int DaysWithoutRain(WeatherRecord currentWeather, DateTime lastRainyDay)
        {
            return Convert.ToInt32(Math.Ceiling((currentWeather.Created - lastRainyDay).TotalDays));
        }
    }
}
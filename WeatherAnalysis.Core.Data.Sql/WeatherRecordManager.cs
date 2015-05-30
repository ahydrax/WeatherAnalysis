using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Exceptions;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data.Sql
{
    public sealed class WeatherRecordManager : IWeatherRecordManager
    {
        private readonly string _configurationString;

        public WeatherRecordManager(string configurationString)
        {
            _configurationString = configurationString;
        }

        public IReadOnlyCollection<WeatherRecord> Get(int locationId, DateTime from, DateTime to)
        {
            using (var db = new DataConnection(_configurationString))
            {
                var weatherRecords = db.GetTable<WeatherRecord>()
                    .LoadWith(record => record.Location)
                    .Where(r => r.LocationId == locationId)
                    .Where(r => r.Created >= from)
                    .Where(r => r.Created <= to)
                    .ToArray();

                return weatherRecords;
            }
        }

        public DateTime GetLastRainyDay(int locationId, DateTime from, DateTime to)
        {
            var lastRainyDay = to.Date;

            while (lastRainyDay > from)
            {
                var records = Get(locationId, lastRainyDay, lastRainyDay.AddHours(24));

                if (records.Sum(r => r.Precipitation) >= 3)
                    return lastRainyDay;

                lastRainyDay = lastRainyDay.AddDays(-1);
            }

            throw new WeatherRecordNotFoundException("No info found about last rainy day.");
        }

        public void Save(WeatherRecord weatherRecord)
        {
            using (var db = new DataConnection(_configurationString))
            {
                if (weatherRecord.Id.HasValue)
                {
                    db.Update(weatherRecord);
                }
                else
                {
                    weatherRecord.Id = Convert.ToInt32(db.InsertWithIdentity(weatherRecord));
                }
            }
        }

        public void Delete(WeatherRecord weatherRecord)
        {
            using (var db = new DataConnection(_configurationString))
            {
                db.Delete(weatherRecord);
            }
        }
    }
}

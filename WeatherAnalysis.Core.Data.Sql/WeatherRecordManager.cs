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
                    .Where(r => r.LocationId == locationId)
                    .Where(r => r.Created >= from)
                    .Where(r => r.Created <= to)
                    .ToArray();

                return weatherRecords;
            }
        }

        public WeatherRecord GetLastRainyWeatherRecord(WeatherRecord current)
        {
            using (var db = new DataConnection(_configurationString))
            {
                var rainyWeatherRecord = db.GetTable<WeatherRecord>()
                    .Where(r => r.LocationId == current.LocationId)
                    .Where(r => r.Created <= current.Created)
                    .LastOrDefault(r => r.Rainy);

                if (rainyWeatherRecord == null) throw new WeatherRecordNotFoundException("No info found about last rainy day.");

                return rainyWeatherRecord;
            }
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

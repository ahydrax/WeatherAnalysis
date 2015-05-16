using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public sealed class WeatherRecordManager
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

using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data.Sql
{
    public sealed class LocationManager : ILocationManager
    {
        private readonly string _configurationString;

        public LocationManager(string configurationString)
        {
            _configurationString = configurationString;
        }

        public IReadOnlyCollection<Location> GetAll()
        {
            using (var db = new DataConnection(_configurationString))
            {
                var locations = CreateQuery(db);
                return locations.ToArray();
            }
        }

        public IReadOnlyCollection<Location> Get(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart)) return Enumerable.Empty<Location>().ToArray();

            using (var db = new DataConnection(_configurationString))
            {
                var locations = CreateQuery(db).Where(location => location.Name.Contains(namePart));
                return locations.ToArray();
            }
        }

        private static IQueryable<Location> CreateQuery(DataConnection db)
        {
            var locations =
                from location in db.GetTable<Location>()
                join report in db.GetTable<FireHazardReport>() on location.Id equals report.LocationId into reports
                join record in db.GetTable<WeatherRecord>() on location.Id equals record.LocationId into records
                select new Location
                {
                    Id = location.Id,
                    Name = location.Name,
                    SystemName = location.SystemName,
                    FireHazardReportsCount = reports.Count(),
                    WeatherRecordsCount = records.Count()
                };
            return locations;
        }

        public void Save(Location location)
        {
            using (var db = new DataConnection(_configurationString))
            {
                if (location.Id.HasValue)
                {
                    db.Update(location);
                }
                else
                {
                    location.Id = Convert.ToInt32(db.InsertWithIdentity(location));
                }
            }
        }

        public void Delete(Location location)
        {
            using (var db = new DataConnection(_configurationString))
            {
                db.Delete(location);
            }
        }
    }
}

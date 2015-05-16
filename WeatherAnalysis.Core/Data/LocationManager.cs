using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public sealed class LocationManager
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
                var locations = db.GetTable<Location>().Select(location => location);
                return locations.ToArray();
            }
        }

        public IReadOnlyCollection<Location> Get(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart)) return Enumerable.Empty<Location>().ToArray();

            using (var db = new DataConnection(_configurationString))
            {
                var locations = db.GetTable<Location>()
                    .Where(location => location.Name.Contains(namePart))
                    .ToArray();

                return locations;
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public class FireHazardReportManager
    {
        private readonly string _configurationString;

        public FireHazardReportManager(string configurationString)
        {
            _configurationString = configurationString;
        }

        public IReadOnlyCollection<FireHazardReport> Get(int? locationId, int? weatherRecordId)
        {
            using (var db = new DataConnection(_configurationString))
            {
                var reports = db.GetTable<FireHazardReport>().Select(r => r);

                if (locationId.HasValue)
                    reports = reports.Where(r => r.Location.Id == locationId.Value);

                if (weatherRecordId.HasValue)
                    reports = reports.Where(r => r.WeatherRecordId == weatherRecordId.Value);

                return reports.ToArray();
            }
        }

        public void Save(FireHazardReport report)
        {
            using (var db = new DataConnection(_configurationString))
            {
                if (report.Id.HasValue)
                {
                    db.Update(report);
                }
                else
                {
                    report.Id = Convert.ToInt32(db.InsertWithIdentity(report));
                }
            }
        }

        public void Delete(FireHazardReport report)
        {
            using (var db = new DataConnection(_configurationString))
            {
                db.Delete(report);
            }
        }
    }
}

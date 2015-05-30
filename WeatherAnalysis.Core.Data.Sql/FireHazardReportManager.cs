using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data.Sql
{
    public class FireHazardReportManager : IFireHazardReportManager
    {
        private readonly string _configurationString;

        public FireHazardReportManager(string configurationString)
        {
            _configurationString = configurationString;
        }

        public IReadOnlyCollection<FireHazardReport> Get(int locationId, DateTime from, DateTime to)
        {
            using (var db = new DataConnection(_configurationString))
            {
                var reports = db.GetTable<FireHazardReport>()
                    .LoadWith(r => r.Location)
                    .LoadWith(r => r.Weather)
                    .Select(r => r)
                    .Where(r => r.Location.Id == locationId)
                    .Where(r => r.Created >= from)
                    .Where(r => r.Created <= to);

                return reports.OrderByDescending(r => r.Created).ToArray();
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

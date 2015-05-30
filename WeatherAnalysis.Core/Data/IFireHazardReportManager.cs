using System;
using System.Collections.Generic;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public interface IFireHazardReportManager
    {
        IReadOnlyCollection<FireHazardReport> Get(int locationId, DateTime from, DateTime to);
        void Save(FireHazardReport report);
        void Delete(FireHazardReport report);
    }
}
using System;
using System.Collections.Generic;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public interface IWeatherRecordManager
    {
        IReadOnlyCollection<WeatherRecord> Get(int locationId, DateTime from, DateTime to);
        WeatherRecord GetLastRainyWeatherRecord(WeatherRecord current);
        void Save(WeatherRecord weatherRecord);
        void Delete(WeatherRecord weatherRecord);
    }
}
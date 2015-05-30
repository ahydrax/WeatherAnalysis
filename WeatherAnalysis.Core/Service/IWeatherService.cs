using System;
using System.Collections.Generic;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Service
{
    public interface IWeatherService
    {
        IReadOnlyCollection<WeatherRecord> GetWeatherData(Location location, DateTime from, DateTime to);
    }
}

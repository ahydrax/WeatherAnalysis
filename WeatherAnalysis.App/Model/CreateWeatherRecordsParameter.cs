using System;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.Model
{
    public class CreateWeatherRecordsParameter
    {
        public DateTime Date { get; set; } 
        public Location Location { get; set; } 
    }
}

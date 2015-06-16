using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using RestSharp;
using WeatherAnalysis.Core.Exceptions;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Service.OpenWeather
{
    public class OpenWeatherService : IWeatherService
    {
        private const string BaseApiUrl = @"http://api.openweathermap.org/data/2.5/";

        private readonly IRestClient _restClient;

        public static OpenWeatherService CreateService()
        {
            try
            {
                var restClient = new RestClient(BaseApiUrl);
                var apiKey = ConfigurationManager.AppSettings["openWeatherApiKey"];
                restClient.AddDefaultHeader("x-api-key", apiKey);
                restClient.AddHandler("application/json", new DynamicJsonDeserializer());
                return new OpenWeatherService(restClient);
            }
            catch (ConfigurationErrorsException e)
            {
                throw new WeatherAnalysisException("openWeatherApiKey hasn't found in config.xml", e);
            }
        }

        public OpenWeatherService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public IReadOnlyCollection<WeatherRecord> GetWeatherData(Location location, DateTime from, DateTime to)
        {
            var request = PrepareRequest(location.SystemName);
            var response = _restClient.Execute<dynamic>(request);

            if (response.Data.list == null) throw new WeatherServiceException("Weather data load error.");

            var forecast = ExtractForecast(response.Data, from, to);
            var result = CreateWeatherRecords(location, forecast);
            return result.AsReadOnly();
        }

        private IRestRequest PrepareRequest(string locationName)
        {
            var request = new RestRequest("/forecast", Method.GET);
            request.AddQueryParameter("q", locationName);
            return request;
        }

        private List<dynamic> ExtractForecast(dynamic data, DateTime from, DateTime to)
        {
            var todayForecast = new List<dynamic>();
            foreach (var weatherData in data.list)
            {
                var date = DateTimeHelper.CreateFromTimestamp(Convert.ToInt32(weatherData.dt));
                if (IsBetween(from, date, to))
                {
                    todayForecast.Add(weatherData);
                }
            }
            return todayForecast;
        }

        private bool IsBetween(DateTime from, DateTime date, DateTime to)
        {
            return date >= from && date <= to;
        }

        private static List<WeatherRecord> CreateWeatherRecords(Location location, List<dynamic> todayForecast)
        {
            return todayForecast.Select(weatherData => new WeatherRecord
            {
                Created = DateTimeHelper.CreateFromTimestamp(Convert.ToInt32(weatherData.dt)),
                Humidity = Convert.ToDecimal(weatherData.main.humidity),
                Location = location,
                LocationId = location.Id,
                Precipitation = Convert.ToDecimal(weatherData.rain == null ? 0 : weatherData.rain["3h"]),
                Temperature = Convert.ToDecimal(weatherData.main.temp) - 273
            }).ToList();
        }
    }
}

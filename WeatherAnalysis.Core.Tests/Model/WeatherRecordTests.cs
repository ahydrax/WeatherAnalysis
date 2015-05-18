using System;
using WeatherAnalysis.Core.Model;
using Xunit;

namespace WeatherAnalysis.Core.Tests.Model
{
    public class WeatherRecordTests
    {
        [Theory]
        [InlineData(5.00, 70, 0.0)]
        [InlineData(15.0, 30, -2.4)]
        [InlineData(17.5, 20, -5.7)]
        [InlineData(20.0, 50, 9.3)]
        [InlineData(22.5, 50, 11.5)]
        [InlineData(25.0, 75, 20.3)]
        public void DewPoint(decimal temperature, decimal humidity, decimal dewPoint)
        {
            var weatherRecord = new WeatherRecord { Temperature = temperature, Humidity = humidity };

            Assert.Equal(dewPoint, weatherRecord.DewPoint, 1);
        }

        [Fact]
        public void Humidity_CantBeAbove_100()
        {
            var weatherRecord = new WeatherRecord();

            Assert.Throws<ArgumentOutOfRangeException>(() => weatherRecord.Humidity = 101);
        }

        [Fact]
        public void Humidity_CanBeBetween_0_And_100()
        {
            var weatherRecord = new WeatherRecord();
            weatherRecord.Humidity = 50;
        }

        [Fact]
        public void Humidity_CantBeBelow_0()
        {
            var weatherRecord = new WeatherRecord();

            Assert.Throws<ArgumentOutOfRangeException>(() => weatherRecord.Humidity = -1);
        }

        [Fact]
        public void Precipitation_CantBeBelow_0()
        {
            var weatherRecord = new WeatherRecord();

            Assert.Throws<ArgumentOutOfRangeException>(() => weatherRecord.Precipitation = -1);
        }
        
        [Fact]
        public void Precipitation_CanBeAbove_0()
        {
            var weatherRecord = new WeatherRecord();

            weatherRecord.Precipitation = 4;
        }
    }
}

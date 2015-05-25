using WeatherAnalysis.Core.Model;
using Xunit;

namespace WeatherAnalysis.Core.Tests.Data
{
    public class WeatherRecordManagerTests
    {
        [Fact]
        public void Rainy_WhenPrecipitationIsAbove_3mm()
        {
            var record = new WeatherRecord { Precipitation = 5 };
            Assert.True(record.Rainy);
        }

        [Fact]
        public void Rainy_WhenPrecipitationIsEqual_3mm()
        {
            var record = new WeatherRecord { Precipitation = 3 };
            Assert.True(record.Rainy);
        }

        [Fact]
        public void Not_Rainy_WhenPrecipitationIsBelow_3mm()
        {
            var record = new WeatherRecord { Precipitation = 2 };
            Assert.False(record.Rainy);
        }
    }
}

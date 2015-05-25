using System;
using WeatherAnalysis.Core.Service.OpenWeather;
using Xunit;

namespace WeatherAnalysis.Core.Tests.Service.OpenWeather
{
    public class DateTimeHelperTests
    {
        public static readonly object[] UnixTimestampTheoryData = 
        {
            new object[]
            {
                new DateTime(1970, 1, 1),
                0
            },
            new object[]
            {
                new DateTime(2015, 5, 25),
                1432512000
            }
        };

        [Theory, MemberData("UnixTimestampTheoryData")]
        public void GetUnixTimestampTheory(DateTime dateTime, int expectedTimestamp)
        {
            var actualTimestamp = dateTime.GetUnixTimestamp();

            Assert.Equal(expectedTimestamp, actualTimestamp);
        }

        [Theory, MemberData("UnixTimestampTheoryData")]
        public void CreateFromTimestampTheory(DateTime expectedDate, int timestamp)
        {
            var actualDate = DateTimeHelper.CreateFromTimestamp(timestamp);

            Assert.Equal(expectedDate, actualDate);
        }
    }
}

using System;
using Moq;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Logic;
using WeatherAnalysis.Core.Model;
using Xunit;

namespace WeatherAnalysis.Core.Tests.Logic
{
    public class FireHazardReportBuilderTests
    {
        public static readonly Location Khabarovsk = new Location { Id = 27, Name = "Khabarovsk" };

        public static readonly object[] TheoryObjects = {
            new object[]
            {
                new WeatherRecord
                {
                    LocationId = Khabarovsk.Id,
                    Location = Khabarovsk,
                    Created = new DateTime(2015, 5, 31),
                    Temperature = 4.6M,
                    Humidity = 35
                },
                new DateTime(2015, 5, 1),
                new FireHazardReport
                {
                    FireHazardCoefficient = 1951
                }
            }
        };

        [Theory, MemberData("TheoryObjects")]
        public void BuildReportTest(WeatherRecord current, DateTime lastRainyDay, FireHazardReport expectedReport)
        {
            var manager = new Mock<IWeatherRecordManager>();
            manager.Setup(
                recordManager => recordManager.GetLastRainyDay(current.LocationId.Value, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(lastRainyDay);

            var builder = new FireHazardReportBuilder(manager.Object);

            var actualReport = builder.BuildReport(current);

            Assert.Equal(expectedReport.FireHazardCoefficient, actualReport.FireHazardCoefficient, 0);
        }
    }
}

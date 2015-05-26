using System;
using WeatherAnalysis.Core.Model;
using Xunit;

namespace WeatherAnalysis.Core.Tests.Model
{
    public class LocationTests
    {
        [Fact]
        public void Location_CanHaveName()
        {
            var location = new Location { Name = "Khabarovsk" };
        }

        [Fact]
        public void Location_CantHaveEmptyName()
        {
            var location = new Location();

            Assert.Throws<ArgumentException>(() => location.Name = "");
            Assert.Throws<ArgumentException>(() => location.Name = null);
        }

        [Fact]
        public void Location_CanHaveSystemName()
        {
            var location = new Location { SystemName = "Khabarovsk" };
        }

        [Fact]
        public void Location_CantHaveEmptySystemName()
        {
            var location = new Location();

            Assert.Throws<ArgumentException>(() => location.SystemName = "");
            Assert.Throws<ArgumentException>(() => location.SystemName = null);
        }
    }
}

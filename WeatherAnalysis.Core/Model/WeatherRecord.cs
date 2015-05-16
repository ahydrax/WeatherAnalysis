using System;
using LinqToDB.Mapping;

namespace WeatherAnalysis.Core.Model
{
    [Table(Name = "WeatherRecords")]
    public sealed class WeatherRecord
    {
        [PrimaryKey, Identity]
        public int? Id { get; set; }

        [PrimaryKey, NotNull]
        public int? LocationId
        {
            get { return Location.Id; }
            set { Location.Id = value; }
        }

        [Association(CanBeNull = false, ThisKey = "LocationId", OtherKey = "Id")]
        public Location Location { get; set; }

        [Column]
        public DateTime Created { get; set; }

        [Column]
        public decimal Temperature { get; set; }

        [Column]
        public decimal Humidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                if (value < 0.0M || value > 100.0M)
                    throw new ArgumentOutOfRangeException();

                _humidity = value;
            }
        }

        [NotColumn]
        public decimal DewPoint
        {
            get
            {
                var tau = AlphaCoefficient * Temperature / (BetaCoefficient + Temperature) + (decimal)Math.Log((double)Humidity / 100);
                var dewPoint = BetaCoefficient * tau / (AlphaCoefficient - tau);
                return dewPoint;
            }
        }

        private decimal _humidity;
        private const decimal AlphaCoefficient = 17.27M;
        private const decimal BetaCoefficient = 237.7M;
    }
}

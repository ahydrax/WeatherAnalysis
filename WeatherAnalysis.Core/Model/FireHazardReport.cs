using System;
using LinqToDB.Mapping;

namespace WeatherAnalysis.Core.Model
{
    [Table(Name = "FireHazardReports")]
    public sealed class FireHazardReport
    {
        [PrimaryKey, Identity]
        public int? Id { get; set; }

        [PrimaryKey, NotNull]
        public int? WeatherRecordId { get; set; }

        [Association(CanBeNull = false, ThisKey = "WeatherRecordId", OtherKey = "Id")]
        public WeatherRecord Weather { get; set; }

        [Column]
        public DateTime Created { get; set; }

        [Column]
        public decimal FireHazardCoefficient { get; set; }

        [NotColumn]
        public Location Location
        {
            get { return Weather.Location; }
        }
    }
}

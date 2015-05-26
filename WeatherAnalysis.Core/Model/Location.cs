using System;
using LinqToDB.Mapping;

namespace WeatherAnalysis.Core.Model
{
    [Table(Name = "Locations")]
    public sealed class Location
    {
        [PrimaryKey, Identity]
        public int? Id { get; set; }

        [Column, NotNull]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException();
                _name = value;
            }
        }

        [Column, NotNull]
        public string SystemName
        {
            get { return _systemName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException();
                _systemName = value;
            }
        }
        
        [NotColumn]
        public int WeatherRecordsCount { get; set; }

        [NotColumn]
        public int FireHazardReportsCount { get; set; }

        private string _name;
        private string _systemName;
    }
}

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

        private string _name;
    }
}

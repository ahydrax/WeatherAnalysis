using System.Collections.Generic;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public interface ILocationManager
    {
        IReadOnlyCollection<Location> GetAll();
        IReadOnlyCollection<Location> Get(string namePart);
        void Save(Location location);
        void Delete(Location location);
    }
}
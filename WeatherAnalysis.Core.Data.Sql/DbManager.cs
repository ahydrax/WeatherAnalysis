using System.Data.Common;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data.Sql
{
    public sealed class DbManager : IDbManager
    {
        private readonly string _configurationString;

        public DbManager(string configurationString)
        {
            _configurationString = configurationString;
        }

        public bool HasValidSchema()
        {
            using (var db = new DataConnection(_configurationString))
            {
                try
                {
                    db.GetTable<Location>().FirstOrDefault();
                    db.GetTable<WeatherRecord>().FirstOrDefault();
                    db.GetTable<FireHazardReport>().FirstOrDefault();

                    return true;
                }
                catch (DbException)
                {
                    return false;
                }
            }
        }

        public void CreateSchema()
        {
            using (var db = new DataConnection(_configurationString))
            {
                CreateIfNotExists<Location>(db);
                CreateIfNotExists<WeatherRecord>(db);
                CreateIfNotExists<FireHazardReport>(db);
            }
        }

        public void CleanUp()
        {
            using (var db = new DataConnection(_configurationString))
            {
                DropIfExists<Location>(db);
                DropIfExists<WeatherRecord>(db);
                DropIfExists<FireHazardReport>(db);
            }
        }

        private static void CreateIfNotExists<T>(DataConnection db) where T : class
        {
            if (!TableExists<T>(db))
                db.CreateTable<T>();
        }

        private static void DropIfExists<T>(DataConnection db) where T : class
        {
            if (TableExists<T>(db))
                db.DropTable<T>();
        }

        private static bool TableExists<T>(DataConnection db) where T : class
        {
            try
            {
                db.GetTable<T>().Any();
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }
    }
}

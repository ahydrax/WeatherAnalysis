using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.Core.Data
{
    public sealed class DbManager
    {
        private readonly string _configurationName;

        public DbManager(string configurationName)
        {
            _configurationName = configurationName;
        }

        public bool IsInValidState()
        {
            using (var db = new DataConnection(_configurationName))
            {
                try
                {
                    db.GetTable<Location>().Any();
                    db.GetTable<WeatherRecord>().Any();
                    db.GetTable<FireHazardReport>().Any();

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
            using (var db = new DataConnection(_configurationName))
            {
                CreateIfNotExists<Location>(db);
                CreateIfNotExists<WeatherRecord>(db);
                CreateIfNotExists<FireHazardReport>(db);
            }
        }

        public void CleanUp()
        {
            using (var db = new DataConnection(_configurationName))
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

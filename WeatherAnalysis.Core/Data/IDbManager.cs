namespace WeatherAnalysis.Core.Data
{
    public interface IDbManager
    {
        bool HasValidSchema();
        void CreateSchema();
        void CleanUp();
    }
}

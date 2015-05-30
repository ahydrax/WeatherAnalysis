using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.Helpers
{
    public static class ConclusionHelper
    {
        public static string GetConclusion(FireHazardReport report)
        {
            string conclusion;
            var coefficient = report.FireHazardCoefficient;
            
            if (coefficient < 300)
            {
                conclusion = "1 классу пожарной опасности (отсутствие опасности).";
            }
            else if (coefficient < 1000)
            {
                conclusion = "2 классу пожарной опасности (малая пожарная опасность).";
            }
            else if (coefficient < 4000)
            {
                conclusion = "3 классу пожарной опасности (средняя пожарная опасность).";
            }
            else if (coefficient < 10000)
            {
                conclusion = "4 классу пожарной опасности (высокая пожарная опасность).";
            }
            else
            {
                conclusion = "5 классу пожарной опасности (чрезвычайная пожарная опасность).";
            }

            return string.Format("Комплексный показатель (К) равен {0:0.00}, что соответствует {1}", coefficient, conclusion);
        }
    }
}

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
                conclusion = "1 ������ �������� ��������� (���������� ���������).";
            }
            else if (coefficient < 1000)
            {
                conclusion = "2 ������ �������� ��������� (����� �������� ���������).";
            }
            else if (coefficient < 4000)
            {
                conclusion = "3 ������ �������� ��������� (������� �������� ���������).";
            }
            else if (coefficient < 10000)
            {
                conclusion = "4 ������ �������� ��������� (������� �������� ���������).";
            }
            else
            {
                conclusion = "5 ������ �������� ��������� (������������ �������� ���������).";
            }

            return string.Format("����������� ���������� (�) ����� {0:0.00}, ��� ������������� {1}", coefficient, conclusion);
        }
    }
}

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class ReportBuilderViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IFireHazardReportManager _fireHazardReportManager;

        private WeatherRecord _weatherRecord;
        private FireHazardReport _selectedReport;
        private string _conclusion = string.Empty;
        private string _signedBy = string.Empty;

        public ReportBuilderViewModel(IFireHazardReportManager fireHazardReportManager, INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger)
        {
            _fireHazardReportManager = fireHazardReportManager;
        }

        public void ReceiveParameter(object parameter)
        {
            var report = parameter as FireHazardReport;
            if (report == null) return;

            SelectedWeatherRecord = report.Weather;
            SelectedReport = report;
            Conclusion = ConclusionHelper.GetConclusion(report);
        }

        #region Properties

        public const string SelectedWeatherRecordPropertyName = "SelectedWeatherRecord";
        public WeatherRecord SelectedWeatherRecord
        {
            get { return _weatherRecord; }
            set { Set(SelectedWeatherRecordPropertyName, ref _weatherRecord, value); }
        }

        public const string SelectedReportPropertyName = "SelectedReport";
        public FireHazardReport SelectedReport
        {
            get { return _selectedReport; }
            set { Set(SelectedReportPropertyName, ref _selectedReport, value); }
        }

        public const string ConclusionPropertyName = "Conclusion";
        public string Conclusion
        {
            get { return _conclusion; }
            set { Set(ConclusionPropertyName, ref _conclusion, value); }
        }

        public const string SignedByPropertyName = "SignedBy";
        public string SignedBy
        {
            get { return _signedBy; }
            set { Set(SignedByPropertyName, ref _signedBy, value); }
        }

        #endregion

        #region Command



        #endregion
    }

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
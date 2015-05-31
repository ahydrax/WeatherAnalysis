using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Win32;
using WeatherAnalysis.App.Helpers;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class ReportBuilderViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IFireHazardReportManager _fireHazardReportManager;

        private RelayCommand<TextRange> _saveReportCommand;
        private RelayCommand<TextRange> _printReportCommand;

        private readonly object _recentReportsSyncReport = new object();
        private WeatherRecord _weatherRecord;
        private FireHazardReport _selectedReport;
        private bool _isSaved = false;
        private string _conclusion = string.Empty;
        private string _signedBy = string.Empty;

        public ReportBuilderViewModel(IFireHazardReportManager fireHazardReportManager, INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger)
        {
            _fireHazardReportManager = fireHazardReportManager;

            PropertyChanged += OnPropertyChanged;

            RecentReports = new ObservableCollection<FireHazardReport>();
            BindingOperations.EnableCollectionSynchronization(RecentReports, _recentReportsSyncReport);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SignedByPropertyName)
            {
                SelectedReport.SignedBy = SignedBy;

            }
        }

        public void ReceiveParameter(object parameter)
        {
            var report = parameter as FireHazardReport;
            if (report == null) return;

            SelectedWeatherRecord = report.Weather;
            SelectedReport = report;
            Conclusion = ConclusionHelper.GetConclusion(report);

            GetRecentReports();
        }

        private void GetRecentReports()
        {
            if (!SelectedReport.LocationId.HasValue) return;

            StartProgress();
            RecentReports.Clear();
            var from = DateTime.Today.AddMonths(-1);
            var to = DateTime.Today.AddDays(1);

            var getTask = Task.Run(() => _fireHazardReportManager.Get(SelectedReport.LocationId.Value, from, to));
            getTask.ContinueWith(task => FinishProgress());
            getTask.ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled) return;

                foreach (var fireHazardReport in task.Result.Take(10))
                {
                    RecentReports.Add(fireHazardReport);
                }
            });
        }

        private void SaveReportToDbIfNotExists(FireHazardReport report)
        {
            if (_isSaved) return;

            var saveTask = Task.Run(() => _fireHazardReportManager.Save(report));
            saveTask.ContinueWith(task => GetRecentReports());
            saveTask.ContinueWith(task => _isSaved = !task.IsFaulted);
        }

        #region Properties

        public ObservableCollection<FireHazardReport> RecentReports { get; private set; }

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

        public RelayCommand<TextRange> SaveReport
        {
            get
            {
                return _saveReportCommand ?? (_saveReportCommand = new RelayCommand<TextRange>(
                    ExecuteSaveReport,
                    CanExecuteDocumentCommand));
            }
        }

        private void ExecuteSaveReport(TextRange reportTextRange)
        {
            SaveReportToDbIfNotExists(SelectedReport);

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "RTF документ (*.rtf)|*.rtf|Все файлы (*.*)|*.*";

            if (saveDialog.ShowDialog() == true)
            {
                SaveToFileAsync(saveDialog.FileName, reportTextRange);
            }
        }

        private void SaveToFileAsync(string fileName, TextRange text)
        {
            byte[] fileBytes;

            using (var memoryStream = new MemoryStream())
            {
                text.Save(memoryStream, DataFormats.Rtf);
                fileBytes = memoryStream.ToArray();
            }

            var saveTask = Task.Run(() =>
            {
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.Write(fileBytes, 0, fileBytes.Length);
                    fs.Flush(true);
                }
            });
            saveTask.ContinueWith(DispatchError);
        }

        public RelayCommand<TextRange> PrintReport
        {
            get
            {
                return _printReportCommand ?? (_printReportCommand = new RelayCommand<TextRange>(
                    ExecutePrintReport,
                    CanExecuteDocumentCommand));
            }
        }

        private void ExecutePrintReport(TextRange reportTextRange)
        {
            SaveReportToDbIfNotExists(SelectedReport);

            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;

            var document = new FlowDocument
            {
                ColumnWidth = 21.0*96/2.54,
                PageWidth = 21.0*96/2.54,
                PageHeight = 29.7*96/2.54,
                PagePadding = new Thickness(2*96/2.54, 1.5*96/2.54, 1.5*96/2.54, 96.0/2.54)
            };

            var printTextRange = new TextRange(document.ContentStart, document.ContentEnd);

            using (var memoryStream = new MemoryStream())
            {
                reportTextRange.Save(memoryStream, DataFormats.Xaml);
                printTextRange.Load(memoryStream, DataFormats.Xaml);
            }

            var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;

            printDialog.PrintDocument(paginator, SignedBy);
        }

        private bool CanExecuteDocumentCommand(TextRange textRange)
        {
            return SelectedReport != null && !string.IsNullOrWhiteSpace(SelectedReport.SignedBy);
        }

        #endregion
    }

}
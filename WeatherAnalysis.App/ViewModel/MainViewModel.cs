using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Model;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Logic;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IWeatherRecordManager _weatherRecordManager;

        private Location _selectedLocation;
        private DateTime _selectedDate = DateTime.Today;
        private WeatherRecord _selectedWeatherRecord;

        private RelayCommand _selectLocationCommand;
        private RelayCommand _createWeatherRecordsCommand;
        private RelayCommand<WeatherRecord> _buildReportCommand;
        private RelayCommand<WeatherRecord> _removeWeatherRecordCommand;
        
        private readonly object _weatherRecordsSyncRoot = new object();
        public ObservableCollection<WeatherRecord> WeatherRecords { get; private set; }

        public MainViewModel(IWeatherRecordManager weatherRecordManager, INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger)
        {
            _weatherRecordManager = weatherRecordManager;

            InitializeWeatherRecordsCollection();
            InitializeEventHandlers();
            Subscribe();
        }

        private void InitializeWeatherRecordsCollection()
        {
            WeatherRecords = new ObservableCollection<WeatherRecord>();
            BindingOperations.EnableCollectionSynchronization(WeatherRecords, _weatherRecordsSyncRoot);
        }

        private void InitializeEventHandlers()
        {
            PropertyChanged += SelectedDateChanged;
        }

        private void SelectedDateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SelectedDatePropertyName)
            {
                GetWeatherRecords();
            }
        }

        private void Subscribe()
        {
            Messenger.Register<Location>(this, NewLocationSelected);
            Messenger.Register<IReadOnlyCollection<WeatherRecord>>(this, WeatherRecordsCreated);
        }

        private void NewLocationSelected(Location location)
        {
            if (location == null) return;

            SelectedLocation = location;
            GetWeatherRecords();
        }

        private void WeatherRecordsCreated(IReadOnlyCollection<WeatherRecord> weatherRecords)
        {
            var saveTask = Task.Run(() =>
            {
                foreach (var weatherRecord in weatherRecords)
                {
                    _weatherRecordManager.Save(weatherRecord);
                }
            });

            saveTask.ContinueWith(task => GetWeatherRecords());
        }

        private void GetWeatherRecords()
        {
            if (SelectedLocation == null || !SelectedLocation.Id.HasValue) return;

            StartProgress();
            WeatherRecords.Clear();

            var refreshTask = Task.Run(() => _weatherRecordManager.Get(SelectedLocation.Id.Value, SelectedDate.Date, SelectedDate.Date.AddHours(24)));
            refreshTask.ContinueWith(task => FinishProgress());
            refreshTask.ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled) return;
                foreach (var weatherRecord in task.Result)
                {
                    WeatherRecords.Add(weatherRecord);
                }
            });
            refreshTask.ContinueWith(DispatchError);
        }

        #region Properties

        public const string SelectedLocationPropertyName = "SelectedLocation";
        public Location SelectedLocation
        {
            get { return _selectedLocation; }
            set { Set(SelectedLocationPropertyName, ref _selectedLocation, value); }
        }

        public const string SelectedDatePropertyName = "SelectedDate";
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { Set(SelectedDatePropertyName, ref _selectedDate, value); }
        }

        public const string SelectedWeatherRecordPropertyName = "SelectedWeatherRecord";
        public WeatherRecord SelectedWeatherRecord
        {
            get { return _selectedWeatherRecord; }
            set { Set(SelectedWeatherRecordPropertyName, ref _selectedWeatherRecord, value); }
        }

        #endregion

        #region Commands

        public RelayCommand SelectLocation
        {
            get
            {
                return _selectLocationCommand ?? (_selectLocationCommand = new RelayCommand(
                    () => NavigationService.NavigateTo(Dialogs.LocationSelect)));
            }
        }

        public RelayCommand CreateWeatherRecords
        {
            get
            {
                return _createWeatherRecordsCommand ?? (_createWeatherRecordsCommand = new RelayCommand(
                    ExecuteCreateWeatherRecords,
                    CanExecuteCreateWeatherRecords));
            }
        }

        private void ExecuteCreateWeatherRecords()
        {
            var parameter = new CreateWeatherRecordsParameter
            {
                Date = SelectedDate,
                Location = SelectedLocation
            };
            NavigationService.NavigateTo(Dialogs.CreateWeatherRecords, parameter);
        }

        private bool CanExecuteCreateWeatherRecords()
        {
            return SelectedLocation != null;
        }

        public RelayCommand<WeatherRecord> BuildReport
        {
            get
            {
                return _buildReportCommand
                    ?? (_buildReportCommand = new RelayCommand<WeatherRecord>(
                        ExecuteBuildReport,
                        record => record != null));
            }
        }

        private void ExecuteBuildReport(WeatherRecord record)
        {
            var b = new FireHazardReportBuilder(_weatherRecordManager);
            var report = b.BuildReport(record);
            NavigationService.NavigateTo(Dialogs.ReportBuilder, report);
        }

        public RelayCommand<WeatherRecord> RemoveWeatherRecord
        {
            get
            {
                return _removeWeatherRecordCommand
                    ?? (_removeWeatherRecordCommand = new RelayCommand<WeatherRecord>(
                        ExecuteRemoveWeatherRecord,
                        record => record != null));
            }
        }

        private void ExecuteRemoveWeatherRecord(WeatherRecord record)
        {
            var removeTask = Task.Run(() => _weatherRecordManager.Delete(record));
            removeTask.ContinueWith(task => GetWeatherRecords());
        }

        #endregion
    }
}

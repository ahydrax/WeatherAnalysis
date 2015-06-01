using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Model;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Model;
using WeatherAnalysis.Core.Service;

namespace WeatherAnalysis.App.ViewModel
{
    public class CreateWeatherRecordsViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IWeatherService _weatherService;

        private RelayCommand _saveRecordsCommand;
        private RelayCommand _downloadRecordsCommand;

        private readonly ObservableCollection<WeatherRecord> _weatherRecords = new ObservableCollection<WeatherRecord>();
        private readonly object _weatherRecordsSyncRoot = new object();
        private Location _selectedLocation;
        private DateTime _selectedDate;
        
        public CreateWeatherRecordsViewModel(INavigationService navigationService, IMessenger messenger, IWeatherService weatherService)
            : base(navigationService, messenger)
        {
            _weatherService = weatherService;
            InitializeWeatherRecordsViewSource();
        }

        private void InitializeWeatherRecordsViewSource()
        {
            BindingOperations.EnableCollectionSynchronization(_weatherRecords, _weatherRecordsSyncRoot);
            WeatherRecordsViewSource = new CollectionViewSource { Source = _weatherRecords };
        }

        public void ReceiveParameter(object parameter)
        {
            var p = parameter as CreateWeatherRecordsParameter;
            if (p == null) return;

            SelectedLocation = p.Location;
            SelectedDate = p.Date;
        }

        #region Properties

        public ICollectionView WeatherRecordsView { get { return WeatherRecordsViewSource.View; } }
        public CollectionViewSource WeatherRecordsViewSource { get; private set; }

        public Location SelectedLocation
        {
            get { return _selectedLocation; }
            private set
            {
                _selectedLocation = value;
                RaisePropertyChanged();
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            private set
            {
                _selectedDate = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand SaveRecords
        {
            get
            {
                return _saveRecordsCommand ?? (_saveRecordsCommand = new RelayCommand(
                    ExecuteSaveRecords,
                    CanSaveRecords));
            }
        }

        private void ExecuteSaveRecords()
        {
            var currentTimeZone = TimeZone.CurrentTimeZone;
            var date = currentTimeZone.ToUniversalTime(SelectedDate.Date);

            foreach (var weatherRecord in _weatherRecords)
            {
                weatherRecord.LocationId = SelectedLocation.Id;
                weatherRecord.Location = SelectedLocation;
                weatherRecord.Created = date.Add(weatherRecord.Created.TimeOfDay);
            }

            Messenger.Send<IReadOnlyCollection<WeatherRecord>>(_weatherRecords);
            NavigationService.GoBack();
        }

        private bool CanSaveRecords()
        {
            return _weatherRecords.Count > 0;
        }

        public RelayCommand DownloadRecords
        {
            get
            {
                return _downloadRecordsCommand ?? (_downloadRecordsCommand = new RelayCommand(
                    ExecuteDownloadRecords));
            }
        }

        private void ExecuteDownloadRecords()
        {
            StartProgress();

            var from = SelectedDate.Date.ToUniversalTime();
            var to = from.AddHours(24);

            var downloadTask = Task.Run(() => _weatherService.GetWeatherData(SelectedLocation, from, to));
            downloadTask.ContinueWith(task => FinishProgress());
            downloadTask.ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled) return;

                var uniqueRecords = task.Result.Where(weatherRecord => _weatherRecords.FirstOrDefault(r => r.Created == weatherRecord.Created) == null);

                foreach (var weatherRecord in uniqueRecords)
                {
                    _weatherRecords.Add(weatherRecord);
                }
            });
            downloadTask.ContinueWith(DispatchError);
        }

        #endregion
    }
}

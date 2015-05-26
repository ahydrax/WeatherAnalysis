using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Ninject.Planning.Bindings;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Data.Sql;
using WeatherAnalysis.Core.Model;
using WeatherAnalysis.Core.Service.OpenWeather;

namespace WeatherAnalysis.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWeatherRecordManager _weatherRecordManager;
        private readonly INavigationService _navigationService;
        private readonly object _weatherRecordsSyncRoot = new object();
        public ObservableCollection<WeatherRecord> WeatherRecords { get; private set; }

        // InProgress
        public const string InProgressPropertyName = "InProgress";
        private bool _inProgress = false;
        public bool InProgress
        {
            get
            {
                return _inProgress;
            }
            set
            {
                Set(InProgressPropertyName, ref _inProgress, value);
            }
        }

        // SelectedLocation
        public const string SelectedLocationPropertyName = "SelectedLocation";
        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                Set(SelectedLocationPropertyName, ref _selectedLocation, value);
            }
        }

        // SelectedDate
        public const string SelectedDatePropertyName = "SelectedDate";
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                Set(SelectedDatePropertyName, ref _selectedDate, value);
            }
        }


        private RelayCommand _navigateToLocationSelectionCommand;
        /// <summary>
        /// Gets the NavigateToLocationSelectionCommand.
        /// </summary>
        public RelayCommand NavigateToLocationSelectionCommand
        {
            get
            {
                return _navigateToLocationSelectionCommand ?? (_navigateToLocationSelectionCommand = new RelayCommand(
                    ExecuteNavigateToLocationSelectionCommand,
                    () => true));
            }
        }

        private void ExecuteNavigateToLocationSelectionCommand()
        {
            _navigationService.NavigateTo(Dialogs.LocationSelect);
        }

        public MainViewModel(IWeatherRecordManager weatherRecordManager, INavigationService navigationService)
        {
            _weatherRecordManager = weatherRecordManager;
            _navigationService = navigationService;

            WeatherRecords = new ObservableCollection<WeatherRecord>();
            BindingOperations.EnableCollectionSynchronization(WeatherRecords, _weatherRecordsSyncRoot);


            SelectedLocation = new Location { Id = 1, Name = "Хабаровск", SystemName = "Khabarovsk" };

            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            PropertyChanged += SelectedDateChanged;
        }

        private void SelectedDateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SelectedDatePropertyName)
            {
                GetTodayWeather();   
            }
        }

        private void GetTodayWeather()
        {
            if (SelectedLocation != null)
            {
                Task.Run(() =>
                {
                    InProgress = true;
                    WeatherRecords.Clear();

                    var service = OpenWeatherService.CreateService();
                    var data = service.GetWeatherData(SelectedLocation, SelectedDate, SelectedDate.AddDays(1));
                    return data;
                }).ContinueWith(task =>
                {
                    InProgress = false;
                    
                    if (!task.IsCompleted || task.IsCanceled) return;
                    foreach (var weatherRecord in task.Result)
                    {
                        WeatherRecords.Add(weatherRecord);
                    }
                });
            }
        }
    }
}

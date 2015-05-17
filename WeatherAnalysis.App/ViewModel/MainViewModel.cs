using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Ninject.Planning.Bindings;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWeatherRecordManager _weatherRecordManager;
        private readonly INavigationService _navigationService;
        private readonly object _weatherRecordsSyncRoot = new object();
        public ObservableCollection<WeatherRecord> WeatherRecords { get; private set; }

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

            SelectedLocation = new Location {Id = 1, Name = "Хабаровск"};
            GetTodayWeather();
        }

        private void GetTodayWeather()
        {
            if (SelectedLocation != null)
            {
                Task.Run(() => _weatherRecordManager.Get(
                    SelectedLocation.Id.Value,
                    DateTime.Today,
                    DateTime.Today.AddHours(24)))
                    .ContinueWith(t =>
                    {
                        WeatherRecords.Clear();
                        foreach (var weatherRecord in t.Result)
                        {
                            WeatherRecords.Add(weatherRecord);
                        }
                    });
            }
        }
    }
}

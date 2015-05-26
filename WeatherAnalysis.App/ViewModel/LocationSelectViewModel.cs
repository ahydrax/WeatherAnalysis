using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class LocationSelectViewModel : ViewModelBase
    {
        private readonly ILocationManager _locationManager;
        private readonly INavigationService _navigationService;

        public ICollectionView Locations { get { return LocationsSource.View; } }
        public CollectionViewSource LocationsSource { get; private set; }

        // LocationsFilter
        public const string LocationsFilterPropertyName = "LocationsFilter";
        private string _locationsFilter = string.Empty;
        public string LocationsFilter
        {
            get
            {
                return _locationsFilter;
            }
            set
            {
                Set(LocationsFilterPropertyName, ref _locationsFilter, value);
            }
        }

        public LocationSelectViewModel(ILocationManager locationManager, INavigationService navigationService)
        {
            _locationManager = locationManager;
            _navigationService = navigationService;
            
            var locations = new ObservableCollection<Location>
            {
                new Location
                {
                    Id = 1,
                    Name = "Хабаровск",
                    SystemName = "Khabarovsk",
                    FireHazardReportsCount = 10,
                    WeatherRecordsCount = 100
                },
                new Location
                {
                    Id = 2,
                    Name = "Москва",
                    SystemName = "Moscow",
                    FireHazardReportsCount = 12,
                    WeatherRecordsCount = 56
                }
            };

            LocationsSource = new CollectionViewSource();
            LocationsSource.Source = locations;
            LocationsSource.Filter += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(LocationsFilter))
                {
                    args.Accepted = true;
                    return;
                }

                var item = args.Item as Location;
                if (item != null)
                {
                    args.Accepted = item.Name.Contains(LocationsFilter);
                }
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == LocationsFilterPropertyName)
                {
                    LocationsSource.View.Refresh();
                }
            };
        }

        private RelayCommand _goBackCommand;

        /// <summary>
        /// Gets the GoBackCommand.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(
                    ExecuteGoBackCommand,
                    CanExecuteGoBackCommand));
            }
        }

        private void ExecuteGoBackCommand()
        {
            _navigationService.GoBack();
        }

        private bool CanExecuteGoBackCommand()
        {
            return true;
        }
    }
}
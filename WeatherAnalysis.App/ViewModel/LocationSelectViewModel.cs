using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Communication;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class LocationSelectViewModel : BaseViewModel
    {
        private readonly ILocationManager _locationManager;

        private RelayCommand _getLocationsCommand;
        private RelayCommand _createLocationCommand;
        private RelayCommand _selectLocationCommand;
        private RelayCommand _removeLocationCommand;

        private string _locationsFilter = string.Empty;
        private Location _selectedLocation;

        private readonly ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        private readonly object _locationsSyncRoot = new object();
        
        public LocationSelectViewModel(ILocationManager locationManager, INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger)
        {
            _locationManager = locationManager;
            
            Messenger.Register<Location>(this, Channels.LocationSave, SaveLocation);

            InitializeLocationsViewSource();
            ExecuteGetLocations();
        }

        private void InitializeLocationsViewSource()
        {
            BindingOperations.EnableCollectionSynchronization(_locations, _locationsSyncRoot);
            LocationsViewSource = new CollectionViewSource { Source = _locations };
            LocationsViewSource.Filter += (sender, args) =>
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
        }

        private void SaveLocation(Location location)
        {
            var saveTask = Task.Run(() => _locationManager.Save(location));
            saveTask.ContinueWith(task => ExecuteGetLocations());
            saveTask.ContinueWith(DispatchError);
        }

        #region Properties
        public ICollectionView LocationsView { get { return LocationsViewSource.View; } }
        public CollectionViewSource LocationsViewSource { get; private set; }

        public const string SelectedLocationPropertyName = "SelectedLocation";
        public Location SelectedLocation
        {
            get { return _selectedLocation; }
            set { Set(SelectedLocationPropertyName, ref _selectedLocation, value); }
        }

        public const string LocationsFilterPropertyName = "LocationsFilter";
        public string LocationsFilter
        {
            get { return _locationsFilter; }
            set
            {
                if (_locationsFilter == value)
                    return;

                _locationsFilter = value;
                RaisePropertyChanged();
                LocationsViewSource.View.Refresh();
            }
        }
        #endregion

        #region Commands

        public RelayCommand GetLocations
        {
            get
            {
                return _getLocationsCommand ?? (_getLocationsCommand = new RelayCommand(
                    ExecuteGetLocations,
                    () => true));
            }
        }

        private void ExecuteGetLocations()
        {
            StartProgress();
            LocationsFilter = string.Empty;
            _locations.Clear();

            var locationGetTask = Task.Run(() => _locationManager.GetAll());
            locationGetTask.ContinueWith(task => FinishProgress());

            locationGetTask.ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled) return;

                foreach (var location in task.Result)
                {
                    _locations.Add(location);
                }
            });

            locationGetTask.ContinueWith(DispatchError);
        }

        public RelayCommand CreateLocation
        {
            get
            {
                return _createLocationCommand ?? (_createLocationCommand = new RelayCommand(
                    () => NavigationService.NavigateTo(Dialogs.CreateLocation)));
            }
        }

        public RelayCommand SelectLocation
        {
            get
            {
                return _selectLocationCommand ?? (_selectLocationCommand = new RelayCommand(
                    ExecuteSelectLocation,
                    () => SelectedLocation != null));
            }
        }

        private void ExecuteSelectLocation()
        {
            Messenger.Send(SelectedLocation, Channels.LocationSelect);
            NavigationService.GoBack();
        }

        public RelayCommand RemoveLocation
        {
            get
            {
                return _removeLocationCommand ?? (_removeLocationCommand = new RelayCommand(
                    ExecuteRemoveLocation,
                    () => SelectedLocation != null));
            }
        }

        private void ExecuteRemoveLocation()
        {
            var selectedLocation = SelectedLocation;

            _locations.Remove(SelectedLocation);
            SelectedLocation = null;

            var removeTask = Task.Run(() => _locationManager.Delete(selectedLocation));
            removeTask.ContinueWith(task => ExecuteGetLocations());
        }

        #endregion
    }
}

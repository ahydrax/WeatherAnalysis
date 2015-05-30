using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App.ViewModel
{
    public class CreateLocationViewModel : BaseViewModel
    {
        private string _name = string.Empty;
        private string _systemName = string.Empty;
        private RelayCommand _saveLocationCommand;

        public CreateLocationViewModel(INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger) { }

        #region Properties

        // Name
        public const string NamePropertyName = "Name";
        public string Name
        {
            get { return _name; }
            set { Set(NamePropertyName, ref _name, value); }
        }

        // SystemName
        public const string SystemNamePropertyName = "SystemName";
        public string SystemName
        {
            get { return _systemName; }
            set { Set(SystemNamePropertyName, ref _systemName, value); }
        }

        #endregion

        #region Commands

        // Save Location Command
        public RelayCommand SaveLocation
        {
            get
            {
                return _saveLocationCommand ?? (_saveLocationCommand = new RelayCommand(
                    ExecuteSaveLocation,
                    CanSaveLocation));
            }
        }

        private void ExecuteSaveLocation()
        {
            var location = new Location
            {
                Name = Name,
                SystemName = SystemName
            };

            Messenger.Send(location);
            NavigationService.GoBack();
        }

        private bool CanSaveLocation()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(SystemName);
        }

        #endregion
    }
}

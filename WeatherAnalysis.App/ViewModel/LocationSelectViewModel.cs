using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace WeatherAnalysis.App.ViewModel
{
    public class LocationSelectViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public LocationSelectViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
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
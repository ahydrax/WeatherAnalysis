using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;

namespace WeatherAnalysis.App.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private bool _inProgress;
        protected readonly INavigationService NavigationService;
        protected readonly IMessenger Messenger;
        
        protected BaseViewModel(INavigationService navigationService, IMessenger messenger)
        {
            NavigationService = navigationService;
            Messenger = messenger;
        }

        protected virtual void StartProgress()
        {
            InProgress = true;
        }
        
        protected virtual void FinishProgress()
        {
            InProgress = false;
        }

        #region Common properties
        // InProgress
        public const string InProgressPropertyName = "InProgress";
        public bool InProgress
        {
            get { return _inProgress; }
            set { Set(InProgressPropertyName, ref _inProgress, value); }
        }
        #endregion

        #region Common commands
        // Cancel command
        private RelayCommand _cancelCommand;
        public RelayCommand Cancel
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(
                    () => NavigationService.GoBack()));
            }
        }
        #endregion
    }
}

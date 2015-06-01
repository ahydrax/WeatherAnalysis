using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Navigation;

namespace WeatherAnalysis.App.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private bool _inProgress;

        protected readonly INavigationService NavigationService;
        protected readonly IMessenger Messenger;

        private RelayCommand _cancelCommand;

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

        protected void DispatchError(Task task)
        {
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted) return;
                if (t.Exception == null) return;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var exception = (t.Exception.InnerException ?? t.Exception);
                    NavigationService.NavigateTo(Dialogs.Error, exception);
                });
            });
        }

        #region Common properties

        public const string InProgressPropertyName = "InProgress";
        public bool InProgress
        {
            get { return _inProgress; }
            set { Set(InProgressPropertyName, ref _inProgress, value); }
        }

        #endregion

        #region Common commands

        public RelayCommand Cancel
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(
                    () => NavigationService.GoBack()));
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Unregister(this);
        }

        #endregion
    }
}

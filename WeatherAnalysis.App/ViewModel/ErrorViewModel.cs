using System;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using WeatherAnalysis.App.Navigation;

namespace WeatherAnalysis.App.ViewModel
{
    public class ErrorViewModel : BaseViewModel, IParameterReceiver
    {
        private Exception _error;

        public ErrorViewModel(INavigationService navigationService, IMessenger messenger)
            : base(navigationService, messenger)
        {

        }

        #region Properties

        public const string ErrorPropertyName = "Error";
        public Exception Error
        {
            get { return _error; }
            set { Set(ErrorPropertyName, ref _error, value); }
        }

        #endregion

        public void ReceiveParameter(object parameter)
        {
            var error = parameter as Exception;
            if (error == null) return;

            Error = error;
        }
    }
}

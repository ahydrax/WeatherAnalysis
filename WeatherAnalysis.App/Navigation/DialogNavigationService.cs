using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Views;
using Ninject;

namespace WeatherAnalysis.App.Navigation
{
    public class DialogNavigationService : INavigationService
    {
        private readonly IKernel _kernel;
        private readonly Dictionary<string, Type> _pages;

        public DialogNavigationService(IKernel kernel)
        {
            _kernel = kernel;
            _pages = new Dictionary<string, Type>();
        }

        public void Register(string name, Type windowType)
        {
            if (!_pages.ContainsKey(name))
            {
                _pages.Add(name, windowType);
            }
            else
            {
                Trace.WriteLine(string.Format("Name {0} is already reserved for {1} type", name, _pages[name].FullName));
            }
        }

        public void GoBack()
        {
            Application.Current.Windows.OfType<Window>().Single(w => w.IsActive).Close();
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            if (_pages.ContainsKey(pageKey))
            {
                var nextDialog = _kernel.Get(_pages[pageKey]) as Window;
                if (nextDialog == null) return;

                var nextDialogParameterReceiver = nextDialog.DataContext as IParameterReceiver;
                if (nextDialogParameterReceiver != null)
                {
                    nextDialogParameterReceiver.ReceiveParameter(parameter);
                }

                nextDialog.ShowDialog();
            }
            else
            {
                Trace.WriteLine(string.Format("Can't find window with name {0}", pageKey));
            }
        }

        public string CurrentPageKey
        {
            get { return string.Empty; }
        }
    }
}

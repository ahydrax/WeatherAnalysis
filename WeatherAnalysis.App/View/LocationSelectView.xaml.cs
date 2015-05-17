using System.Windows;
using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class LocationSelectView : Window
    {
        [Inject]
        public LocationSelectViewModel LocationSelectViewModel
        {
            set { DataContext = value; }
        }

        public LocationSelectView()
        {
            InitializeComponent();
        }
    }
}

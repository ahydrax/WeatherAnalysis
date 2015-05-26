using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class LocationSelectView
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

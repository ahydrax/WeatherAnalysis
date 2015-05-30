using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class CreateLocationView
    {
        [Inject]
        public CreateLocationViewModel CreateLocationViewModel
        {
            set { DataContext = value; }
        }

        public CreateLocationView()
        {
            InitializeComponent();
        }
    }
}

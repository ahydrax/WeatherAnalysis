using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class CreateWeatherRecordsView
    {
        [Inject]
        public CreateWeatherRecordsViewModel CreateWeatherRecordsViewModel
        {
            set { DataContext = value; }
        }

        public CreateWeatherRecordsView()
        {
            InitializeComponent();
        }
    }
}

using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class ErrorView
    {
        [Inject]
        public ErrorViewModel ErrorViewModel
        {
            set { DataContext = value; }
        }

        public ErrorView()
        {
            InitializeComponent();
        }
    }
}

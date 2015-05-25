using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class MainView
    {
        [Inject]
        public MainViewModel MainViewModel
        {
            get { return (MainViewModel) DataContext; }
            set { DataContext = value; }
        }

        public MainView()
        {
            InitializeComponent();
        }
    }
}

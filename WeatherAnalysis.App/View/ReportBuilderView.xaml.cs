using Ninject;
using WeatherAnalysis.App.ViewModel;

namespace WeatherAnalysis.App.View
{
    public partial class ReportBuilderView
    {
        [Inject]
        public ReportBuilderViewModel ReportBuilderViewModel
        {
            set { DataContext = value; }
        }

        public ReportBuilderView()
        {
            InitializeComponent();
        }
    }
}

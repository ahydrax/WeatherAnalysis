using System.Windows.Documents;
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

        public TextRange DocumentText { get; private set; }

        public ReportBuilderView()
        {
            InitializeComponent();
            DocumentText = new TextRange(FlowDocument.ContentStart, FlowDocument.ContentEnd);
        }
    }
}

using System.Windows;
using Ninject;
using WeatherAnalysis.App.Configuration;
using WeatherAnalysis.App.View;
using WeatherAnalysis.Core.Data;

namespace WeatherAnalysis.App
{
    public partial class App
    {
        public IKernel Container { get { return _container; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbm = Container.Get<IDbManager>();

            if (!dbm.HasValidSchema())
            {
                dbm.CleanUp();
                dbm.CreateSchema();
            }

            var mainView = Container.Get<MainView>();
            Current.MainWindow = mainView;
            mainView.Show();
        }

        private readonly IKernel _container = new StandardKernel(new AppModule());
    }
}

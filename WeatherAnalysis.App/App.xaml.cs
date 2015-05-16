using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using WeatherAnalysis.App.Configuration;
using WeatherAnalysis.App.View;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Model;

namespace WeatherAnalysis.App
{
    public partial class App
    {
        public IKernel Container { get { return _container; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //a();
            var mainView = Container.Get<MainView>();
            Current.MainWindow = mainView;
            mainView.Show();
        }

        private void a()
        {

            var dbMan = new DbManager("WeatherDb");

            dbMan.CleanUp();
            if (!dbMan.IsInValidState())
            {
                
                dbMan.CreateSchema();
            }

            var m = new LocationManager("WeatherDb");
            var b = new WeatherRecordManager("WeatherDb");
            var loc = new Location {Name = "Khabarovsk"};
            m.Save(loc);
            var data = new WeatherRecord {Created = DateTime.Now, Humidity = 50, Location = loc, Temperature = 20};
            b.Save(data);

        }

        private readonly IKernel _container = new StandardKernel(new AppModule());
    }
}

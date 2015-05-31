using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Ninject.Modules;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.App.View;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Data.Sql;
using WeatherAnalysis.Core.Service;
using WeatherAnalysis.Core.Service.OpenWeather;

namespace WeatherAnalysis.App.Configuration
{
    public class AppModule : NinjectModule
    {
        public static readonly string DbConfigurationName = "WeatherDb";

        public override void Load()
        {
            Kernel.Bind<IDbManager>()
                .To<DbManager>()
                .InSingletonScope()
                .WithConstructorArgument("configurationString", DbConfigurationName);

            Kernel.Bind<ILocationManager>()
                .To<LocationManager>()
                .InSingletonScope()
                .WithConstructorArgument("configurationString", DbConfigurationName);

            Kernel.Bind<IWeatherRecordManager>()
                .To<WeatherRecordManager>()
                .InSingletonScope()
                .WithConstructorArgument("configurationString", DbConfigurationName); ;

            Kernel.Bind<IFireHazardReportManager>()
                .To<FireHazardReportManager>()
                .InSingletonScope()
                .WithConstructorArgument("configurationString", DbConfigurationName);

            Kernel.Bind<IMessenger>()
                .To<Messenger>()
                .InSingletonScope();

            ConfigureWeatherService();
            ConfigureNavigationService();
        }

        private void ConfigureWeatherService()
        {
            var openWeatherService = OpenWeatherService.CreateService();
            Kernel.Bind<IWeatherService>()
                .ToConstant(openWeatherService)
                .InSingletonScope();
        }

        private void ConfigureNavigationService()
        {
            var navigationService = new DialogNavigationService(KernelInstance);
            navigationService.Register(Dialogs.Main, typeof (MainView));
            navigationService.Register(Dialogs.LocationSelect, typeof (LocationSelectView));
            navigationService.Register(Dialogs.CreateLocation, typeof (CreateLocationView));
            navigationService.Register(Dialogs.CreateWeatherRecords, typeof (CreateWeatherRecordsView));
            navigationService.Register(Dialogs.ReportBuilder, typeof(ReportBuilderView));
            navigationService.Register(Dialogs.Error, typeof(ErrorView));
            Kernel.Bind<INavigationService>().ToConstant(navigationService);
        }
    }
}

using GalaSoft.MvvmLight.Views;
using Ninject.Modules;
using WeatherAnalysis.App.Navigation;
using WeatherAnalysis.App.View;
using WeatherAnalysis.Core.Data;
using WeatherAnalysis.Core.Data.Sql;

namespace WeatherAnalysis.App.Configuration
{
    public class AppModule : NinjectModule
    {
        public static readonly string DbConfigurationName = "WeatherDb";

        public override void Load()
        {
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

            var navigationService = new DialogNavigationService(KernelInstance);
            navigationService.Register(Dialogs.Main, typeof(MainView));
            navigationService.Register(Dialogs.LocationSelect, typeof(LocationSelectView));
            Kernel.Bind<INavigationService>().ToConstant(navigationService);
        }
    }
}

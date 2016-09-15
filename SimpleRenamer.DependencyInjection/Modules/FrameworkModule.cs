using Ninject.Modules;
using SimpleRenamer.Framework;
using SimpleRenamer.Framework.Interface;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class FrameworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<IBackgroundQueue>().To<BackgroundQueue>().InSingletonScope();
            Bind<IConfigurationManager>().To<AppConfigurationManager>().InSingletonScope();
            Bind<IHelper>().To<Helper>().InSingletonScope();
        }
    }
}

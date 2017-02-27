using Ninject.Modules;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Logging;

namespace Sarjee.SimpleRenamer.DependencyInjection.Modules
{
    public class FrameworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<IRetryHelper>().To<RetryHelper>().InSingletonScope();
            Bind<IBackgroundQueue>().To<BackgroundQueue>().InSingletonScope();
            Bind<IConfigurationManager>().To<AppConfigurationManager>().InSingletonScope();
            Bind<IHelper>().To<Helper>().InSingletonScope();
        }
    }
}

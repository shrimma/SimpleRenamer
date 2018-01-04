using Ninject.Modules;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Logging;

namespace Sarjee.SimpleRenamer.DependencyInjection.Modules
{
    /// <summary>
    /// Framework Module
    /// </summary>
    /// <seealso cref="Ninject.Modules.NinjectModule" />
    public class FrameworkModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<IBackgroundQueue>().To<BackgroundQueue>().InSingletonScope();
            Bind<IConfigurationManager>().To<AppConfigurationManager>().InSingletonScope();
            Bind<IHelper>().To<Helper>().InSingletonScope();
            Bind<IMessageSender>().To<EventHubSender>().InTransientScope();
        }
    }
}

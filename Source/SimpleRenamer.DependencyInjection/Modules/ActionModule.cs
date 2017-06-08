using Ninject.Modules;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Framework.TV;

namespace Sarjee.SimpleRenamer.DependencyInjection.Modules
{
    /// <summary>
    /// Action Module
    /// </summary>
    /// <seealso cref="Ninject.Modules.NinjectModule" />
    public class ActionModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<IFileMover>().To<FileMover>().InSingletonScope();
            Bind<IActionMatchedFiles>().To<ActionMatchedFiles>().InSingletonScope();
            Bind<IBannerDownloader>().To<BannerDownloader>().InSingletonScope();
        }
    }
}
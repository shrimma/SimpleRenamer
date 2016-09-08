using Ninject.Modules;
using SimpleRenamer.Framework;
using SimpleRenamer.Framework.Interface;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class ActionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMover>().To<FileMover>();
            Bind<IBannerDownloader>().To<BannerDownloader>();
            Bind<IIgnoreListFramework>().To<IgnoreListFramework>();
        }
    }
}

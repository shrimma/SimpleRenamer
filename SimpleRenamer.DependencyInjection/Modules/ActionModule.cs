using Ninject.Modules;
using SimpleRenamer.Framework;
using SimpleRenamer.Framework.Interface;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class ActionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMover>().To<FileMover>().InSingletonScope();
            Bind<IBannerDownloader>().To<BannerDownloader>().InSingletonScope();
            Bind<IPerformActionsOnShows>().To<ActionMatchedFiles>().InSingletonScope();
            Bind<IGetShowDetails>().To<GetShowDetails>().InSingletonScope();
        }
    }
}

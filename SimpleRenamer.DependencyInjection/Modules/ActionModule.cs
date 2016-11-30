using Ninject.Modules;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.Framework.Core;
using SimpleRenamer.Framework.Movie;
using SimpleRenamer.Framework.TV;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class ActionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMover>().To<FileMover>().InSingletonScope();
            Bind<IActionMatchedFiles>().To<ActionMatchedFiles>().InSingletonScope();
            Bind<IBannerDownloader>().To<BannerDownloader>().InSingletonScope();
            Bind<IGetShowDetails>().To<GetShowDetails>().InSingletonScope();
            Bind<IGetMovieDetails>().To<GetMovieDetails>().InSingletonScope();
        }
    }
}

using Ninject.Modules;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.Framework.TV;

namespace Sarjee.SimpleRenamer.DependencyInjection.Modules
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

using Ninject.Modules;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.Framework.TV;

namespace Sarjee.SimpleRenamer.DependencyInjection.Modules
{
    public class ScanModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMatcher>().To<FileMatcher>().InSingletonScope();
            Bind<IFileWatcher>().To<FileWatcher>().InSingletonScope();
            Bind<ITVShowMatcher>().To<TVShowMatcher>().InSingletonScope();
            Bind<IMovieMatcher>().To<MovieMatcher>().InSingletonScope();
            Bind<IScanFiles>().To<ScanFiles>().InSingletonScope();
            Bind<ITmdbManager>().To<TmdbManager>().InSingletonScope();
            Bind<ITvdbManager>().To<TvdbManager>().InSingletonScope();
        }
    }
}

using Ninject.Modules;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.Framework.Core;
using SimpleRenamer.Framework.Movie;
using SimpleRenamer.Framework.TV;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class ScanModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMatcher>().To<FileMatcher>().InSingletonScope();
            Bind<IFileWatcher>().To<FileWatcher>().InSingletonScope();
            Bind<ITVShowMatcher>().To<TVShowMatcher>().InSingletonScope();
            Bind<IMovieMatcher>().To<MovieMatcher>().InSingletonScope();
            Bind<IScanForShows>().To<ScanFiles>().InSingletonScope();
            Bind<ITmdbManager>().To<TmdbManager>().InSingletonScope();
            Bind<ITvdbManager>().To<TvdbManager>().InSingletonScope();
        }
    }
}

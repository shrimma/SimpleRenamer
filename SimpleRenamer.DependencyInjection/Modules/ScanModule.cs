using Ninject.Modules;
using SimpleRenamer.Framework;
using SimpleRenamer.Framework.Interface;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class ScanModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileMatcher>().To<FileMatcher>().InSingletonScope();
            Bind<IFileWatcher>().To<FileWatcher>().InSingletonScope();
            Bind<ITVShowMatcher>().To<TVShowMatcher>().InSingletonScope();
        }
    }
}

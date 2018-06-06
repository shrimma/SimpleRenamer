using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.DependencyInjection;
using Sarjee.SimpleRenamer.Framework.Core;
using System.Windows;

namespace Sarjee.SimpleRenamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IDependencyInjectionContext _injectionContext;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            _injectionContext = new DependencyInjectionContext();
            _injectionContext.Initialize();
            _injectionContext.BindConstant<IConfigurationManager>(new JotConfigurationManager());
        }

        private void ComposeObjects()
        {
            Current.MainWindow = new MainWindow(_injectionContext.GetService<ILogger>(), _injectionContext.GetService<ITVShowMatcher>(), _injectionContext.GetService<IMovieMatcher>(), _injectionContext, _injectionContext.GetService<IActionMatchedFiles>(), _injectionContext.GetService<IScanFiles>(), _injectionContext.GetService<IConfigurationManager>());
        }  
    }
}

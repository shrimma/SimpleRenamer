using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.DependencyInjection;
using System;
using System.Windows;

namespace Sarjee.SimpleRenamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private IDependencyInjectionContext injection;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            injection = new DependencyInjectionContext();
            injection.Initialize();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = this.injection.GetService<MainWindow>();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (injection != null)
                    {
                        injection.Dispose();
                        injection = null;
                    }
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

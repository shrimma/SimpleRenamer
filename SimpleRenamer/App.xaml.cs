﻿using SimpleRenamer.DependencyInjection;
using SimpleRenamer.Framework.Interface;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
    }
}

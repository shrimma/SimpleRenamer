﻿using Ninject.Modules;
using SimpleRenamer.Common;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Framework;
using SimpleRenamer.Framework.Core;

namespace SimpleRenamer.DependencyInjection.Modules
{
    public class FrameworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<IRetryHelper>().To<RetryHelper>().InSingletonScope();
            Bind<IBackgroundQueue>().To<BackgroundQueue>().InSingletonScope();
            Bind<IConfigurationManager>().To<AppConfigurationManager>().InSingletonScope();
            Bind<IHelper>().To<Helper>().InSingletonScope();
        }
    }
}

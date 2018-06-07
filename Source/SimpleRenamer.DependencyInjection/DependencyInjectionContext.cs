using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.Framework.TV;
using Sarjee.SimpleRenamer.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sarjee.SimpleRenamer.DependencyInjection
{
    /// <summary>
    /// Dependency Injection Context
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IDependencyInjectionContext" />
    /// <seealso cref="System.IDisposable" />
    public class DependencyInjectionContext : IDependencyInjectionContext
    {
        protected IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;
        private readonly RetryPolicy<HttpResponseMessage> _transientRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2)
            });

        /// <summary>
        /// Starts the process of setting up dependency injection
        /// </summary>
        public void Initialize()
        {            
            _serviceCollection = new ServiceCollection();            
            AddActionDependencies();
            AddFrameworkDependencies();
            AddScanDependencies();            
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <returns>
        /// The service if it exists
        /// </returns>
        /// <inheritdoc />
        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Bind a context to a constant
        /// </summary>
        /// <typeparam name="T">Type to bind to</typeparam>
        /// <param name="context">Context to use for the constant binding</param>
        public void BindConstant<T>(T context)
        {
            BindConstant(new Func<T>(() => context));
        }

        /// <summary>
        /// Binds the constant using a factory (lazy instantiation)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazyContextFactory">The context factory - only called if type not bound.</param>
        private void BindConstant<T>(Func<T> lazyContextFactory)
        {
            lock (_serviceCollection)
            {
                //check if there is a singleton instance of the same type already
                ServiceDescriptor existingServiceDescriptor = _serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(T) && x.Lifetime == ServiceLifetime.Singleton && x.ImplementationInstance != null);
                if (existingServiceDescriptor == null)
                {
                    //this binds this instance as singleton
                    _serviceCollection.AddSingleton(typeof(T), lazyContextFactory.Invoke());
                    _serviceProvider = _serviceCollection.BuildServiceProvider();
                }
            }
        }

        /// <summary>
        /// Releases the service.
        /// </summary>
        /// <param name="serviceToRelease">The service to release.</param>
        public void ReleaseService(object serviceToRelease)
        {
            lock (_serviceCollection)
            {
                ServiceDescriptor existingServiceDescriptor = _serviceCollection.FirstOrDefault(x => x.ServiceType == serviceToRelease.GetType());
                if (existingServiceDescriptor != null)
                {
                    _serviceCollection.Remove(existingServiceDescriptor);
                }
            }
        }        

        private void AddActionDependencies()
        {
            _serviceCollection.AddSingleton<IFileMover, FileMover>();
            _serviceCollection.AddSingleton<IActionMatchedFiles, ActionMatchedFiles>();
            _serviceCollection.AddSingleton<IBannerDownloader, BannerDownloader>();
        }

        private void AddFrameworkDependencies()
        {
            _serviceCollection.AddSingleton<ILogger, Logger>();
            _serviceCollection.AddSingleton<IBackgroundQueue, BackgroundQueue>();
            _serviceCollection.AddSingleton<IHelper, Helper>();
            _serviceCollection.AddTransient<IMessageSender, ServiceBusSender>();
        }

        private void AddScanDependencies()
        {            
            _serviceCollection.AddOptions();
            _serviceCollection.AddLazyCache();
            _serviceCollection.AddHttpClient<ITvdbClient, TvdbClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.thetvdb.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(_transientRetryPolicy);
            _serviceCollection.AddHttpClient<ITmdbClient, TmdbClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(_transientRetryPolicy);
            _serviceCollection.AddSingleton<IFileMatcher, FileMatcher>();
            _serviceCollection.AddSingleton<IFileWatcher, FileWatcher>();
            _serviceCollection.AddSingleton<ITVShowMatcher, TVShowMatcher>();
            _serviceCollection.AddSingleton<IMovieMatcher, MovieMatcher>();
            _serviceCollection.AddSingleton<IScanFiles, ScanFiles>();
            _serviceCollection.AddSingleton<ITmdbManager, TmdbManager>();
            _serviceCollection.AddSingleton<ITvdbManager, TvdbManager>();
        }
    }
}

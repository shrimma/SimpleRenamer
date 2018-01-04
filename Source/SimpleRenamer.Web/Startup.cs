using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.Framework.TV;
using Sarjee.SimpleRenamer.Logging;

namespace Sarjee.SimpleRenamer.Web
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
            services.AddSingleton<IConfigurationManager, AppConfigurationManager>();
            services.AddSingleton<IHelper, Helper>();

            services.AddSingleton<IFileMatcher, FileMatcher>();
            services.AddSingleton<IFileWatcher, FileWatcher>();
            services.AddSingleton<ITVShowMatcher, TVShowMatcher>();
            services.AddSingleton<IMovieMatcher, MovieMatcher>();
            services.AddSingleton<IScanFiles, ScanFiles>();
            services.AddSingleton<ITmdbManager, TmdbManager>();
            services.AddSingleton<ITvdbManager, TvdbManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

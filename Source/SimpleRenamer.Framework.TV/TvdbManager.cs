using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// TVDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.ITvdbManager" />
    public class TvdbManager : ITvdbManager
    {                
        private readonly IHelper _helper;
        private readonly ITvdbClient _tvdbClient;        

        /// <summary>
        /// Initializes a new instance of the <see cref="TvdbManager" /> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper">The helper.</param>
        /// <exception cref="ArgumentNullException">
        
        /// </exception>       
        public TvdbManager(IHelper helper, ITvdbClient tvdbClient)
        {            
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _tvdbClient = tvdbClient ?? throw new ArgumentNullException(nameof(tvdbClient));
        }                

        /// <summary>
        /// Gets the banner URI.
        /// </summary>
        /// <param name="bannerPath">The banner path.</param>
        /// <returns></returns>
        public string GetBannerUri(string bannerPath)
        {
            if (string.IsNullOrWhiteSpace(bannerPath))
            {
                throw new ArgumentNullException(nameof(bannerPath));
            }

            return string.Format("http://thetvdb.com/banners/{0}", bannerPath);
        }

        /// <summary>
        /// Gets the series by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">tmdbId</exception>
        public async Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tmdbId))
            {
                throw new ArgumentNullException(nameof(tmdbId));
            }

            //get series
            Task<SeriesData> getSeriesTask = _tvdbClient.GetSeriesAsync(tmdbId, cancellationToken);
            //get actors
            Task<SeriesActors> getActorsTask = _tvdbClient.GetActorsAsync(tmdbId, cancellationToken);
            //wait for series and actors
            await Task.WhenAll(getSeriesTask, getActorsTask);

            //get episodes                        
            Task<SeriesEpisodes> getEpisodesTask = _tvdbClient.GetEpisodesAsync(tmdbId, cancellationToken);
            //get series posters            
            Task<SeriesImageQueryResults> getSeriesPostersTask = _tvdbClient.GetSeriesPostersAsync(tmdbId, cancellationToken);
            //wait for episodes and series posters
            await Task.WhenAll(getEpisodesTask, getSeriesPostersTask);

            //get season specific posters                        
            Task<SeriesImageQueryResults> getSeasonPostersTask = _tvdbClient.GetSeasonPostersAsync(tmdbId, cancellationToken);
            //get series banners            
            Task<SeriesImageQueryResults> getSeriesBannersTask = _tvdbClient.GetSeriesBannersAsync(tmdbId, cancellationToken);
            //wait for season and series posters
            await Task.WhenAll(getSeasonPostersTask, getSeasonPostersTask);

            //get results from all tasks            
            SeriesData series = getSeriesTask.Result;
            SeriesActors actors = getActorsTask.Result;
            SeriesEpisodes episodes = getEpisodesTask.Result;
            SeriesImageQueryResults seriesPosters = getSeriesPostersTask.Result;
            SeriesImageQueryResults seasonPosters = getSeasonPostersTask.Result;
            SeriesImageQueryResults seriesBanners = getSeriesBannersTask.Result;

            if (series != null)
            {
                return new CompleteSeries(series?.Data, actors?.Data, episodes?.Data, seriesPosters?.Data, seasonPosters?.Data, seriesBanners?.Data);
            }
            else
            {
                throw new InvalidOperationException("Unable to retrieve all required data");
            }
        }        

        /// <summary>
        /// Searches the series by name asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">seriesName</exception>
        public Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(seriesName))
            {
                throw new ArgumentNullException(nameof(seriesName));
            }

            return _tvdbClient.SearchSeriesByNameAsync(seriesName, cancellationToken);            
        }
    }
}

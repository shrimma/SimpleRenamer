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
        private readonly ITvdbClient _tvdbClient;
        private const string _bannerBaseUri = "http://thetvdb.com/banners/";

        /// <summary>
        /// Initializes a new instance of the <see cref="TvdbManager" /> class.
        /// </summary>
        /// <param name="tvdbClient">The TVDB client.</param>
        /// <exception cref="ArgumentNullException">tvdbClient</exception>
        public TvdbManager(ITvdbClient tvdbClient)
        {                        
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

            return string.Concat(_bannerBaseUri, bannerPath);
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
            Task<SeriesImageQueryResults> getSeriesPostersTask = QuerySeriesImagesAsync(tmdbId, "posters", cancellationToken);
            //wait for episodes and series posters
            await Task.WhenAll(getEpisodesTask, getSeriesPostersTask);

            //get season specific posters                        
            Task<SeriesImageQueryResults> getSeasonPostersTask = QuerySeriesImagesAsync(tmdbId, "season", cancellationToken);
            //get series banners            
            Task<SeriesImageQueryResults> getSeriesBannersTask = QuerySeriesImagesAsync(tmdbId, "series", cancellationToken);
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
        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(seriesName))
            {
                throw new ArgumentNullException(nameof(seriesName));
            }

            SeriesSearchDataList searchData = await _tvdbClient.SearchSeriesByNameAsync(seriesName, cancellationToken);
            if (searchData?.SearchResults != null)
            {
                return searchData.SearchResults;
            }
            else
            {
                throw new InvalidOperationException($"Unable to find any results for {seriesName}.");
            }            
        }        

        private async Task<SeriesImageQueryResults> QuerySeriesImagesAsync(string tmdbId, string imageKeyType, CancellationToken cancellationToken)
        {
            SeriesImageQueryResults results = await _tvdbClient.QuerySeriesImagesAsync(tmdbId, imageKeyType, cancellationToken);
            if (results == null)
            {
                return new SeriesImageQueryResults();
            }
            else
            {
                return results;
            }
        }
    }
}

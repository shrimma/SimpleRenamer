using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// TVDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.ITvdbManager" />
    public class TvdbManager : ITvdbManager
    {
        private string _apiKey;
        private int _maxRetryCount = 10;
        private int _maxBackoffSeconds = 2;
        private IHelper _helper;
        private IRestClient _restClient;
        private JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvdbManager" /> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper">The helper.</param>
        /// <exception cref="ArgumentNullException">
        /// configManager
        /// orW
        /// helper
        /// </exception>       
        public TvdbManager(IConfigurationManager configManager, IHelper helper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            _apiKey = configManager.TvDbApiKey;
            _restClient = new RestClient("https://api.thetvdb.com");
            _restClient.AddDefaultHeader("content-type", "application/json");
            _jsonSerializerSettings = new JsonSerializerSettings { Error = HandleDeserializationError };
        }

        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            //TODO log the error
            //errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        /// <summary>
        /// Logs into the API.
        /// </summary>
        /// <returns></returns>
        private async Task Login()
        {
            //create rest request
            Auth auth = new Auth()
            {
                Apikey = _apiKey
            };
            IRestRequest request = new RestRequest("login", Method.POST);
            request.AddParameter("application/json", auth.ToJson(), ParameterType.RequestBody);

            //execute the request
            Token token = await _helper.ExecuteRestRequest<Token>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
            if (token != null)
            {
                _restClient.Authenticator = new JwtAuthenticator(token._Token);
            }
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

            return $"http://thetvdb.com/banners/{bannerPath}";
        }

        /// <summary>
        /// Gets the series by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">tmdbId</exception>
        public async Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId)
        {
            if (string.IsNullOrWhiteSpace(tmdbId))
            {
                throw new ArgumentNullException(nameof(tmdbId));
            }

            if (_restClient.Authenticator == null)
            {
                await Login();
            }

            //get series
            Task<SeriesData> getSeriesTask = GetSeries(tmdbId);
            //get actors
            Task<SeriesActors> getActorsTask = GetActors(tmdbId);
            //wait for series and actors
            await Task.WhenAll(getSeriesTask, getActorsTask);

            //get episodes                        
            Task<SeriesEpisodes> getEpisodesTask = GetEpisodes(tmdbId);
            //get series posters            
            Task<SeriesImageQueryResults> getSeriesPostersTask = GetSeriesPosters(tmdbId);
            //wait for episodes and series posters
            await Task.WhenAll(getEpisodesTask, getSeriesPostersTask);

            //get season specific posters                        
            Task<SeriesImageQueryResults> getSeasonPostersTask = GetSeasonPosters(tmdbId);
            //get series banners            
            Task<SeriesImageQueryResults> getSeriesBannersTask = GetSeriesBanners(tmdbId);
            //wait for season and series posters
            await Task.WhenAll(getSeasonPostersTask, getSeasonPostersTask);

            //get results from all tasks            
            SeriesData series = getSeriesTask.Result;
            SeriesActors actors = getActorsTask.Result;
            SeriesEpisodes episodes = getEpisodesTask.Result;
            SeriesImageQueryResults seriesPosters = getSeriesPostersTask.Result;
            SeriesImageQueryResults seasonPosters = getSeasonPostersTask.Result;
            SeriesImageQueryResults seriesBanners = getSeriesBannersTask.Result;

            if (series != null && actors != null && episodes != null && seriesPosters != null && seasonPosters != null && seriesBanners != null)
            {
                return new CompleteSeries(series.Data, actors.Data, episodes.Data, seriesPosters.Data, seasonPosters.Data, seriesBanners.Data);
            }
            else
            {
                //TODO this should throw
                return null;
            }
        }

        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesData> GetSeries(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"series/{tmdbId}", Method.GET);

            //execute the request
            return await _helper.ExecuteRestRequest<SeriesData>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
        }

        /// <summary>
        /// Gets the actors.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesActors> GetActors(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/series/{tmdbId}/actors", Method.GET);

            //execute the request
            return await _helper.ExecuteRestRequest<SeriesActors>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
        }

        /// <summary>
        /// Gets the episodes.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesEpisodes> GetEpisodes(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/series/{tmdbId}/episodes", Method.GET);

            //execute the request
            return await _helper.ExecuteRestRequest<SeriesEpisodes>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
        }

        /// <summary>
        /// Gets the series posters.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeriesPosters(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
            request.AddParameter("keyType", "poster", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await _helper.ExecuteRestRequest<SeriesImageQueryResults>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
            if (results == null)
            {
                results = new SeriesImageQueryResults();
            }
            return results;
        }

        /// <summary>
        /// Gets the season posters.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeasonPosters(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
            request.AddParameter("keyType", "season", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await _helper.ExecuteRestRequest<SeriesImageQueryResults>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
            if (results == null)
            {
                results = new SeriesImageQueryResults();
            }
            return results;
        }

        /// <summary>
        /// Gets the series banners.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeriesBanners(string tmdbId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
            request.AddParameter("keyType", "series", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await _helper.ExecuteRestRequest<SeriesImageQueryResults>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
            if (results == null)
            {
                results = new SeriesImageQueryResults();
            }
            return results;
        }

        /// <summary>
        /// Searches the series by name asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">seriesName</exception>
        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName)
        {
            if (string.IsNullOrWhiteSpace(seriesName))
            {
                throw new ArgumentNullException(nameof(seriesName));
            }

            //login if this is the first call
            if (_restClient.Authenticator == null)
            {
                await Login();
            }

            //create the request
            IRestRequest request = new RestRequest("/search/series", Method.GET);
            request.AddParameter("name", seriesName, ParameterType.QueryString);

            //execute the request
            SeriesSearchDataList searchData = await _helper.ExecuteRestRequest<SeriesSearchDataList>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, Login);
            if (searchData?.SearchResults != null)
            {
                return searchData.SearchResults;
            }
            else
            {
                return null;
            }
        }
    }
}

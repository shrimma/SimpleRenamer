using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Helpers;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private string _jwtToken;
        private int _maxRetryCount = 10;
        private int _maxBackoffSeconds = 2;
        private IHelper _helper;
        private IRestClient _restClient;
        private JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="retryHelper">The retry helper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// retryHelper
        /// </exception>
        public TvdbManager(IConfigurationManager configManager, IHelper helper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            _apiKey = configManager.TvDbApiKey;
            _jwtToken = string.Empty;
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

        private int[] httpStatusCodesWorthRetrying = { 408, 500, 502, 503, 504, 598, 599 };
        private async Task<T> ExecuteRestRequest<T>(IRestRequest restRequest, Func<Task> LoginCallback) where T : class
        {
            int currentRetry = 0;
            int offset = ThreadLocalRandom.Instance.Next(100, 500);
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    //execute the request
                    IRestResponse response = await _restClient.ExecuteTaskAsync(restRequest);
                    //if no errors and statuscode ok then deserialize the response
                    if (response?.ErrorException == null && response?.StatusCode == HttpStatusCode.OK)
                    {
                        T result = JsonConvert.DeserializeObject<T>(response.Content, _jsonSerializerSettings);
                        return result;
                    }
                    //if status code indicates transient error then throw timeoutexception
                    else if (httpStatusCodesWorthRetrying.Contains((int)response?.StatusCode))
                    {
                        throw new TimeoutException();
                    }
                    //if status code indicates unauthorized then throw unauthorized exception
                    else if (response?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    //else throw the responses exception
                    else
                    {
                        throw response.ErrorException;
                    }
                }
                catch (TimeoutException)
                {
                    currentRetry++;
                    await _helper.ExponentialDelayAsync(offset, currentRetry, _maxBackoffSeconds);
                }
                catch (WebException)
                {
                    currentRetry++;
                    await _helper.ExponentialDelayAsync(offset, currentRetry, _maxBackoffSeconds);
                }
                catch (UnauthorizedAccessException)
                {
                    currentRetry++;
                    if (LoginCallback != null)
                    {
                        await LoginCallback();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Logins this instance.
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
            Token token = await ExecuteRestRequest<Token>(request, Login);
            if (token != null)
            {
                _jwtToken = token._Token;
            }
        }

        /// <summary>
        /// Gets the banner URI.
        /// </summary>
        /// <param name="bannerPath">The banner path.</param>
        /// <returns></returns>
        public string GetBannerUri(string bannerPath)
        {
            return $"http://thetvdb.com/banners/{bannerPath}";
        }

        /// <summary>
        /// Gets the series by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        public async Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId)
        {
            if (string.IsNullOrEmpty(_jwtToken))
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");

            //execute the request
            return await ExecuteRestRequest<SeriesData>(request, Login);
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");

            //execute the request
            return await ExecuteRestRequest<SeriesActors>(request, Login);
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");

            //execute the request
            return await ExecuteRestRequest<SeriesEpisodes>(request, Login);
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");
            request.AddParameter("keyType", "poster", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await ExecuteRestRequest<SeriesImageQueryResults>(request, Login);
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");
            request.AddParameter("keyType", "season", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await ExecuteRestRequest<SeriesImageQueryResults>(request, Login);
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
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");
            request.AddParameter("keyType", "series", ParameterType.QueryString);

            //execute the request
            SeriesImageQueryResults results = await ExecuteRestRequest<SeriesImageQueryResults>(request, Login);
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
        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName)
        {
            //login if this is the first call
            if (string.IsNullOrEmpty(_jwtToken))
            {
                await Login();
            }

            //create the request
            IRestRequest request = new RestRequest("/search/series", Method.GET);
            request.AddHeader("Authorization", $"Bearer {_jwtToken}");
            request.AddParameter("name", seriesName, ParameterType.QueryString);

            //execute the request
            return await ExecuteRestRequest<List<SeriesSearchData>>(request, Login);
        }
    }
}

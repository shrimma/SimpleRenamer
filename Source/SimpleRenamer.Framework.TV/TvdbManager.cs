using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
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
        private string _jwtToken;
        private int _maxRetryCount = 10;
        private int _maxBackoffSeconds = 2;
        private IRetryHelper _retryHelper;
        private RestClient _restClient;
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
        public TvdbManager(IConfigurationManager configManager, IRetryHelper retryHelper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            _apiKey = configManager.TvDbApiKey;
            _jwtToken = "";
            _retryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
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
        /// Logins this instance.
        /// </summary>
        /// <returns></returns>
        private async Task Login()
        {
            Auth auth = new Auth()
            {
                Apikey = _apiKey
            };

            //create rest request
            var request = new RestRequest("login", Method.POST);
            request.AddParameter("application/json", auth.ToJson(), ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Token token = JsonConvert.DeserializeObject<Token>(response.Content, _jsonSerializerSettings);
                _jwtToken = token._Token;
            }
            else
            {
                //should we throw here?
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

            //setup the datablock
            Task<SeriesData> getSeriesTask = GetSeries(tmdbId);

            //get actors
            Task<SeriesActors> getActorsTask = GetActors(tmdbId);

            //get episodes                        
            Task<SeriesEpisodes> getEpisodesTask = GetEpisodes(tmdbId);

            //get series posters            
            Task<SeriesImageQueryResults> getSeriesPostersTask = GetSeriesPosters(tmdbId);

            //get season specific posters                        
            Task<SeriesImageQueryResults> getSeasonPostersTask = GetSeasonPosters(tmdbId);

            //get series banners            
            Task<SeriesImageQueryResults> getSeriesBannersTask = GetSeriesBanners(tmdbId);

            //await completion of all tasks
            Task[] getTasks = new Task[] { getSeriesTask, getActorsTask, getEpisodesTask, getSeriesPostersTask, getSeasonPostersTask, getSeriesBannersTask };
            await Task.WhenAll(getTasks);
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
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"series/{tmdbId}", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SeriesData series = JsonConvert.DeserializeObject<SeriesData>(response.Content, _jsonSerializerSettings);
                        return series;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the actors.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesActors> GetActors(string tmdbId)
        {
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"/series/{tmdbId}/actors", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SeriesActors actors = JsonConvert.DeserializeObject<SeriesActors>(response.Content, _jsonSerializerSettings);
                        return actors;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the episodes.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesEpisodes> GetEpisodes(string tmdbId)
        {
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"/series/{tmdbId}/episodes", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        SeriesEpisodes episodes = JsonConvert.DeserializeObject<SeriesEpisodes>(response.Content, _jsonSerializerSettings);
                        return episodes;

                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the series posters.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeriesPosters(string tmdbId)
        {
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    request.AddParameter("keyType", "poster", ParameterType.QueryString);
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SeriesImageQueryResults posters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content, _jsonSerializerSettings);
                        return posters;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        //not found
                        return new SeriesImageQueryResults();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return new SeriesImageQueryResults();
        }

        /// <summary>
        /// Gets the season posters.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeasonPosters(string tmdbId)
        {
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    request.AddParameter("keyType", "season", ParameterType.QueryString);
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SeriesImageQueryResults posters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content, _jsonSerializerSettings);
                        return posters;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        //not found
                        return new SeriesImageQueryResults();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return new SeriesImageQueryResults();
        }

        /// <summary>
        /// Gets the series banners.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        private async Task<SeriesImageQueryResults> GetSeriesBanners(string tmdbId)
        {
            int currentRetry = 0;
            int offset = 50;
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest($"/series/{tmdbId}/images/query", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    request.AddParameter("keyType", "series", ParameterType.QueryString);
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SeriesImageQueryResults posters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content, _jsonSerializerSettings);
                        return posters;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        //not found
                        return new SeriesImageQueryResults();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return new SeriesImageQueryResults();
        }

        /// <summary>
        /// Searches the series by name asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <returns></returns>
        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName)
        {
            int currentRetry = 0;
            int offset = 50;
            if (string.IsNullOrEmpty(_jwtToken))
            {
                await Login();
            }

            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    RestRequest request = new RestRequest("/search/series", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {_jwtToken}");
                    request.AddParameter("name", seriesName, ParameterType.QueryString);
                    IRestResponse response = await _restClient.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SearchData data = JsonConvert.DeserializeObject<SearchData>(response.Content, _jsonSerializerSettings);
                        return data.Series;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await Login();
                    currentRetry++;
                    //TODO exponential backoff
                }
                catch (Exception)
                {
                    currentRetry++;
                    //TODO exponential backoff
                }
            }
            return null;
        }
    }
}

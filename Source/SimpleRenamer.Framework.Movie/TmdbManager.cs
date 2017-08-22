using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Helpers;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    /// <summary>
    /// TMDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Interface.ITmdbManager" />
    public class TmdbManager : ITmdbManager
    {
        private string _apiKey;
        private string _posterBaseUri;
        private int _maxRetryCount = 10;
        private int _maxBackoffSeconds = 2;
        private IRestClient _restClient;
        private IHelper _helper;
        private JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TmdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="retryHelper">The retry helper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// retryHelper
        /// </exception>
        public TmdbManager(IConfigurationManager configManager, IHelper helper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            _apiKey = configManager.TmDbApiKey;
            _restClient = new RestClient("https://api.themoviedb.org");
            _restClient.AddDefaultHeader("content-type", "application/json");
            _jsonSerializerSettings = new JsonSerializerSettings { Error = HandleDeserializationError };
        }

        /// <summary>
        /// Handles the deserialization error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="errorArgs">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            //TODO log the error
            //errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>virtual method for testability</remarks>
        protected virtual async Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            return await _restClient.ExecuteTaskAsync(request);
        }

        private int[] httpStatusCodesWorthRetrying = { 408, 500, 502, 503, 504, 598, 599 };
        private async Task<T> ExecuteRestRequest<T>(IRestRequest restRequest) where T : class
        {
            int currentRetry = 0;
            int offset = ThreadLocalRandom.Instance.Next(100, 500);
            while (currentRetry < _maxRetryCount)
            {
                try
                {
                    //execute the request
                    IRestResponse response = await ExecuteRequestAsync(restRequest);
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
                    //else throw the responses exception
                    else
                    {
                        if (response.ErrorException != null)
                        {
                            throw response.ErrorException;
                        }
                        //if no exception then do nothing and return null
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
            }
            return null;
        }

        /// <summary>
        /// Searches the movie by name asynchronous.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <param name="movieYear">The movie year.</param>
        /// <returns></returns>
        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int? movieYear = null)
        {
            return await GetSearchContainerMoviesAsync(movieName, movieYear);
        }
        private async Task<SearchContainer<SearchMovie>> GetSearchContainerMoviesAsync(string movieName, int? movieYear)
        {
            string resource = string.Empty;
            //if no movie year then don't include in the query
            if (!movieYear.HasValue)
            {
                resource = $"/3/search/movie?&query={movieName}&language=en-US&api_key={_apiKey}";
            }
            else
            {
                resource = $"/3/search/movie?year={movieYear}&query={movieName}&language=en-US&api_key={_apiKey}";
            }

            //create the request
            IRestRequest request = new RestRequest(resource, Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            return await ExecuteRestRequest<SearchContainer<SearchMovie>>(request);
        }

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId)
        {
            //spawn tasks for getting movie and credit info
            Task<Common.Movie.Model.Movie> movieTask = GetMovieDetailsAsync(movieId);
            Task<Credits> creditsTask = GetCreditsAsync(movieId);

            //wait for the tasks to complete
            await Task.WhenAll(movieTask, creditsTask);

            Common.Movie.Model.Movie movie = movieTask.Result;
            if (movie != null)
            {
                movie.Credits = creditsTask.Result;
            }

            return movie;
        }

        private async Task<Common.Movie.Model.Movie> GetMovieDetailsAsync(string movieId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/3/movie/{movieId}?api_key={_apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            //execute the request
            return await ExecuteRestRequest<Common.Movie.Model.Movie>(request);
        }

        private async Task<Credits> GetCreditsAsync(string movieId)
        {
            //create the request
            IRestRequest request = new RestRequest($"/3/movie/{movieId}/credits?api_key={_apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            //execute the request
            return await ExecuteRestRequest<Credits>(request);
        }

        /// <summary>
        /// Searches the movie by identifier asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId)
        {
            return await GetSearchMovieAsync(movieId);
        }

        private async Task<SearchMovie> GetSearchMovieAsync(string movieId)
        {
            //create request
            IRestRequest request = new RestRequest($"/3/movie/{movieId}?api_key={_apiKey}", Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            return await ExecuteRestRequest<SearchMovie>(request);
        }

        /// <summary>
        /// Gets the poster URI asynchronous.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        public async Task<string> GetPosterUriAsync(string posterPath)
        {
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrEmpty(_posterBaseUri))
            {
                string posterUri = await GetPosterBaseUriAsync();
                if (!string.IsNullOrEmpty(posterUri))
                {
                    _posterBaseUri = posterUri;
                }
                else
                {
                    return string.Empty;
                }
            }

            return $"{_posterBaseUri}w342{posterPath}";
        }
        private async Task<string> GetPosterBaseUriAsync()
        {
            //create the request
            IRestRequest request = new RestRequest($"/3/configuration?api_key={_apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            //execute the request
            TMDbConfig tmdbConfig = await ExecuteRestRequest<TMDbConfig>(request);
            if (tmdbConfig != null)
            {
                return tmdbConfig.Images.BaseUrl;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

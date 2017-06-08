using Newtonsoft.Json;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    /// <summary>
    /// TMDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Interface.ITmdbManager" />
    public class TmdbManager : ITmdbManager
    {
        private string apiKey;
        private IRetryHelper _retryHelper;
        private string posterBaseUri;
        private RestClient _restClient;

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
        public TmdbManager(IConfigurationManager configManager, IRetryHelper retryHelper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            apiKey = configManager.TmDbApiKey;
            _retryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
            _restClient = new RestClient("https://api.themoviedb.org");
            _restClient.AddDefaultHeader("content-type", "application/json");
        }

        /// <summary>
        /// Searches the movie by name asynchronous.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <param name="movieYear">The movie year.</param>
        /// <returns></returns>
        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear)
        {
            string resource = string.Empty;
            //if no movie year then don't include in the query
            if (movieYear == 0)
            {
                resource = $"/3/search/movie?&query={movieName}&language=en-US&api_key={apiKey}";
            }
            else
            {
                resource = $"/3/search/movie?year={movieYear}&query={movieName}&language=en-US&api_key={apiKey}";
            }

            RestRequest request = new RestRequest(resource, Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(response.Content);
            }
            else
            {
                //TODO throw
                return null;
            }
        }

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId)
        {
            Common.Movie.Model.Movie movie = null;
            RestRequest request = new RestRequest($"/3/movie/{movieId}?api_key={apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                movie = JsonConvert.DeserializeObject<Common.Movie.Model.Movie>(response.Content);
            }
            else
            {
                //TODO THROW
            }

            request = new RestRequest($"/3/movie/{movieId}/credits?api_key={apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                movie.Credits = JsonConvert.DeserializeObject<Credits>(response.Content);
            }

            return movie;
        }

        /// <summary>
        /// Searches the movie by identifier asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId)
        {
            var request = new RestRequest($"/3/movie/{movieId}?api_key={apiKey}", Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SearchMovie>(response.Content);
            }
            else
            {
                //TODO THROW
                return null;
            }
        }

        /// <summary>
        /// Gets the poster URI asynchronous.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        public async Task<string> GetPosterUriAsync(string posterPath)
        {
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrEmpty(posterBaseUri))
            {
                RestRequest request = new RestRequest($"/3/configuration?api_key={apiKey}", Method.GET);
                request.AddParameter("application/json", "{}", ParameterType.RequestBody);
                IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TMDbConfig tmdbConfig = JsonConvert.DeserializeObject<TMDbConfig>(response.Content);
                    posterBaseUri = tmdbConfig.Images.BaseUrl;
                }
                else
                {
                    //TODO THROW
                }
            }

            return $"{posterBaseUri}w342{posterPath}";
        }
    }
}

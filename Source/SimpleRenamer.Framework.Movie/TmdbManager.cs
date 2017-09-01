using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    /// <summary>
    /// TMDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Interface.ITmdbManager" />
    public class TmdbManager : ITmdbManager
    {
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

            _restClient = new RestClient("https://api.themoviedb.org");
            _restClient.AddDefaultHeader("content-type", "application/json");
            _restClient.AddDefaultParameter("api_key", configManager.TmDbApiKey, ParameterType.QueryString);
            _restClient.AddDefaultParameter("language", CultureInfo.CurrentCulture.Name, ParameterType.QueryString);
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
        /// Searches the movie by name.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <param name="movieYear">The movie release year.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieName</exception>
        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int? movieYear = null)
        {
            if (string.IsNullOrWhiteSpace(movieName))
            {
                throw new ArgumentNullException(nameof(movieName));
            }

            //create the request
            IRestRequest request = new RestRequest("/3/search/movie", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            request.AddParameter("query", movieName, ParameterType.QueryString);
            if (movieYear.HasValue)
            {
                request.AddParameter("year", movieYear.ToString());
            }

            return await _helper.ExecuteRestRequest<SearchContainer<SearchMovie>>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds);
        }

        /// <summary>
        /// Gets movie details.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }
            //create the request
            IRestRequest request = new RestRequest($"/3/movie/{movieId}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            request.AddParameter("append_to_response", "credits", ParameterType.QueryString);

            //execute the request
            return await _helper.ExecuteRestRequest<Common.Movie.Model.Movie>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds);
        }

        /// <summary>
        /// Searches the movie by identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }
            //create request
            IRestRequest request = new RestRequest($"/3/movie/{movieId}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            return await _helper.ExecuteRestRequest<SearchMovie>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds);
        }

        /// <summary>
        /// Gets the poster URI.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">posterPath</exception>
        public async Task<string> GetPosterUriAsync(string posterPath)
        {
            if (string.IsNullOrWhiteSpace(posterPath))
            {
                throw new ArgumentNullException(nameof(posterPath));
            }
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrWhiteSpace(_posterBaseUri))
            {
                string posterUri = await GetPosterBaseUriAsync();
                if (!string.IsNullOrWhiteSpace(posterUri))
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
            IRestRequest request = new RestRequest($"/3/configuration", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            //execute the request
            TMDbConfig tmdbConfig = await _helper.ExecuteRestRequest<TMDbConfig>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds);
            if (!string.IsNullOrWhiteSpace(tmdbConfig?.Images?.SecureBaseUrl))
            {
                return tmdbConfig.Images.SecureBaseUrl;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

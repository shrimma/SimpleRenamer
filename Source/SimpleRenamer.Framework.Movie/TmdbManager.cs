using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
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
        private const int _maxRetryCount = 10;
        private const int _maxBackoffSeconds = 2;
        private readonly IRestClient _restClient;
        private readonly IHelper _helper;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private const string _baseUrl = "https://api.themoviedb.org";

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

            //create the rest client
            _restClient = new RestClient(_baseUrl);
            _restClient.AddDefaultHeader("content-type", "application/json");
            _restClient.AddDefaultParameter("api_key", configManager.TmDbApiKey, ParameterType.QueryString);
            _restClient.AddDefaultParameter("language", CultureInfo.CurrentCulture.Name, ParameterType.QueryString);

            //setup lease timeout to account for DNS changes
            ServicePoint sp = ServicePointManager.FindServicePoint(new Uri(_baseUrl));
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute

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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="movieYear">The movie release year.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieName</exception>
        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null)
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

            return await _helper.ExecuteRestRequestAsync<SearchContainer<SearchMovie>>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, cancellationToken);
        }

        /// <summary>
        /// Gets movie details.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }
            //create the request
            IRestRequest request = new RestRequest(string.Format("/3/movie/{0}", movieId), Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            request.AddParameter("append_to_response", "credits", ParameterType.QueryString);

            //execute the request
            return await _helper.ExecuteRestRequestAsync<Common.Movie.Model.Movie>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, cancellationToken);
        }

        /// <summary>
        /// Searches the movie by identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }
            //create request
            IRestRequest request = new RestRequest(string.Format("/3/movie/{0}", movieId), Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            return await _helper.ExecuteRestRequestAsync<SearchMovie>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, cancellationToken);
        }

        /// <summary>
        /// Gets the poster URI.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">posterPath</exception>
        public async Task<string> GetPosterUriAsync(string posterPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(posterPath))
            {
                throw new ArgumentNullException(nameof(posterPath));
            }
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrWhiteSpace(_posterBaseUri))
            {
                string posterUri = await GetPosterBaseUriAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(posterUri))
                {
                    _posterBaseUri = posterUri;
                }
                else
                {
                    return string.Empty;
                }
            }

            return string.Format("{0}w342{1}", _posterBaseUri, posterPath);
        }
        private async Task<string> GetPosterBaseUriAsync(CancellationToken cancellationToken)
        {
            //create the request
            IRestRequest request = new RestRequest("/3/configuration", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            //execute the request
            TMDbConfig tmdbConfig = await _helper.ExecuteRestRequestAsync<TMDbConfig>(_restClient, request, _jsonSerializerSettings, _maxRetryCount, _maxBackoffSeconds, cancellationToken);
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

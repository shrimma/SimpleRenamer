using Flurl;
using Flurl.Http;
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
        private readonly Url _restClient;
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
            _restClient = _baseUrl.SetQueryParams(new { api_key = configManager.TmDbApiKey, language = CultureInfo.CurrentCulture.Name });

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

            //create request
            Url request = _restClient
                .AppendPathSegment("/3/search/movie")
                .SetQueryParam("query", movieName);
            if (movieYear.HasValue)
            {
                request.SetQueryParam("year", movieYear.ToString());
            }

            //execute the request
            return await request.WithHeader("content-type", "application/json").GetJsonAsync<SearchContainer<SearchMovie>>(cancellationToken);
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
            Url request = _restClient
                .AppendPathSegment(string.Format("/3/movie/{0}", movieId))
                .SetQueryParam("append_to_response", "credits");

            //execute the request
            return await request.WithHeader("content-type", "application/json").GetJsonAsync<Common.Movie.Model.Movie>(cancellationToken);
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

            //create the request
            Url request = _restClient.AppendPathSegment(string.Format("/3/movie/{0}", movieId));

            //execute request
            return await request.WithHeader("content-type", "application/json").GetJsonAsync<SearchMovie>(cancellationToken);
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
            Url request = _restClient.AppendPathSegment("/3/configuration");


            //execute the request            
            TMDbConfig tmdbConfig = await request.WithHeader("content-type", "application/json").GetJsonAsync<TMDbConfig>(cancellationToken);
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

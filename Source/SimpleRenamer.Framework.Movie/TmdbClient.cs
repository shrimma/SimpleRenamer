using Newtonsoft.Json;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    public class TmdbClient : ITmdbClient
    {
        private const string _baseQueryParameterTemplate = "?api_key={0}&language={1}";
        private const string _searchMovieById = "/3/movie/{0}{1}";        
        private const string _searchMovieByNameUri = "/3/search/movie{0}&query={1}";
        private const string _searchMovieByNameAndYearUri = _searchMovieByNameUri + "&year={2}";
        private const string _getMovieUri = _searchMovieById + "&append_to_response=credits";
        private const string _getConfigurationUri = "/3/configuration{0}";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _baseQueryParameters;
        private readonly JsonSerializerSettings _jsonSerializerSettings;        

        public TmdbClient(HttpClient httpClient, ILogger logger, IConfigurationManager configurationManager)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            _jsonSerializerSettings = new JsonSerializerSettings { Error = HandleDeserializationError };
            _baseQueryParameters = string.Format(_baseQueryParameterTemplate, configurationManager.TmDbApiKey, CultureInfo.CurrentCulture.Name);            
        }
        
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            //TODO log the error
            //errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }

            HttpResponseMessage response = await _httpClient.GetAsync(string.Format(_searchMovieById, movieId, _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SearchMovie>(responseContent, _jsonSerializerSettings);
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null)
        {
            if (string.IsNullOrWhiteSpace(movieName))
            {
                throw new ArgumentNullException(nameof(movieName));
            }

            string searchUri = string.Empty;
            if (movieYear.HasValue)
            {
                searchUri = string.Format(_searchMovieByNameAndYearUri, _baseQueryParameters, movieName, movieYear);
            }
            else
            {
                searchUri = string.Format(_searchMovieByNameUri, _baseQueryParameters, movieName);
            }

            HttpResponseMessage response = await _httpClient.GetAsync(searchUri);
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(responseContent, _jsonSerializerSettings);                        
        }

        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }            

            HttpResponseMessage response = await _httpClient.GetAsync(string.Format(_getMovieUri, movieId, _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Common.Movie.Model.Movie>(responseContent, _jsonSerializerSettings);                        
        }        

        public async Task<TMDbConfig> GetTmdbConfigAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(string.Format(_getConfigurationUri, _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TMDbConfig>(responseContent, _jsonSerializerSettings);                        
        }
    }
}

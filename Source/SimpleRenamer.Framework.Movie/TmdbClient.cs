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
            _baseQueryParameters = string.Format("?api_key={0}&language={1}", configurationManager.TmDbApiKey, CultureInfo.CurrentCulture.Name);            
        }
        
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            //TODO log the error
            //errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null)
        {
            if (string.IsNullOrWhiteSpace(movieName))
            {
                throw new ArgumentNullException(nameof(movieName));
            }

            string queryString = string.Empty;
            if (movieYear.HasValue)
            {
                queryString = string.Format("{0}&query={1}&year={2}", _baseQueryParameters, movieName, movieYear);
            }
            else
            {
                queryString = string.Format("{0}&query={1}", _baseQueryParameters, movieName);
            }

            HttpResponseMessage response = await _httpClient.GetAsync(string.Format("/3/search/movie{0}", queryString));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(responseContent, _jsonSerializerSettings);                        
        }

        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }            

            HttpResponseMessage response = await _httpClient.GetAsync(string.Format("/3/movie/{0}{1}&append_to_response=credits", movieId, _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Common.Movie.Model.Movie>(responseContent, _jsonSerializerSettings);                        
        }

        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }            

            HttpResponseMessage response = await _httpClient.GetAsync(string.Format("/3/movie/{0}{1}", movieId, _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SearchMovie>(responseContent, _jsonSerializerSettings);                        
        }

        public async Task<TMDbConfig> GetTmdbConfigAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(string.Format("/3/configuration{0}", _baseQueryParameters));
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TMDbConfig>(responseContent, _jsonSerializerSettings);                        
        }
    }
}

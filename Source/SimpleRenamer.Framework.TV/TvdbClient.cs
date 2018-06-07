using Newtonsoft.Json;
using Polly;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    public class TvdbClient : ITvdbClient
    {
        //URIs
        private const string _loginUri = "login";
        private const string _getSeriesUri = "series/{0}";
        private const string _getActorsUri = _getSeriesUri + "/actors";
        private const string _getEpisodesUri = _getSeriesUri + "/episodes";
        private const string _queryImagesUri = _getSeriesUri + "/images/query?keyType={1}";
        private const string _searchSeriesUri = "/search/series?name={0}";

        //HTTP bits
        private const string _jsonMediaType = "application/json";
        private const string _authorizationHeader = "Authorization";
        private const string _bearerToken = "Bearer {0}";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly Auth _auth;
        private readonly Policy<HttpResponseMessage> _authorizationPolicy;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public TvdbClient(HttpClient httpClient, ILogger logger, IConfigurationManager configurationManager)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }
            _auth = new Auth
            {
                Apikey = configurationManager.TvDbApiKey
            };

            //create polly retry policy
            _authorizationPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
            .RetryAsync(1, async (exception, retryCount, context) =>
            {                        
                await LoginAsync();
            });

            _jsonSerializerSettings = new JsonSerializerSettings { Error = HandleDeserializationError };
        }

        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            //TODO log the error
            //errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        private async Task LoginAsync()
        {            
            //create new request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _loginUri)
            {
                Content = new StringContent(_auth.ToJson(), Encoding.UTF8, _jsonMediaType)
            };

            //execute request and get response
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Token token = JsonConvert.DeserializeObject<Token>(responseContent, _jsonSerializerSettings);

            //if we got a response then add the authorization header
            if (token != null)
            {
                if (_httpClient.DefaultRequestHeaders.Contains(_authorizationHeader))
                {
                    _httpClient.DefaultRequestHeaders.Remove(_authorizationHeader);
                }
                _httpClient.DefaultRequestHeaders.Add(_authorizationHeader, string.Format(_bearerToken, token._Token));
            }            
        }

        public async Task<SeriesData> GetSeriesAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format(_getSeriesUri, tmdbId), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesData>(responseContent, _jsonSerializerSettings);            
        }

        public async Task<SeriesActors> GetActorsAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format(_getActorsUri, tmdbId), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesActors>(responseContent, _jsonSerializerSettings);
        }

        public async Task<SeriesEpisodes> GetEpisodesAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format(_getEpisodesUri, tmdbId), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesEpisodes>(responseContent, _jsonSerializerSettings);
        }        

        public async Task<SeriesImageQueryResults> QuerySeriesImagesAsync(string tmdbId, string imageType, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format(_queryImagesUri, tmdbId, imageType), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesImageQueryResults>(responseContent, _jsonSerializerSettings);            
        }

        public async Task<SeriesSearchDataList> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken)
        {                      
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format(_searchSeriesUri, seriesName), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesSearchDataList>(responseContent, _jsonSerializerSettings);            
        }
    }
}

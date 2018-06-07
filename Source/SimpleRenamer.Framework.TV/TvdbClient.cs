using Newtonsoft.Json;
using Polly;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    public class TvdbClient : ITvdbClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private Policy<HttpResponseMessage> _authorizationPolicy;

        public TvdbClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            //create polly retry policy
            _authorizationPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
            .RetryAsync(1, async (exception, retryCount, context) =>
            {                        
                await LoginAsync();
            });
        }

        private async Task LoginAsync()
        {
            //create rest request
            Auth auth = new Auth()
            {
                Apikey = _apiKey
            };

            //create new request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "login")
            {
                Content = new StringContent(auth.ToJson())
            };


            //execute request and get response
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Token token = JsonConvert.DeserializeObject<Token>(responseContent);

            //if we got a response then add the authorization header
            if (token != null)
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Bearer"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Bearer");
                }
                _httpClient.DefaultRequestHeaders.Add("Bearer", token._Token);
            }            
        }

        public async Task<SeriesData> GetSeriesAsync(string tmdbId, CancellationToken cancellationToken)
        {
            //create the request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("series/{0}", tmdbId));

            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesData>(responseContent);            
        }

        public async Task<SeriesActors> GetActorsAsync(string tmdbId, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("/series/{0}/actors", tmdbId));
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesActors>(responseContent);
        }

        public async Task<SeriesEpisodes> GetEpisodesAsync(string tmdbId, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("/series/{0}/episodes", tmdbId));
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesEpisodes>(responseContent);
        }

        public Task<SeriesImageQueryResults> GetSeriesPostersAsync(string tmdbId, CancellationToken cancellationToken)
        {
            return GetImagesAsync(tmdbId, "poster", cancellationToken);            
        }

        public Task<SeriesImageQueryResults> GetSeasonPostersAsync(string tmdbId, CancellationToken cancellationToken)
        {
            return GetImagesAsync(tmdbId, "season", cancellationToken);            
        }

        public Task<SeriesImageQueryResults> GetSeriesBannersAsync(string tmdbId, CancellationToken cancellationToken)
        {
            return GetImagesAsync(tmdbId, "series", cancellationToken);
        }

        private async Task<SeriesImageQueryResults> GetImagesAsync(string tmdbId, string imageType, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("/series/{0}/images/query?keyType={1}", tmdbId, imageType));
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            SeriesImageQueryResults results = JsonConvert.DeserializeObject<SeriesImageQueryResults>(responseContent);
            if (results == null)
            {
                return new SeriesImageQueryResults();
            }
            else
            {
                return results;
            }
        }

        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("/search/series?name={0}", seriesName));
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            SeriesSearchDataList searchData = JsonConvert.DeserializeObject<SeriesSearchDataList>(responseContent);
            if (searchData?.SearchResults != null)
            {
                return searchData.SearchResults;
            }
            else
            {
                throw new InvalidOperationException($"Unable to find any results for {seriesName}.");
            }
        }
    }
}

﻿using Newtonsoft.Json;
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
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly Auth _auth;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private Policy<HttpResponseMessage> _authorizationPolicy;

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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "login")
            {
                Content = new StringContent(_auth.ToJson(), Encoding.UTF8, "application/json")
            };

            //execute request and get response
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Token token = JsonConvert.DeserializeObject<Token>(responseContent);

            //if we got a response then add the authorization header
            if (token != null)
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token._Token));
            }            
        }

        public async Task<SeriesData> GetSeriesAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format("series/{0}", tmdbId), ct), cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesData>(responseContent);            
        }

        public async Task<SeriesActors> GetActorsAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format("/series/{0}/actors", tmdbId), ct), cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SeriesActors>(responseContent);
        }

        public async Task<SeriesEpisodes> GetEpisodesAsync(string tmdbId, CancellationToken cancellationToken)
        {            
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format("/series/{0}/episodes", tmdbId), ct), cancellationToken);
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
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format("/series/{0}/images/query?keyType={1}", tmdbId, imageType), ct), cancellationToken);

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
            HttpResponseMessage response = await _authorizationPolicy.ExecuteAsync((ct) => _httpClient.GetAsync(string.Format("/search/series?name={0}", seriesName), ct), cancellationToken);

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
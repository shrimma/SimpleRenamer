using Newtonsoft.Json;
using RestSharp;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TvdbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class TvdbManager : ITvdbManager
    {
        private string apiKey;
        private string baseUri;
        private string jwtToken;
        private IRetryHelper retryHelper;
        public TvdbManager(IConfigurationManager configManager, IRetryHelper retryHelp)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            if (retryHelp == null)
            {
                throw new ArgumentNullException(nameof(retryHelp));
            }
            apiKey = configManager.TvDbApiKey;
            baseUri = "https://api.thetvdb.com";
            jwtToken = "";
            retryHelper = retryHelp;
        }

        private async Task Login()
        {
            Auth auth = new Auth();
            auth.Apikey = apiKey;

            RestClient client = new RestClient($"{baseUri}/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", auth.ToJson(), ParameterType.RequestBody);
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Token token = JsonConvert.DeserializeObject<Token>(response.Content);
                jwtToken = token._Token;
            }
            else
            {
                //should we throw here?
            }

        }

        public string GetBannerUri(string bannerPath)
        {
            return $"http://thetvdb.com/banners/{bannerPath}";
        }

        public async Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                await Login();
            }

            SeriesData series = null;
            SeriesActors actors = null;
            SeriesEpisodes episodes = null;
            SeriesImageQueryResults posters = null;
            SeriesImageQueryResults seasonPosters = null;
            SeriesImageQueryResults seriesBanners = null;

            //get general series data
            RestClient client = new RestClient($"{baseUri}/series/{tmdbId}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                series = JsonConvert.DeserializeObject<SeriesData>(response.Content);
            }
            else
            {
                //TODO what to do if login rejected
            }

            //get actors
            client = new RestClient($"{baseUri}/series/{tmdbId}/actors");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                actors = JsonConvert.DeserializeObject<SeriesActors>(response.Content);
            }
            else
            {
                //TODO
            }

            //get episodes
            client = new RestClient($"{baseUri}/series/{tmdbId}/episodes");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                episodes = JsonConvert.DeserializeObject<SeriesEpisodes>(response.Content);
            }
            else
            {
                //TODO
            }

            //get series posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "poster", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                posters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);
            }
            else
            {
                //TODO
            }

            //get season specific posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "season", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                seasonPosters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);
            }
            else
            {
                //TODO
            }

            //get series banners
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "series", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                seriesBanners = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);
            }
            else
            {
                //TODO
            }

            if (series != null && actors != null && episodes != null && posters != null && seasonPosters != null && seriesBanners != null)
            {
                return new CompleteSeries(series.Data, actors.Data, episodes.Data, posters.Data, seasonPosters.Data, seriesBanners.Data);
            }
            else
            {
                //TODO this should throw
                return null;
            }
        }

        public async Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                await Login();
            }

            RestClient client = new RestClient($"{baseUri}/search/series");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("name", seriesName, ParameterType.QueryString);
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SearchData data = JsonConvert.DeserializeObject<SearchData>(response.Content);
                return data.Series;
            }
            else
            {
                //TODO throw
                return null;
            }
        }
    }
}

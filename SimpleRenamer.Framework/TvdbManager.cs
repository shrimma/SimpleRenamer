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

            Token token = JsonConvert.DeserializeObject<Token>(response.Content);
            jwtToken = token._Token;
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

            //get general series data
            RestClient client = new RestClient($"{baseUri}/series/{tmdbId}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesData series = JsonConvert.DeserializeObject<SeriesData>(response.Content);

            //get actors
            client = new RestClient($"{baseUri}/series/{tmdbId}/actors");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesActors actors = JsonConvert.DeserializeObject<SeriesActors>(response.Content);

            //get episodes
            client = new RestClient($"{baseUri}/series/{tmdbId}/episodes");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesEpisodes episodes = JsonConvert.DeserializeObject<SeriesEpisodes>(response.Content);

            //get series posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "poster", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesImageQueryResults posters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);

            //get season specific posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "season", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesImageQueryResults seasonPosters = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);

            //get series banners
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "series", ParameterType.QueryString);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            SeriesImageQueryResults seriesBanners = JsonConvert.DeserializeObject<SeriesImageQueryResults>(response.Content);

            return new CompleteSeries(series.Data, actors.Data, episodes.Data, posters.Data, seasonPosters.Data, seriesBanners.Data);
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
            SearchData data = JsonConvert.DeserializeObject<SearchData>(response.Content);
            return data.Series;
        }
    }
}

using Newtonsoft.Json;
using RestSharp;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TvdbModel;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class TvdbManager : ITvdbManager
    {
        private string apiKey;
        private string baseUri;
        private string jwtToken;
        public TvdbManager(IConfigurationManager configManager)
        {
            apiKey = configManager.TvDbApiKey;
            baseUri = "https://api.thetvdb.com";
            jwtToken = "";
        }

        private async Task Login(CancellationToken ct)
        {
            Auth auth = new Auth();
            auth.Apikey = apiKey;

            RestClient client = new RestClient($"{baseUri}/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", auth.ToJson(), ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteTaskAsync(request, ct);

            Token token = JsonConvert.DeserializeObject<Token>(response.Content);
            jwtToken = token._Token;
        }

        public async Task<string> GetBannerUriAsync(string bannerPath, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                await Login(ct);
            }



            return null;
        }

        public async Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                await Login(ct);
            }

            //get general series data
            RestClient client = new RestClient($"{baseUri}/series/{tmdbId}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            IRestResponse response = await client.ExecuteTaskAsync(request, ct);
            SeriesData series = JsonConvert.DeserializeObject<SeriesData>(response.Content);

            //get actors
            client = new RestClient($"{baseUri}/series/{tmdbId}/actors");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await client.ExecuteTaskAsync(request, ct);
            SeriesActors actors = JsonConvert.DeserializeObject<SeriesActors>(response.Content);

            //get episodes
            client = new RestClient($"{baseUri}/series/{tmdbId}/episodes");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            response = await client.ExecuteTaskAsync(request, ct);
            SeriesEpisodes episodes = JsonConvert.DeserializeObject<SeriesEpisodes>(response.Content);

            //get series posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "poster", ParameterType.QueryString);
            response = await client.ExecuteTaskAsync(request, ct);
            //TODO parse the response

            //get season specific posters
            client = new RestClient($"{baseUri}/series/{tmdbId}/images/query");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("keyType", "season", ParameterType.QueryString);
            response = await client.ExecuteTaskAsync(request, ct);
            //TODO parse the response

            return new CompleteSeries(series, actors.Data, episodes.Data);
        }

        public async Task<SearchData> SearchSeriesByNameAsync(string seriesName, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                await Login(ct);
            }

            RestClient client = new RestClient($"{baseUri}/search/series");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddParameter("name", seriesName, ParameterType.QueryString);
            IRestResponse response = await client.ExecuteTaskAsync(request, ct);

            SearchData data = JsonConvert.DeserializeObject<SearchData>(response.Content);
            return data;
        }
    }
}

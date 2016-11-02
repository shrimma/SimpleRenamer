using Newtonsoft.Json;
using RestSharp;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TmdbModel;
using System;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class TmdbManager : ITmdbManager
    {
        private string apiKey;
        private string baseUri;
        private IRetryHelper retryHelper;

        public TmdbManager(IConfigurationManager configManager, IRetryHelper retryHelp)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            if (retryHelp == null)
            {
                throw new ArgumentNullException(nameof(retryHelp));
            }
            apiKey = configManager.TmDbApiKey;
            baseUri = "";
            retryHelper = retryHelp;
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear)
        {
            RestClient client;
            //if no movie year then don't include in the query
            if (movieYear == 0)
            {
                client = new RestClient($"https://api.themoviedb.org/3/search/movie?&query={movieName}&language=en-US&api_key={apiKey}");
            }
            else
            {
                client = new RestClient($"https://api.themoviedb.org/3/search/movie?year={movieYear}&query={movieName}&language=en-US&api_key={apiKey}");
            }
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));

            return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(response.Content);
        }

        public async Task<MovieCredits> GetMovieAsync(string movieId)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{movieId}?api_key={apiKey}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            Movie movie = JsonConvert.DeserializeObject<Movie>(response.Content);

            client = new RestClient($"https://api.themoviedb.org/3/movie/{movieId}/credits?api_key={apiKey}");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));
            Credits credits = JsonConvert.DeserializeObject<Credits>(response.Content);

            return new MovieCredits(movie, credits);
        }

        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{movieId}?api_key={apiKey}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));

            return JsonConvert.DeserializeObject<SearchMovie>(response.Content);
        }

        public async Task<string> GetPosterUriAsync(string posterPath)
        {
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrEmpty(baseUri))
            {
                var client = new RestClient($"https://api.themoviedb.org/3/configuration?api_key={apiKey}");
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{}", ParameterType.RequestBody);
                IRestResponse response = await retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await client.ExecuteTaskAsync(request));

                TMDbConfig tmdbConfig = JsonConvert.DeserializeObject<TMDbConfig>(response.Content);
                baseUri = tmdbConfig.Images.BaseUrl;
            }

            return $"{baseUri}w342{posterPath}";
        }
    }
}

using Newtonsoft.Json;
using RestSharp;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace SimpleRenamer.Framework
{
    public class TmdbManager : ITmdbManager
    {
        private string apiKey;
        private string baseUri;

        public TmdbManager(IConfigurationManager configManager)
        {
            apiKey = configManager.TmDbApiKey;
            baseUri = "";
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear, CancellationToken ct)
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

            IRestResponse response = await client.ExecuteTaskAsync(request, ct);

            return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(response.Content);
        }

        public async Task<MovieCredits> GetMovieAsync(string movieId, CancellationToken ct)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{movieId}?api_key={apiKey}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Movie movie = JsonConvert.DeserializeObject<Movie>(response.Content);

            client = new RestClient($"https://api.themoviedb.org/3/movie/{movieId}/credits?api_key={apiKey}");
            request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            response = await client.ExecuteTaskAsync(request, ct);
            Credits credits = JsonConvert.DeserializeObject<Credits>(response.Content);

            return new MovieCredits(movie, credits);
        }

        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken ct)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/search/movie/{movieId}?api_key={apiKey}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);

            IRestResponse response = await client.ExecuteTaskAsync(request, ct);

            return JsonConvert.DeserializeObject<SearchMovie>(response.Content);
        }

        public async Task<string> GetPosterUriAsync(string posterPath, CancellationToken ct)
        {
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrEmpty(baseUri))
            {
                var client = new RestClient($"https://api.themoviedb.org/3/configuration?api_key={apiKey}");
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{}", ParameterType.RequestBody);

                IRestResponse response = await client.ExecuteTaskAsync(request, ct);

                TMDbConfig tmdbConfig = JsonConvert.DeserializeObject<TMDbConfig>(response.Content);
                baseUri = tmdbConfig.Images.BaseUrl;
            }

            return $"{baseUri}w342{posterPath}";
        }
    }
}

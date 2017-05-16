using Newtonsoft.Json;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    public class TmdbManager : ITmdbManager
    {
        private string apiKey;
        private IRetryHelper _retryHelper;
        private string posterBaseUri;
        private RestClient _restClient;

        public TmdbManager(IConfigurationManager configManager, IRetryHelper retryHelper)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            apiKey = configManager.TmDbApiKey;
            _retryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
            _restClient = new RestClient("https://api.themoviedb.org");
            _restClient.AddDefaultHeader("content-type", "application/json");
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear)
        {
            string resource = string.Empty;
            //if no movie year then don't include in the query
            if (movieYear == 0)
            {
                resource = $"/3/search/movie?&query={movieName}&language=en-US&api_key={apiKey}";
            }
            else
            {
                resource = $"/3/search/movie?year={movieYear}&query={movieName}&language=en-US&api_key={apiKey}";
            }

            RestRequest request = new RestRequest(resource, Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SearchContainer<SearchMovie>>(response.Content);
            }
            else
            {
                //TODO throw
                return null;
            }
        }

        public async Task<MovieCredits> GetMovieAsync(string movieId)
        {
            Common.Movie.Model.Movie movie = null;
            Credits credits = null;
            RestRequest request = new RestRequest($"/3/movie/{movieId}?api_key={apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                movie = JsonConvert.DeserializeObject<Common.Movie.Model.Movie>(response.Content);
            }
            else
            {
                //TODO THROW
            }

            request = new RestRequest($"/3/movie/{movieId}/credits?api_key={apiKey}", Method.GET);
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                credits = JsonConvert.DeserializeObject<Credits>(response.Content);
            }

            if (movie != null && credits != null)
            {
                return new MovieCredits(movie, credits);
            }
            else
            {
                //TODO THROW
                return null;
            }
        }

        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId)
        {
            var request = new RestRequest($"/3/movie/{movieId}?api_key={apiKey}", Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{}", ParameterType.RequestBody);
            IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SearchMovie>(response.Content);
            }
            else
            {
                //TODO THROW
                return null;
            }
        }

        public async Task<string> GetPosterUriAsync(string posterPath)
        {
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrEmpty(posterBaseUri))
            {
                RestRequest request = new RestRequest($"/3/configuration?api_key={apiKey}", Method.GET);
                request.AddParameter("application/json", "{}", ParameterType.RequestBody);
                IRestResponse response = await _retryHelper.OperationWithBasicRetryAsync<IRestResponse>(async () => await _restClient.ExecuteTaskAsync(request));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TMDbConfig tmdbConfig = JsonConvert.DeserializeObject<TMDbConfig>(response.Content);
                    posterBaseUri = tmdbConfig.Images.BaseUrl;
                }
                else
                {
                    //TODO THROW
                }
            }

            return $"{posterBaseUri}w342{posterPath}";
        }
    }
}

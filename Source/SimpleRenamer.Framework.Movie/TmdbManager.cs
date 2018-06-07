using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    /// <summary>
    /// TMDB Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Interface.ITmdbManager" />
    public class TmdbManager : ITmdbManager
    {
        private string _posterBaseUri;                      
        private readonly ITmdbClient _tmdbClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TmdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="retryHelper">The retry helper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// retryHelper
        /// </exception>
        public TmdbManager(ITmdbClient tmdbClient)
        {                     
            _tmdbClient = tmdbClient ?? throw new ArgumentNullException(nameof(tmdbClient));
        }       

        /// <summary>
        /// Searches the movie by name.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="movieYear">The movie release year.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieName</exception>
        public async Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null)
        {
            if (string.IsNullOrWhiteSpace(movieName))
            {
                throw new ArgumentNullException(nameof(movieName));
            }

            //execute the request and check we got a result
            SearchContainer<SearchMovie> results = await _tmdbClient.SearchMovieByNameAsync(movieName, cancellationToken, movieYear);
            if (results != null)
            {
                return results;
            }
            else
            {
                throw new InvalidOperationException($"Unable to find any results for {movieName}");
            }
        }

        /// <summary>
        /// Gets movie details.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }


            Common.Movie.Model.Movie result = await _tmdbClient.GetMovieAsync(movieId, cancellationToken);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException($"Failed to retrieve data for {movieId}");
            }
        }

        /// <summary>
        /// Searches the movie by identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">movieId</exception>
        public async Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(movieId))
            {
                throw new ArgumentNullException(nameof(movieId));
            }

            SearchMovie result = await _tmdbClient.SearchMovieByIdAsync(movieId, cancellationToken);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException($"Unable to retrieve movie result for {movieId}.");
            }
        }

        /// <summary>
        /// Gets the poster URI.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">posterPath</exception>
        public async Task<string> GetPosterUriAsync(string posterPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(posterPath))
            {
                throw new ArgumentNullException(nameof(posterPath));
            }
            //if we havent grabbed the base uri yet this session
            if (string.IsNullOrWhiteSpace(_posterBaseUri))
            {
                string posterUri = await GetPosterBaseUriAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(posterUri))
                {
                    _posterBaseUri = posterUri;
                }
                else
                {
                    throw new InvalidOperationException("Unable to retrieve Poster URI path");
                }
            }

            return string.Format("{0}w342{1}", _posterBaseUri, posterPath);
        }
        private async Task<string> GetPosterBaseUriAsync(CancellationToken cancellationToken)
        {
            //execute the request
            TMDbConfig tmdbConfig = await _tmdbClient.GetTmdbConfigAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(tmdbConfig?.Images?.SecureBaseUrl))
            {
                return tmdbConfig.Images.SecureBaseUrl;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

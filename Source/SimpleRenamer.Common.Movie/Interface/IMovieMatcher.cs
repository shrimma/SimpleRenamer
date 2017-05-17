using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Movie.Interface
{
    public interface IMovieMatcher
    {
        /// <summary>
        /// Scrapes the details asynchronous.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <returns></returns>
        Task<MatchedFile> ScrapeDetailsAsync(MatchedFile episode);

        /// <summary>
        /// Gets the possible movies for file.
        /// </summary>
        /// <param name="showName">Name of the show.</param>
        /// <returns></returns>
        Task<List<ShowView>> GetPossibleMoviesForFile(string showName);

        /// <summary>
        /// Updates the file with matched movie.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="matchedFile">The matched file.</param>
        /// <returns></returns>
        Task<MatchedFile> UpdateFileWithMatchedMovie(string movieId, MatchedFile matchedFile);

        /// <summary>
        /// Gets a movie and it's banner
        /// </summary>
        /// <param name="movieId">The TMDB movie ID</param>
        /// <returns>Populated MovieInfo object</returns>
        Task<MovieInfo> GetMovieWithBanner(string movieId, CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

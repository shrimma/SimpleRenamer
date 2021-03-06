﻿using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Movie.Interface
{
    /// <summary>
    /// Movie Matcher Interface
    /// </summary>
    public interface IMovieMatcher
    {
        /// <summary>
        /// Scrapes the details asynchronous.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<MatchedFile> ScrapeDetailsAsync(MatchedFile episode, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the possible movies for file.
        /// </summary>
        /// <param name="showName">Name of the show.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<DetailView>> GetPossibleMoviesForFileAsync(string showName, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the file with matched movie.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="matchedFile">The matched file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<MatchedFile> UpdateFileWithMatchedMovieAsync(string movieId, MatchedFile matchedFile, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a movie and it's banner
        /// </summary>
        /// <param name="movieId">The TMDB movie ID</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Populated MovieInfo object
        /// </returns>
        Task<(Model.Movie movie, Uri bannerUri)> GetMovieWithBannerAsync(string movieId, CancellationToken cancellationToken);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

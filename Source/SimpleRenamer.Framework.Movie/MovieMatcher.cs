using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    /// <summary>
    /// Movie Matcher
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Interface.IMovieMatcher" />
    public class MovieMatcher : IMovieMatcher
    {
        private ILogger _logger;
        private ITmdbManager _tmdbManager;
        private IHelper _helper;
        private ParallelOptions _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieMatcher"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="tmdbManager">The TMDB manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// tmdbManager
        /// </exception>
        public MovieMatcher(ILogger logger, ITmdbManager tmdbManager, IHelper helper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tmdbManager = tmdbManager ?? throw new ArgumentNullException(nameof(tmdbManager));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <summary>
        /// Gets the possible movies for file.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <returns></returns>
        public async Task<List<DetailView>> GetPossibleMoviesForFileAsync(string movieName)
        {
            _logger.TraceMessage($"Get possible matches for movie: {movieName}.", EventLevel.Verbose);
            ConcurrentBag<DetailView> movies = new ConcurrentBag<DetailView>();
            SearchContainer<SearchMovie> results = await _tmdbManager.SearchMovieByNameAsync(movieName);
            if (results != null)
            {
                Parallel.ForEach(results.Results, _parallelOptions, (s) =>
                {
                    try
                    {
                        string desc = "N/A";
                        if (!string.IsNullOrWhiteSpace(s.Overview))
                        {
                            if (s.Overview.Length > 50)
                            {
                                desc = string.Format("{0}...", s.Overview.Substring(0, 50));
                            }
                            else
                            {
                                desc = s.Overview;
                            }
                        }
                        movies.Add(new DetailView(s.Id.ToString(), s.Title, s.ReleaseDate.HasValue ? s.ReleaseDate.Value.Year.ToString() : "N/A", desc));
                    }
                    catch (Exception)
                    {
                        //TODO just swalow this?
                    }
                });
            }

            _logger.TraceMessage($"Found {movies.Count} possible matches for movie: {movieName}.", EventLevel.Verbose);
            return movies.ToList();
        }

        /// <summary>
        /// Scrapes the details asynchronous.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        public async Task<MatchedFile> ScrapeDetailsAsync(MatchedFile movie)
        {
            _logger.TraceMessage($"Scraping Movie Details for {movie.SourceFilePath}.", EventLevel.Verbose);
            OnProgressTextChanged(new ProgressTextEventArgs($"Scraping details for file {movie.SourceFilePath}"));

            SearchContainer<SearchMovie> results = await _tmdbManager.SearchMovieByNameAsync(movie.ShowName, movie.Year);
            //if only one result then we can safely match
            if (results?.Results?.Count == 1)
            {
                //if theres only one match then scape the specific show
                movie.TMDBShowId = results.Results[0].Id;
                movie.ShowImage = results.Results[0].PosterPath;
                movie.NewFileName = _helper.RemoveSpecialCharacters(movie.ShowName);
                _logger.TraceMessage($"Found exactly one result so can safely match {movie.SourceFilePath} with MovieId {movie.TMDBShowId}.", EventLevel.Verbose);
            }
            else
            {
                movie.ActionThis = false;
                movie.SkippedExactSelection = true;
                _logger.TraceMessage($"Found {results?.Results?.Count} possible matches for {movie.SourceFilePath}. So must be manually matched by user.", EventLevel.Verbose);
            }
            return movie;
        }

        /// <summary>
        /// Updates the file with matched movie.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="matchedFile">The matched file.</param>
        /// <returns></returns>
        public async Task<MatchedFile> UpdateFileWithMatchedMovieAsync(string movieId, MatchedFile matchedFile)
        {
            if (matchedFile == null)
            {
                throw new ArgumentNullException(nameof(matchedFile));
            }

            return await Task.Run(async () =>
            {
                //id can be null if user didn't select a match
                if (!string.IsNullOrWhiteSpace(movieId))
                {
                    _logger.TraceMessage($"Updating File {matchedFile.SourceFilePath} with info for MovieId: {movieId}.", EventLevel.Verbose);
                    SearchMovie searchedMovie = await _tmdbManager.SearchMovieByIdAsync(movieId);
                    matchedFile.ActionThis = true;
                    matchedFile.SkippedExactSelection = false;
                    matchedFile.ShowName = searchedMovie.Title;
                    matchedFile.TMDBShowId = searchedMovie.Id;
                    matchedFile.Year = searchedMovie.ReleaseDate.HasValue ? searchedMovie.ReleaseDate.Value.Year : 0;
                    matchedFile.ShowImage = searchedMovie.PosterPath;
                    matchedFile.FileType = FileType.Movie;
                    matchedFile.NewFileName = _helper.RemoveSpecialCharacters(searchedMovie.Title);
                    _logger.TraceMessage($"Updated File {matchedFile.SourceFilePath} with info for MovieId: {movieId}.", EventLevel.Verbose);
                }
                else
                {
                    _logger.TraceMessage($"Did not update File {matchedFile.SourceFilePath} as no MovieId provided.", EventLevel.Verbose);
                    matchedFile.ActionThis = false;
                    matchedFile.SkippedExactSelection = true;
                }

                return matchedFile;
            });
        }

        /// <summary>
        /// Gets the movie with banner.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public async Task<(Common.Movie.Model.Movie movie, BitmapImage banner)> GetMovieWithBannerAsync(string movieId, CancellationToken ct)
        {
            _logger.TraceMessage($"Getting MovieInfo for MovieId: {movieId}.", EventLevel.Verbose);
            Common.Movie.Model.Movie matchedMovie = await _tmdbManager.GetMovieAsync(movieId);

            BitmapImage bannerImage = null;
            if (!string.IsNullOrWhiteSpace(matchedMovie.PosterPath))
            {
                bannerImage = InitializeBannerImage(new Uri(await _tmdbManager.GetPosterUriAsync(matchedMovie.PosterPath)));
            }
            else
            {
                //TODO add a not found poster
                bannerImage = new BitmapImage();
            }

            _logger.TraceMessage("Got MovieInfo for MovieId: {movieId}", EventLevel.Verbose);
            return (matchedMovie, bannerImage);
        }

        protected virtual BitmapImage InitializeBannerImage(Uri uri)
        {
            BitmapImage banner = new BitmapImage();
            banner.BeginInit();
            banner.UriSource = uri;
            banner.EndInit();

            return banner;
        }

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}

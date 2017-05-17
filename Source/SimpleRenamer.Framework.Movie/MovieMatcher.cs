using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    public class MovieMatcher : IMovieMatcher
    {
        private ILogger _logger;
        private ITmdbManager _tmdbManager;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public MovieMatcher(ILogger logger, ITmdbManager tmdbManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tmdbManager = tmdbManager ?? throw new ArgumentNullException(nameof(tmdbManager));
        }

        public async Task<List<ShowView>> GetPossibleMoviesForFile(string movieName)
        {
            List<ShowView> movies = new List<ShowView>();
            SearchContainer<SearchMovie> results = await _tmdbManager.SearchMovieByNameAsync(movieName, 0);
            foreach (var s in results.Results)
            {
                try
                {
                    string desc = string.Empty;
                    if (!string.IsNullOrEmpty(s.Overview))
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
                    movies.Add(new ShowView(s.Id.ToString(), s.Title, s.ReleaseDate.HasValue ? s.ReleaseDate.Value.Year.ToString() : "N/A", desc));
                }
                catch (Exception ex)
                {
                    //TODO just swalow this?
                }
            }

            return movies;
        }

        public async Task<MatchedFile> ScrapeDetailsAsync(MatchedFile movie)
        {
            _logger.TraceMessage("ScrapeDetailsAsync - Start");
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Scraping details for file {movie.SourceFilePath}"));

            SearchContainer<SearchMovie> results = await _tmdbManager.SearchMovieByNameAsync(movie.ShowName, movie.Year);

            //IF we have more than 1 result then flag the file to be manually matched
            if (results.Results.Count > 1)
            {
                movie.ActionThis = false;
                movie.SkippedExactSelection = true;
            }
            else if (results.Results.Count == 1)
            {
                //if theres only one match then scape the specific show
                movie.TMDBShowId = results.Results[0].Id;
                movie.ShowImage = results.Results[0].PosterPath;
                movie.NewFileName = RemoveSpecialCharacters(movie.ShowName);
            }
            _logger.TraceMessage("ScrapeDetailsAsync - End");
            return movie;
        }

        public async Task<MatchedFile> UpdateFileWithMatchedMovie(string movieId, MatchedFile matchedFile)
        {
            return await Task.Run(async () =>
            {
                _logger.TraceMessage("UpdateFileWithMatchedMovie - Start");

                if (!string.IsNullOrEmpty(movieId))
                {
                    SearchMovie searchedMovie = await _tmdbManager.SearchMovieByIdAsync(movieId);
                    matchedFile.ActionThis = true;
                    matchedFile.SkippedExactSelection = false;
                    matchedFile.ShowName = searchedMovie.Title;
                    matchedFile.TMDBShowId = searchedMovie.Id;
                    matchedFile.Year = searchedMovie.ReleaseDate.HasValue ? searchedMovie.ReleaseDate.Value.Year : 0;
                    matchedFile.ShowImage = searchedMovie.PosterPath;
                    matchedFile.FileType = FileType.Movie;
                    matchedFile.NewFileName = RemoveSpecialCharacters(searchedMovie.Title);
                }
                else
                {
                    matchedFile.ActionThis = false;
                    matchedFile.SkippedExactSelection = true;
                }

                _logger.TraceMessage("UpdateFileWithMatchedMovie - End");
                return matchedFile;
            });
        }

        private string RemoveSpecialCharacters(string input)
        {
            _logger.TraceMessage("RemoveSpecialCharacters - Start");
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            _logger.TraceMessage("RemoveSpecialCharacters - End");
            return r.Replace(input, "");
        }

        public async Task<MovieInfo> GetMovieWithBanner(string movieId, CancellationToken ct)
        {
            _logger.TraceMessage("GetMovieInfo - Start");
            MovieCredits matchedMovie = await _tmdbManager.GetMovieAsync(movieId);
            BitmapImage bannerImage = new BitmapImage();

            if (!string.IsNullOrEmpty(matchedMovie.Movie.PosterPath))
            {
                bannerImage.BeginInit();
                bannerImage.UriSource = new Uri(await _tmdbManager.GetPosterUriAsync(matchedMovie.Movie.PosterPath));
                bannerImage.EndInit();
            }
            else
            {
                //TODO add a not found poster
            }

            _logger.TraceMessage("GetMovieInfo - End");
            return new MovieInfo(matchedMovie, bannerImage);
        }
    }
}

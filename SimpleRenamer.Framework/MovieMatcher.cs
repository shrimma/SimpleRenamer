using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TmdbModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class MovieMatcher : IMovieMatcher
    {
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
        private ILogger logger;
        private ITmdbManager tmdbManager;

        public MovieMatcher(ILogger log, ITmdbManager tmdbMan)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tmdbMan == null)
            {
                throw new ArgumentNullException(nameof(tmdbMan));
            }


            logger = log;
            tmdbManager = tmdbMan;
        }

        public async Task<List<ShowView>> GetPossibleMoviesForFile(string movieName, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                List<ShowView> movies = new List<ShowView>();
                SearchContainer<SearchMovie> results = await tmdbManager.SearchMovieByNameAsync(movieName, 0);
                foreach (var s in results.Results)
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
                    movies.Add(new ShowView(s.Id.ToString(), s.Title, s.ReleaseDate.Value.Year.ToString(), desc));
                }

                return movies;
            });
        }

        public async Task<MatchedFile> ScrapeDetailsAsync(MatchedFile movie, CancellationToken ct)
        {
            logger.TraceMessage("ScrapeDetailsAsync - Start");
            //RaiseProgressEvent(this, new ProgressTextEventArgs($"Scraping details for file {movie.FilePath}"));

            SearchContainer<SearchMovie> results = await tmdbManager.SearchMovieByNameAsync(movie.ShowName, movie.Year);

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
            }
            logger.TraceMessage("ScrapeDetailsAsync - End");
            return movie;
        }

        public async Task<MatchedFile> UpdateFileWithMatchedMovie(string movieId, MatchedFile matchedFile, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                logger.TraceMessage("UpdateFileWithMatchedMovie - Start");

                if (!string.IsNullOrEmpty(movieId))
                {
                    SearchMovie searchedMovie = await tmdbManager.SearchMovieByIdAsync(movieId);
                    matchedFile.ActionThis = true;
                    matchedFile.SkippedExactSelection = false;
                    matchedFile.ShowName = searchedMovie.Title;
                    matchedFile.TMDBShowId = searchedMovie.Id;
                    matchedFile.ShowImage = searchedMovie.PosterPath;
                }
                else
                {
                    matchedFile.ActionThis = false;
                    matchedFile.SkippedExactSelection = true;
                }

                logger.TraceMessage("ScrapeDetailsAsync - End");
                return matchedFile;
            });
        }
    }
}

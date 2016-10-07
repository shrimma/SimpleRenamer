using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace SimpleRenamer.Framework
{
    public class MovieMatcher : IMovieMatcher
    {
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
        private ILogger logger;
        private TMDbClient tmdbManager;

        public MovieMatcher(ILogger log, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            logger = log;
            tmdbManager = new TMDbClient(configManager.TmDbApiKey);
        }

        public async Task<List<ShowView>> GetPossibleMoviesForFile(string showName)
        {
            throw new NotImplementedException();
        }

        public async Task<MatchedFile> ScrapeDetailsAsync(MatchedFile movie)
        {
            logger.TraceMessage("ScrapeDetailsAsync - Start");
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Scraping details for file {movie.FilePath}"));

            var results = await tmdbManager.SearchMovieAsync(movie.ShowName, year: movie.Year);
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

        public async Task<MatchedFile> UpdateFileWithMatchedMovie(string selectedMovieId, MatchedFile episode)
        {
            throw new NotImplementedException();
        }
    }
}

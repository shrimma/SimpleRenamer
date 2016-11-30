using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Movie.Interface
{
    public interface IMovieMatcher
    {
        Task<MatchedFile> ScrapeDetailsAsync(MatchedFile episode);

        Task<List<ShowView>> GetPossibleMoviesForFile(string showName);

        Task<MatchedFile> UpdateFileWithMatchedMovie(string movieId, MatchedFile matchedFile);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

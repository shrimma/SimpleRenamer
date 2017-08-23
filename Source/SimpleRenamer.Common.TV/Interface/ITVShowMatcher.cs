using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    /// <summary>
    /// TV Show Matcher Interface
    /// </summary>
    public interface ITVShowMatcher
    {
        /// <summary>
        /// Searches the show by name asynchronous.
        /// </summary>
        /// <param name="showName">Name of the show.</param>
        /// <returns></returns>
        Task<CompleteSeries> SearchShowByNameAsync(string showName);

        /// <summary>
        /// Searches the show by identifier asynchronous.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns></returns>
        Task<CompleteSeries> SearchShowByIdAsync(string showId);

        /// <summary>
        /// Updates the file with series details.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        MatchedFile UpdateFileWithSeriesDetails(MatchedFile file, CompleteSeries series);

        /// <summary>
        /// Fixes the shows from mappings.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        MatchedFile FixShowsFromMappings(MatchedFile file);

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="showName">The showname to be searched</param>
        /// <returns>A list of series</returns>
        Task<List<DetailView>> GetPossibleShowsForEpisodeAsync(string showName);

        /// <summary>
        /// Updates a TV episode with the details of a selected series
        /// </summary>
        /// <param name="selectedSeriesId">The TVDB show id selected</param>
        /// <param name="episode">Episode to be updated</param>
        /// <returns>The updated TV episode</returns>
        Task<MatchedFile> UpdateEpisodeWithMatchedSeriesAsync(string selectedSeriesId, MatchedFile episode);

        /// <summary>
        /// Gets a show and it's banner
        /// </summary>
        /// <param name="showId">The TVDB show ID to grab banner for</param>
        /// <returns>Populated SeriesWithBanner object</returns>
        Task<(CompleteSeries series, BitmapImage banner)> GetShowWithBannerAsync(string showId);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

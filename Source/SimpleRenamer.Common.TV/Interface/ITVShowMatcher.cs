using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    public interface ITVShowMatcher
    {
        /// <summary>
        /// Scrapes the TVDB for episode information on a specific episode and show
        /// </summary>
        /// <param name="episode">Episode to scrape</param>
        /// <param name="showNameMapping">Show to scrape</param>
        /// <returns></returns>
        Task<MatchedFile> ScrapeDetailsAsync(MatchedFile episode);

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="showName">The showname to be searched</param>
        /// <returns>A list of series</returns>
        Task<List<DetailView>> GetPossibleShowsForEpisode(string showName);

        /// <summary>
        /// Updates a TV episode with the details of a selected series
        /// </summary>
        /// <param name="selectedSeriesId">The TVDB show id selected</param>
        /// <param name="episode">Episode to be updated</param>
        /// <returns>The updated TV episode</returns>
        Task<MatchedFile> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, MatchedFile episode);

        /// <summary>
        /// Gets a show and it's banner
        /// </summary>
        /// <param name="showId">The TVDB show ID to grab banner for</param>
        /// <returns>Populated SeriesWithBanner object</returns>
        Task<SeriesWithBanner> GetShowWithBannerAsync(string showId);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

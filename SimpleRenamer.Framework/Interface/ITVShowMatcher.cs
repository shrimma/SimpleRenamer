using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITVShowMatcher
    {
        /// <summary>
        /// Scrapes the TVDB for episode information on a specific episode and show
        /// </summary>
        /// <param name="episode">Episode to scrape</param>
        /// <param name="showNameMapping">Show to scrape</param>
        /// <returns></returns>
        Task<TVEpisodeScrape> ScrapeDetailsAsync(MatchedFile episode);

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="showName">The showname to be searched</param>
        /// <returns>A list of series</returns>
        Task<List<ShowView>> GetPossibleShowsForEpisode(string showName);

        /// <summary>
        /// Updates a TV episode with the details of a selected series
        /// </summary>
        /// <param name="selectedSeriesId">The TVDB show id selected</param>
        /// <param name="episode">Episode to be updated</param>
        /// <returns>The updated TV episode</returns>
        Task<MatchedFile> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, MatchedFile episode);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

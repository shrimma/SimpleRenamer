﻿using SimpleRenamer.Framework.DataModel;
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
        Task<TVEpisodeScrape> ScrapeDetailsAsync(TVEpisode episode);

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="episode">Episode to be searched</param>
        /// <returns>A list of series</returns>
        Task<List<ShowView>> GetPossibleShowsForEpisode(TVEpisode episode);

        /// <summary>
        /// Updates a TV episode with the details of a selected series
        /// </summary>
        /// <param name="selectedSeriesId">The TVDB show id selected</param>
        /// <param name="episode">Episode to be updated</param>
        /// <returns>The updated TV episode</returns>
        Task<TVEpisode> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, TVEpisode episode);
    }
}

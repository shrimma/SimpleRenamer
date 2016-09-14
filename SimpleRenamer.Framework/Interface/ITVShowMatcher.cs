using SimpleRenamer.Framework.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITVShowMatcher
    {
        Task<TVEpisodeScrape> ScrapeDetailsAsync(TVEpisode episode, ShowNameMapping showNameMapping);
        Task<ShowNameMapping> ReadMappingFileAsync();
        Task<bool> WriteMappingFileAsync(ShowNameMapping showNameMapping);
        Task<List<ShowView>> GetPossibleShowsForEpisode(TVEpisode episode);
        Task<TVEpisode> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, TVEpisode episode);
    }
}

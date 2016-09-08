using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITVShowMatcher
    {
        Task<TVEpisodeScrape> ScrapeDetailsAsync(TVEpisode episode, ShowNameMapping showNameMapping);
        Task<ShowNameMapping> ReadMappingFileAsync();
        Task<bool> WriteMappingFileAsync(ShowNameMapping showNameMapping);
        Task<TVEpisode> SelectShowFromList(TVEpisode episode);
    }
}

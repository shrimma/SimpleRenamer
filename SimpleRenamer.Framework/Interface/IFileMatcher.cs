using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMatcher
    {
        /// <summary>
        /// Processes a file name against various Regular Expressions to extract TV show information
        /// </summary>
        /// <param name="fileName">The string to process</param>
        /// <returns>Populated TVEpisode object</returns>
        Task<TVEpisode> SearchFileNameAsync(string fileName);
    }
}

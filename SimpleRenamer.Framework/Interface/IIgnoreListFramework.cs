using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IIgnoreListFramework
    {
        /// <summary>
        /// Reads the IgnoreList File from the file system
        /// </summary>
        /// <returns>Populated IgnoreList object</returns>
        Task<IgnoreList> ReadIgnoreListAsync();

        /// <summary>
        /// Writes the IgnoreList File to the file system
        /// </summary>
        /// <param name="ignoreList">IgnoreLIst object to write</param>
        /// <returns>True if successfully written</returns>
        Task<bool> WriteIgnoreListAsync(IgnoreList ignoreList);
    }
}

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

        /// <summary>
        /// Reads the RegexExpression File from the file system
        /// </summary>
        /// <returns>Populated RegexFile object</returns>
        Task<RegexFile> ReadExpressionFileAsync();

        /// <summary>
        /// Writes the RegexExpression File to the file system
        /// </summary>
        /// <param name="regexMatchers">RegexFile object to write</param>
        /// <returns>True if successfully written</returns>
        Task<bool> WriteExpressionFileAsync(RegexFile regexMatchers);
    }
}

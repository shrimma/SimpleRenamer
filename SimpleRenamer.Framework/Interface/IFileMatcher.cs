using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMatcher
    {
        Task<TVEpisode> SearchFileNameAsync(string fileName);

        Task<RegexFile> ReadExpressionFileAsync();

        Task<bool> WriteExpressionFileAsync(RegexFile regexMatchers);
    }
}

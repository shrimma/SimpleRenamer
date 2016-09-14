using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IIgnoreListFramework
    {
        Task<IgnoreList> ReadIgnoreListAsync();
        Task<bool> WriteIgnoreListAsync(IgnoreList ignoreList);
    }
}

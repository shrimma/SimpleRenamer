using SimpleRenamer.Framework.DataModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileWatcher
    {
        Task<List<string>> SearchTheseFoldersAsync(Settings settings, CancellationToken ct);
    }
}

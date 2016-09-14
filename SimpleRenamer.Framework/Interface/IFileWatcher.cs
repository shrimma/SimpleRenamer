using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileWatcher
    {
        Task<List<string>> SearchTheseFoldersAsync(CancellationToken ct);
    }
}

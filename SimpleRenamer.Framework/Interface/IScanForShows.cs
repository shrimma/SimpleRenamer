using SimpleRenamer.Framework.DataModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IScanForShows
    {
        Task<List<TVEpisode>> Scan(CancellationToken ct);
    }
}

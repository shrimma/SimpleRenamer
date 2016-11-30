using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Interface
{
    public interface IScanForShows
    {
        /// <summary>
        /// Scans the watch folders and matches files against TVDB shows
        /// </summary>
        /// <param name="ct">CancellationToken</param>
        /// <returns>A list of TVEpisodes</returns>
        Task<List<MatchedFile>> Scan(CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IScanForShows
    {
        /// <summary>
        /// Scans the watch folders and matches files against TVDB shows
        /// </summary>
        /// <param name="ct">CancellationToken</param>
        /// <returns>A list of TVEpisodes</returns>
        Task<List<TVEpisode>> Scan(CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

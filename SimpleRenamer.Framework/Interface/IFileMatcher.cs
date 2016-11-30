using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Interface
{
    public interface IFileMatcher
    {
        /// <summary>
        /// Processes a file name against various Regular Expressions to extract TV show information
        /// </summary>
        /// <param name="fileName">The string to process</param>
        /// <returns>Populated TVEpisode object</returns>
        Task<List<MatchedFile>> SearchFilesAsync(List<string> files, CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

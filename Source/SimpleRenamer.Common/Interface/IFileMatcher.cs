using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// FileMatcher Interface
    /// </summary>
    public interface IFileMatcher
    {
        /// <summary>
        /// Processes a list of files trying to match the filenames against the regular expressions
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="ct">The cancellationtoken.</param>
        /// <returns></returns>
        Task<List<MatchedFile>> SearchFilesAsync(List<string> files, CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

﻿using Sarjee.SimpleRenamer.Common.EventArguments;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// File Watcher interface
    /// </summary>
    public interface IFileWatcher
    {
        /// <summary>
        /// Searches the configured folders for any video files
        /// </summary>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A list of file paths of video files</returns>
        Task<List<string>> SearchTheseFoldersAsync(CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

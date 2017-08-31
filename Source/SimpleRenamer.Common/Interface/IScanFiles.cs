﻿using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// ScanFiles interface
    /// </summary>
    public interface IScanFiles
    {
        /// <summary>
        /// Scans the watch folders and matches files against shows/movies
        /// </summary>
        /// <param name="ct">CancellationToken</param>
        /// <returns></returns>
        Task<List<MatchedFile>> ScanAsync(CancellationToken ct);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}
using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Interface
{
    public interface IActionMatchedFiles
    {
        /// <summary>
        /// Fired whenever a preprocessor action is completed on a file
        /// </summary>
        event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;

        /// <summary>
        /// Fired whenever a file is moved
        /// </summary>
        event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Performs preprocessor actions and then moves a list of scanned and matched episodes
        /// </summary>
        /// <param name="scannedEpisodes">The episodes to action</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns></returns>
        Task<bool> Action(ObservableCollection<MatchedFile> scannedEpisodes, CancellationToken ct);
    }
}

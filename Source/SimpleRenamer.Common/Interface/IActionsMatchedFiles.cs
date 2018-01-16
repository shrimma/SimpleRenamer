using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// ActionMatchedFiles interface
    /// </summary>
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
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        Task<bool> ActionAsync(ObservableCollection<MatchedFile> scannedEpisodes, CancellationToken cancellationToken);
    }
}

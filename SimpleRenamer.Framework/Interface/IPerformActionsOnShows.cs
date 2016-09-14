using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IPerformActionsOnShows
    {
        event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;
        event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;
        Task Action(ObservableCollection<TVEpisode> scannedEpisodes, CancellationToken ct);
    }
}

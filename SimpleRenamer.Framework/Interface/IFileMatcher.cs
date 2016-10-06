using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMatcher
    {
        /// <summary>
        /// Processes a file name against various Regular Expressions to extract TV show information
        /// </summary>
        /// <param name="fileName">The string to process</param>
        /// <returns>Populated TVEpisode object</returns>
        Task<List<TVEpisode>> SearchFilesAsync(List<string> files);

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;
    }
}

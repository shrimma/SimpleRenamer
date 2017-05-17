using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.Common.EventArguments
{
    /// <summary>
    /// FileMovedEventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class FileMovedEventArgs : EventArgs
    {
        private MatchedFile _file;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMovedEventArgs"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="System.ArgumentNullException">file</exception>
        public FileMovedEventArgs(MatchedFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public MatchedFile File
        {
            get { return _file; }
        }
    }
}

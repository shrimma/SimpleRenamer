using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    public interface ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to scan sub directories of watch folders.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should scan sub directories; otherwise, <c>false</c>.
        /// </value>
        bool SubDirectories { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to rename files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rename files]; otherwise, <c>false</c>.
        /// </value>
        bool RenameFiles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to copy or move files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if copy files; <c>false</c> if move files.
        /// </value>
        bool CopyFiles { get; set; }
        /// <summary>
        /// Gets or sets the new file name format.
        /// </summary>
        string NewFileNameFormat { get; set; }
        /// <summary>
        /// Gets or sets the watch folders.
        /// </summary>
        List<string> WatchFolders { get; set; }
        /// <summary>
        /// Gets or sets the valid extensions.
        /// </summary>
        List<string> ValidExtensions { get; set; }
        /// <summary>
        /// Gets or sets the destination folder for TV shows.
        /// </summary>        
        string DestinationFolderTV { get; set; }
        /// <summary>
        /// Gets or sets the destination folder for movies.
        /// </summary>        
        string DestinationFolderMovie { get; set; }
    }
}


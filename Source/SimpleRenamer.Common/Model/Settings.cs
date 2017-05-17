using System.Collections.Generic;
using System.ComponentModel;

namespace Sarjee.SimpleRenamer.Common.Model
{
    public class Settings : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged implementation

        /// <summary>
        /// The sub directories
        /// </summary>
        private bool subDirectories;
        /// <summary>
        /// Gets or sets a value indicating whether [sub directories].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sub directories]; otherwise, <c>false</c>.
        /// </value>
        public bool SubDirectories
        {
            get { return subDirectories; }
            set
            {
                if (value != subDirectories)
                {
                    subDirectories = value;
                    Notify("SubDirectories");
                }
            }
        }

        /// <summary>
        /// The rename files
        /// </summary>
        private bool renameFiles;
        /// <summary>
        /// Gets or sets a value indicating whether [rename files].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rename files]; otherwise, <c>false</c>.
        /// </value>
        public bool RenameFiles
        {
            get { return renameFiles; }
            set
            {
                if (value != renameFiles)
                {
                    renameFiles = value;
                    Notify("RenameFiles");
                }
            }
        }

        /// <summary>
        /// The copy files
        /// </summary>
        private bool copyFiles;
        /// <summary>
        /// Gets or sets a value indicating whether [copy files].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [copy files]; otherwise, <c>false</c>.
        /// </value>
        public bool CopyFiles
        {
            get { return copyFiles; }
            set
            {
                if (value != copyFiles)
                {
                    copyFiles = value;
                    Notify("CopyFiles");
                }
            }
        }

        /// <summary>
        /// The new file name format
        /// </summary>
        private string newFileNameFormat;
        /// <summary>
        /// Gets or sets the new file name format.
        /// </summary>
        /// <value>
        /// The new file name format.
        /// </value>
        public string NewFileNameFormat
        {
            get { return newFileNameFormat; }
            set
            {
                if (value != newFileNameFormat)
                {
                    newFileNameFormat = value;
                    Notify("NewFileNameFormat");
                }
            }
        }

        /// <summary>
        /// Gets or sets the watch folders.
        /// </summary>
        /// <value>
        /// The watch folders.
        /// </value>
        public List<string> WatchFolders { get; set; }
        /// <summary>
        /// Gets or sets the valid extensions.
        /// </summary>
        /// <value>
        /// The valid extensions.
        /// </value>
        public List<string> ValidExtensions { get; set; }

        /// <summary>
        /// The destination folder tv
        /// </summary>
        private string destinationFolderTV;

        /// <summary>
        /// Gets or sets the destination folder tv.
        /// </summary>
        /// <value>
        /// The destination folder tv.
        /// </value>
        public string DestinationFolderTV
        {
            get { return destinationFolderTV; }
            set
            {
                if (value != destinationFolderTV)
                {
                    destinationFolderTV = value;
                    Notify("DestinationFolderTV");
                }
            }
        }

        /// <summary>
        /// The destination folder movie
        /// </summary>
        private string destinationFolderMovie;

        /// <summary>
        /// Gets or sets the destination folder movie.
        /// </summary>
        /// <value>
        /// The destination folder movie.
        /// </value>
        public string DestinationFolderMovie
        {
            get { return destinationFolderMovie; }
            set
            {
                if (value != destinationFolderMovie)
                {
                    destinationFolderMovie = value;
                    Notify("DestinationFolderMovie");
                }
            }
        }
    }
}

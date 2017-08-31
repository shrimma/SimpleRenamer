using Jot.DefaultInitializer;
using Sarjee.SimpleRenamer.Common.Interface;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sarjee.SimpleRenamer.Common.Model
{
    public class Settings : ISettings, INotifyPropertyChanged
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

        private bool subDirectories;
        /// <summary>
        /// Gets or sets a value indicating whether to scan sub directories of watch folders.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should scan sub directories; otherwise, <c>false</c>.
        /// </value>
        [Trackable]
        public bool SubDirectories
        {
            get { return subDirectories; }
            set
            {
                if (value != subDirectories)
                {
                    subDirectories = value;
                    Notify(nameof(SubDirectories));
                }
            }
        }

        private bool renameFiles;
        /// <summary>
        /// Gets or sets a value indicating whether to rename files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rename files]; otherwise, <c>false</c>.
        /// </value>
        [Trackable]
        public bool RenameFiles
        {
            get { return renameFiles; }
            set
            {
                if (value != renameFiles)
                {
                    renameFiles = value;
                    Notify(nameof(RenameFiles));
                }
            }
        }

        private bool copyFiles;
        /// <summary>
        /// Gets or sets a value indicating whether to copy or move files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if copy files; <c>false</c> if move files.
        /// </value>
        [Trackable]
        public bool CopyFiles
        {
            get { return copyFiles; }
            set
            {
                if (value != copyFiles)
                {
                    copyFiles = value;
                    Notify(nameof(CopyFiles));
                }
            }
        }

        private string newFileNameFormat;
        /// <summary>
        /// Gets or sets the new file name format.
        /// </summary>        
        [Trackable]
        public string NewFileNameFormat
        {
            get { return newFileNameFormat; }
            set
            {
                if (value != newFileNameFormat)
                {
                    newFileNameFormat = value;
                    Notify(nameof(NewFileNameFormat));
                }
            }
        }

        private List<string> watchFolders = new List<string>();
        /// <summary>
        /// Gets or sets the watch folders.
        /// </summary>        
        [Trackable]
        public List<string> WatchFolders
        {
            get { return watchFolders; }
            set
            {
                if (value != watchFolders)
                {
                    watchFolders = value;
                    Notify(nameof(WatchFolders));
                }
            }
        }

        private List<string> validExtensions = new List<string>();
        /// <summary>
        /// Gets or sets the valid extensions.
        /// </summary>        
        [Trackable]
        public List<string> ValidExtensions
        {
            get { return validExtensions; }
            set
            {
                if (value != validExtensions)
                {
                    validExtensions = value;
                    Notify(nameof(ValidExtensions));
                }
            }
        }

        private string destinationFolderTV;
        /// <summary>
        /// Gets or sets the destination folder for TV shows.
        /// </summary>
        [Trackable]
        public string DestinationFolderTV
        {
            get { return destinationFolderTV; }
            set
            {
                if (value != destinationFolderTV)
                {
                    destinationFolderTV = value;
                    Notify(nameof(DestinationFolderTV));
                }
            }
        }

        private string destinationFolderMovie;
        /// <summary>
        /// Gets or sets the destination folder for movies.
        /// </summary>
        [Trackable]
        public string DestinationFolderMovie
        {
            get { return destinationFolderMovie; }
            set
            {
                if (value != destinationFolderMovie)
                {
                    destinationFolderMovie = value;
                    Notify(nameof(DestinationFolderMovie));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {

        }
    }
}

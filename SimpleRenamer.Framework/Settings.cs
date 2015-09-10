﻿using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleRenamer.Framework
{
    public class Settings : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged implementation

        private bool subDirectories;
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

        private bool renameFiles;
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

        private bool copyFiles;
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

        private string newFileNameFormat;
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

        public List<string> WatchFolders { get; set; }
        public List<string> ValidExtensions { get; set; }

        private string destinationFolder;

        public string DestinationFolder
        {
            get { return destinationFolder; }
            set
            {
                if (value != destinationFolder)
                {
                    destinationFolder = value;
                    Notify("DestinationFolder");
                }
            }
        }
    }
}

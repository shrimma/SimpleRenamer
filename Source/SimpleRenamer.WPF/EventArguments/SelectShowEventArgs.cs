using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.EventArguments
{
    public class SelectShowEventArgs : EventArgs
    {
        private string id;
        private FileType type;

        public SelectShowEventArgs(string showId, FileType fileType)
        {
            id = showId;
            type = fileType;
        }

        public string ID
        {
            get { return id; }
        }

        public FileType Type
        {
            get { return type; }
        }
    }
}

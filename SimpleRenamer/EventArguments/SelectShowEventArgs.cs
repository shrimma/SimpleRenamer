using SimpleRenamer.Framework.DataModel;
using System;

namespace SimpleRenamer.EventArguments
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

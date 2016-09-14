using SimpleRenamer.Framework.DataModel;
using System;

namespace SimpleRenamer.EventArguments
{
    public class EditShowEventArgs : EventArgs
    {
        private Mapping mapping;
        private string newFolder;

        public EditShowEventArgs(Mapping m, string newFolderName)
        {
            mapping = m;
            newFolder = newFolderName;
        }

        public Mapping Mapping
        {
            get { return mapping; }
        }

        public string NewFolder
        {
            get { return newFolder; }
        }
    }
}

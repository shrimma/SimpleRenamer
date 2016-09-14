using System;

namespace SimpleRenamer.EventArguments
{
    public class SelectShowEventArgs : EventArgs
    {
        private string id;
        public SelectShowEventArgs(string s)
        {
            id = s;
        }

        public string ID
        {
            get { return id; }
        }
    }
}

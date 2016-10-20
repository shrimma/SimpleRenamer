using System;

namespace SimpleRenamer.EventArguments
{
    public class SelectMovieEventArgs : EventArgs
    {
        private string id;
        public SelectMovieEventArgs(string s)
        {
            id = s;
        }

        public string ID
        {
            get { return id; }
        }
    }
}

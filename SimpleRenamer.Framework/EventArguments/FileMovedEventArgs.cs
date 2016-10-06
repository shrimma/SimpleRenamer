using SimpleRenamer.Framework.DataModel;
using System;

namespace SimpleRenamer.Framework.EventArguments
{
    public class FileMovedEventArgs : EventArgs
    {
        private MatchedFile tvEpisode;
        public FileMovedEventArgs(MatchedFile tvEp)
        {
            tvEpisode = tvEp;
        }

        public MatchedFile Episode
        {
            get { return tvEpisode; }
        }
    }
}

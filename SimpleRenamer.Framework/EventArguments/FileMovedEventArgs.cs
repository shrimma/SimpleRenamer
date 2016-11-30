using SimpleRenamer.Common.Model;
using System;

namespace SimpleRenamer.Common.EventArguments
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

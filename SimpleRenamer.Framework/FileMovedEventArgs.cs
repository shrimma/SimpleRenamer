using SimpleRenamer.Framework.DataModel;
using System;

namespace SimpleRenamer.Framework
{
    public class FileMovedEventArgs : EventArgs
    {
        private TVEpisode tvEpisode;
        public FileMovedEventArgs(TVEpisode tvEp)
        {
            tvEpisode = tvEp;
        }

        public TVEpisode Episode
        {
            get { return tvEpisode; }
        }
    }
}

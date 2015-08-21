
using System.Collections.Generic;
namespace SimpleRenamer.Framework
{
    public class Settings
    {
        public bool SubDirectories { get; set; }
        public bool RenameFiles { get; set; }
        public bool CopyFiles { get; set; }
        public string NewFileNameFormat { get; set; }
        public List<string> WatchFolders { get; set; }
        public List<string> ValidExtensions { get; set; }
        public string DestinationFolder { get; set; }
    }
}

using System.Collections.Generic;

namespace SimpleRenamer.Framework.Interface
{
    public interface ISettings
    {
        bool SubDirectories { get; set; }
        bool RenameFiles { get; set; }
        bool CopyFiles { get; set; }
        string NewFileNameFormat { get; set; }
        List<string> WatchFolders { get; set; }
        List<string> ValidExtensions { get; set; }
        string DestinationFolder { get; set; }
    }
}

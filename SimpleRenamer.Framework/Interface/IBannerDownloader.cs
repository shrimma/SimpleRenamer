﻿using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IBannerDownloader
    {
        /// <summary>
        /// Saves a copy of a banner in a folder
        /// </summary>
        /// <param name="tvdbBannerPath">The remote TVDB banner path</param>
        /// <param name="destinationFolder">The destination folder to save the banner</param>
        /// <returns></returns>
        Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder, CancellationToken ct);
    }
}

﻿using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableBannerDownloader : BannerDownloader
    {
        public TestableBannerDownloader(ILogger logger, ITvdbManager tvdbManager) : base(logger, tvdbManager)
        {
        }

        protected override Task<bool> DownloadItem(Uri tvdbUri, string bannerFilePath)
        {
            return Task.FromResult(true);
        }
    }
}

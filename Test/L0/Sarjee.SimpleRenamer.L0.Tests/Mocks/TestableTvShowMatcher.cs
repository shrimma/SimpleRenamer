﻿using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableTvShowMatcher : TVShowMatcher
    {
        public TestableTvShowMatcher(ILogger logger, IConfigurationManager configManager, ITvdbManager tvdbManager, IHelper helper) : base(logger, configManager, tvdbManager, helper)
        {
        }

        protected override BitmapImage InitializeBannerImage(Uri uri)
        {
            BitmapImage bitmapImage = new BitmapImage
            {
                BaseUri = new Uri("http://www.uri.com")
            };

            return bitmapImage;
        }
    }
}

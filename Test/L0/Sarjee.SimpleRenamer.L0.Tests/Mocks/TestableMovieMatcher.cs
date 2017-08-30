using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Framework.Movie;
using System;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableMovieMatcher : MovieMatcher
    {
        public TestableMovieMatcher(ILogger logger, ITmdbManager tmdbManager, IHelper helper) : base(logger, tmdbManager, helper)
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

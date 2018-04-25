using LazyCache;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Framework.Movie;
using System;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableMovieMatcher : MovieMatcher
    {
        public TestableMovieMatcher(ILogger logger, ITmdbManager tmdbManager, IHelper helper, IAppCache cache) : base(logger, tmdbManager, helper, cache)
        {
        }
    }
}

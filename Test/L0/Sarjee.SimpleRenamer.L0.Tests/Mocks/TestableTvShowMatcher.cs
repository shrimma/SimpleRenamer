using LazyCache;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableTvShowMatcher : TVShowMatcher
    {
        public TestableTvShowMatcher(ILogger logger, IConfigurationManager configManager, ITvdbManager tvdbManager, IHelper helper, IAppCache cache) : base(logger, configManager, tvdbManager, helper, cache)
        {
        }
    }
}

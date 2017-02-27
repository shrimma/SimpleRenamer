using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class TvShowMatcherTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TVShowMatcherCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ITVShowMatcher tvShowMatcher = new TVShowMatcher(null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TVShowMatcherCtor_NullTvdbManager_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ITVShowMatcher tvShowMatcher = new TVShowMatcher(configManager, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TVShowMatcherCtor_NullLogger_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ITvdbManager tvdbManager = new Mock<ITvdbManager>().Object;
            ITVShowMatcher tvShowMatcher = new TVShowMatcher(configManager, tvdbManager, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcherCtor_Success()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ITvdbManager tvdbManager = new Mock<ITvdbManager>().Object;
            ILogger logger = new Mock<ILogger>().Object;
            ITVShowMatcher tvShowMatcher = new TVShowMatcher(configManager, tvdbManager, logger);

            Assert.IsNotNull(tvShowMatcher);
        }
        #endregion Constructor
    }
}

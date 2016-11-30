using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.L0;
using System;

namespace SimpleRenamer.Framework.Movie.L0
{
    [TestClass]
    public class TmdbManagerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TmdbManagerCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ITmdbManager tmdbManager = new TmdbManager(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TmdbManagerCtor_NullRetryHelper_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ITmdbManager tmdbManager = new TmdbManager(configManager, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManagerCtor_Success()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IRetryHelper retryHelper = new Mock<IRetryHelper>().Object;
            ITmdbManager tmdbManager = new TmdbManager(configManager, retryHelper);

            Assert.IsNotNull(tmdbManager);
        }
    }
}

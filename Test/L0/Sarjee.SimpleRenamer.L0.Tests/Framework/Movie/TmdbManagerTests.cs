using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Framework.Movie;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Movie
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

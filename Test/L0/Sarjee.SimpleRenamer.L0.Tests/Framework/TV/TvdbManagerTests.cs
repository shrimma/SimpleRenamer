using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class TvdbManagerTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TvdbManagerCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ITvdbManager tvdbManager = new TvdbManager(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TvdbManagerCtor_NullRetryHelper_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ITvdbManager tvdbManager = new TvdbManager(configManager, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TvdbManagerCtor_Success()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IRetryHelper retryHelper = new Mock<IRetryHelper>().Object;
            ITvdbManager tvdbManager = new TvdbManager(configManager, null);

            //we shouldnt get here so throw if we do
            Assert.IsNotNull(tvdbManager);
        }
        #endregion Constructor
    }
}

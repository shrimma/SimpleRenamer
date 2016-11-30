using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.Framework.TV;
using SimpleRenamer.L0;
using System;

namespace SimpleRenamer.Framework.L0
{
    [TestClass]
    public class TvdbManagerTests
    {
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
    }
}

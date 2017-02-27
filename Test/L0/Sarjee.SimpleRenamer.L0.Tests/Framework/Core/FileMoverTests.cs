using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class FileMoverTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMoverCtor_NullBannerDownloader_ThrowsArgumentNullException()
        {
            IFileMover fileMover = new FileMover(null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMoverCtor_NullLogger_ThrowsArgumentNullException()
        {
            IBannerDownloader bannerDownloader = new Mock<IBannerDownloader>().Object;
            IFileMover fileMover = new FileMover(bannerDownloader, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMoverCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            IBannerDownloader bannerDownloader = new Mock<IBannerDownloader>().Object;
            ILogger logger = new Mock<ILogger>().Object;
            IFileMover fileMover = new FileMover(bannerDownloader, logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMoverCtor_Success()
        {
            IBannerDownloader bannerDownloader = new Mock<IBannerDownloader>().Object;
            ILogger logger = new Mock<ILogger>().Object;
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IFileMover fileMover = new FileMover(bannerDownloader, logger, configManager);

            Assert.IsNotNull(fileMover);
        }
        #endregion Constructor
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.L0;
using System;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.TV.L0
{
    [TestClass]
    public class BannerDownloaderTest
    {
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BannerDownloaderCtor_NullLogger_ThrowsArgumentNullException()
        {
            IBannerDownloader bannerDownloader = new BannerDownloader(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BannerDownloaderCtor_NullTvdbManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IBannerDownloader bannerDownloader = new BannerDownloader(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void BannerDownloaderCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            ITvdbManager tvdbManager = new Mock<ITvdbManager>().Object;
            IBannerDownloader bannerDownloader = new BannerDownloader(logger, tvdbManager);

            Assert.IsNotNull(bannerDownloader);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [Ignore]
        public async Task BannerDownloader_SaveBannerAsync_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            ITvdbManager tvdbManager = new Mock<ITvdbManager>().Object;
            IBannerDownloader bannerDownloader = new BannerDownloader(logger, tvdbManager);

            Assert.IsNotNull(bannerDownloader);

            bool result = await bannerDownloader.SaveBannerAsync("", "");

            Assert.IsTrue(result);
        }
    }
}

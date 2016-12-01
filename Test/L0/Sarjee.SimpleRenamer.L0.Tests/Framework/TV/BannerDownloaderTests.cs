using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
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

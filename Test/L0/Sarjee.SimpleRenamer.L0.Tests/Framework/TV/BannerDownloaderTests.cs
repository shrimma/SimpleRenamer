using FluentAssertions;
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
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<ITvdbManager> mockTvdbManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockTvdbManager = mockRepository.Create<ITvdbManager>();
        }

        private IBannerDownloader GetBannerDownloader()
        {
            IBannerDownloader bannerDownloader = new BannerDownloader(mockLogger.Object, mockTvdbManager.Object);
            bannerDownloader.Should().NotBeNull();
            return bannerDownloader;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void BannerDownloaderCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new BannerDownloader(null, null);
            Action action2 = () => new BannerDownloader(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void BannerDownloaderCtor_Success()
        {
            IBannerDownloader bannerDownloader = null;
            Action action1 = () => bannerDownloader = GetBannerDownloader();

            action1.ShouldNotThrow();
            bannerDownloader.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [Ignore]
        public async Task BannerDownloader_SaveBannerAsync_Success()
        {
            IBannerDownloader bannerDownloader = GetBannerDownloader();

            bool result = await bannerDownloader.SaveBannerAsync("", "");

            Assert.IsTrue(result);
        }
    }
}

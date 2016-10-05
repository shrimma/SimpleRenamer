using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Framework.Interface;

namespace SimpleRenamer.Framework.L0
{
    [TestClass]
    public class BannerDownloaderTest
    {
        [TestMethod]
        public void ConstructorValid()
        {
            var configurationManager = new Mock<IConfigurationManager>();
            var logger = new Mock<ILogger>();
            IBannerDownloader bannerDownloader = new BannerDownloader(configurationManager.Object, logger.Object);
        }
    }
}

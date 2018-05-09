using FluentAssertions;
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
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IBannerDownloader> mockBannerDownloader;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockBannerDownloader = mockRepository.Create<IBannerDownloader>();
        }

        private IFileMover GetFileMover()
        {
            IFileMover fileMover = new FileMover(mockLogger.Object, mockConfigurationManager.Object, mockBannerDownloader.Object);
            fileMover.Should().NotBeNull();
            return fileMover;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMoverCtor_NullBannerDownloader_ThrowsArgumentNullException()
        {
            Action action1 = () => new FileMover(null, null, null);
            Action action2 = () => new FileMover(mockLogger.Object, null, null);
            Action action3 = () => new FileMover(mockLogger.Object, mockConfigurationManager.Object, null);

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMoverCtor_Success()
        {
            IFileMover fileMover = null;
            Action action1 = () => fileMover = GetFileMover();

            action1.Should().NotThrow();
            fileMover.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

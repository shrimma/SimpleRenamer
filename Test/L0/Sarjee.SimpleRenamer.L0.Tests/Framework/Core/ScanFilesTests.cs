using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class ScanFilesTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IFileWatcher> mockFileWatcher;
        private Mock<ITVShowMatcher> mockShowMatcher;
        private Mock<IMovieMatcher> mockMovieMatcher;
        private Mock<IFileMatcher> mockFileMatcher;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockFileWatcher = mockRepository.Create<IFileWatcher>();
            mockShowMatcher = mockRepository.Create<ITVShowMatcher>();
            mockMovieMatcher = mockRepository.Create<IMovieMatcher>();
            mockFileMatcher = mockRepository.Create<IFileMatcher>();
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new ScanFiles(null, null, null, null, null, null);
            Action action2 = () => new ScanFiles(mockLogger.Object, null, null, null, null, null);
            Action action3 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, null, null, null, null);
            Action action4 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, null, null, null);
            Action action5 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, null, null);
            Action action6 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, mockMovieMatcher.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
            action4.ShouldThrow<ArgumentNullException>();
            action5.ShouldThrow<ArgumentNullException>();
            action6.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_Success()
        {
            IScanFiles scanFiles = null;
            Action action1 = () => scanFiles = new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, mockMovieMatcher.Object, mockFileMatcher.Object);

            action1.ShouldNotThrow();
            scanFiles.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

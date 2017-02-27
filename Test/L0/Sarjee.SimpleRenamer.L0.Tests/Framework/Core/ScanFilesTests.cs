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
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullLogger_ThrowsArgumentNullException()
        {
            IScanFiles scanFiles = new ScanFiles(null, null, null, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullFileWatcher_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, null, null, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullTvShowMatcher_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new Mock<IFileWatcher>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, fileWatcher, null, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullMovieMatcher_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new Mock<IFileWatcher>().Object;
            ITVShowMatcher tvShowMatcher = new Mock<ITVShowMatcher>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, fileWatcher, tvShowMatcher, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullFileMatcher_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new Mock<IFileWatcher>().Object;
            ITVShowMatcher tvShowMatcher = new Mock<ITVShowMatcher>().Object;
            IMovieMatcher movieMatcher = new Mock<IMovieMatcher>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, fileWatcher, tvShowMatcher, movieMatcher, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScanFilesCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new Mock<IFileWatcher>().Object;
            ITVShowMatcher tvShowMatcher = new Mock<ITVShowMatcher>().Object;
            IMovieMatcher movieMatcher = new Mock<IMovieMatcher>().Object;
            IFileMatcher fileMatcher = new Mock<IFileMatcher>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, fileWatcher, tvShowMatcher, movieMatcher, fileMatcher, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new Mock<IFileWatcher>().Object;
            ITVShowMatcher tvShowMatcher = new Mock<ITVShowMatcher>().Object;
            IMovieMatcher movieMatcher = new Mock<IMovieMatcher>().Object;
            IFileMatcher fileMatcher = new Mock<IFileMatcher>().Object;
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IScanFiles scanFiles = new ScanFiles(logger, fileWatcher, tvShowMatcher, movieMatcher, fileMatcher, configManager);

            Assert.IsNotNull(scanFiles);
        }
        #endregion Constructor
    }
}

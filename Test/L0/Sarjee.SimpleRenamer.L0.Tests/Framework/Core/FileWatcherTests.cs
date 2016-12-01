using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class FileWatcherTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileWatcherCtor_NullLogger_ThrowsArgumentNullException()
        {
            IFileWatcher fileWatcher = new FileWatcher(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileWatcherCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileWatcher fileWatcher = new FileWatcher(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcherCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IFileWatcher fileWatcher = new FileWatcher(logger, configManager);

            Assert.IsNotNull(fileWatcher);
        }
        #endregion Constructor
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public async Task FileWatcher_SearchTheseFoldersAsync_NoWatchFolders_ReturnsEmptyList()
        {
            ILogger logger = new Mock<ILogger>().Object;
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.Settings).Returns(new Settings() { WatchFolders = new List<string>() });
            IConfigurationManager configManager = config.Object;
            IFileWatcher fileWatcher = new FileWatcher(logger, configManager);
            fileWatcher.RaiseProgressEvent += FileWatcher_RaiseProgressEvent;

            Assert.IsNotNull(fileWatcher);

            List<string> filesFound = await fileWatcher.SearchTheseFoldersAsync(new System.Threading.CancellationToken());

            List<string> emptyList = new List<string>();
            Assert.AreEqual<int>(emptyList.Count, filesFound.Count);
            Assert.AreEqual<List<string>>(emptyList, filesFound);
        }

        private void FileWatcher_RaiseProgressEvent(object sender, SimpleRenamer.Common.EventArguments.ProgressTextEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }
    }
}

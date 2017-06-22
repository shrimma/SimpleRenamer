using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class FileWatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private static string _path = Path.Combine(Environment.CurrentDirectory, "L0FileWatcherTests");
        private static string validFileName = "Castle.S01E01.mkv";
        private static string invalidFileName = "Castle.S01E01.txt";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            //create dummy files
            string fileName = Path.Combine(_path, validFileName);
            if (File.Exists(fileName))
            {
                // Note that no lock is put on the
                // file and the possibility exists
                // that another process could do
                // something with it between
                // the calls to Exists and Delete.
                File.Delete(fileName);
            }
            using (FileStream fs = File.Create(fileName))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
            fileName = Path.Combine(_path, invalidFileName);
            if (File.Exists(fileName))
            {
                // Note that no lock is put on the
                // file and the possibility exists
                // that another process could do
                // something with it between
                // the calls to Exists and Delete.
                File.Delete(fileName);
            }
            using (FileStream fs = File.Create(fileName))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            //delete dummy files            
            string fileName = Path.Combine(_path, validFileName);
            if (File.Exists(fileName))
            {
                // Note that no lock is put on the
                // file and the possibility exists
                // that another process could do
                // something with it between
                // the calls to Exists and Delete.
                File.Delete(fileName);
            }

            fileName = Path.Combine(_path, invalidFileName);
            if (File.Exists(fileName))
            {
                // Note that no lock is put on the
                // file and the possibility exists
                // that another process could do
                // something with it between
                // the calls to Exists and Delete.
                File.Delete(fileName);
            }

            if (Directory.Exists(_path))
            {
                Directory.Delete(_path);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
        }

        private IFileWatcher GetFileWatcher()
        {
            IFileWatcher fileWatcher = new FileWatcher(mockLogger.Object, mockConfigurationManager.Object);
            fileWatcher.Should().NotBeNull();
            fileWatcher.RaiseProgressEvent += FileWatcher_RaiseProgressEvent;
            return fileWatcher;
        }
        private void FileWatcher_RaiseProgressEvent(object sender, SimpleRenamer.Common.EventArguments.ProgressTextEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcherCtor_NullArgument_ThrowsArgumentNullException()
        {
            Action action1 = () => new FileWatcher(null, null);
            Action action2 = () => new FileWatcher(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcherCtor_Success()
        {
            IFileWatcher fileWatcher = null;
            Action action1 = () => fileWatcher = GetFileWatcher();

            action1.ShouldNotThrow();
            fileWatcher.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcher_SearchTheseFoldersAsync_NoWatchFolders_ReturnsEmptyList()
        {
            mockConfigurationManager.SetupGet(x => x.Settings).Returns(new Settings() { WatchFolders = new List<string>() });
            IFileWatcher fileWatcher = GetFileWatcher();

            List<string> emptyList = new List<string>();
            List<string> filesFound = null;
            Func<Task> action1 = async () => filesFound = await fileWatcher.SearchTheseFoldersAsync(new System.Threading.CancellationToken());
            action1.ShouldNotThrow();

            filesFound.Count.Should().Be(emptyList.Count);
            filesFound.ShouldBeEquivalentTo(emptyList);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcher_SearchTheseFoldersAsync_Success()
        {
            mockConfigurationManager.SetupGet(x => x.Settings).Returns(new Settings() { WatchFolders = new List<string> { _path }, ValidExtensions = new List<string> { ".mkv" } });
            mockConfigurationManager.SetupGet(x => x.IgnoredFiles).Returns(new IgnoreList());
            IFileWatcher fileWatcher = GetFileWatcher();

            List<string> filesFound = null;
            Func<Task> action1 = async () => filesFound = await fileWatcher.SearchTheseFoldersAsync(new System.Threading.CancellationToken());
            action1.ShouldNotThrow();

            filesFound.Count.Should().Be(1);
        }
    }
}

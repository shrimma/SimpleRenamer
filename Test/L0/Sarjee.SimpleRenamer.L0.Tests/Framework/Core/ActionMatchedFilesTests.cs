using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class ActionMatchedFilesTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionMatchedFilesCtor_NullLogger_ThrowsArgumentNullException()
        {
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(null, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionMatchedFilesCtor_NullBackgroundQueue_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(logger, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionMatchedFilesCtor_NullFileMover_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IBackgroundQueue backgroundQueue = new Mock<IBackgroundQueue>().Object;
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(logger, backgroundQueue, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActionMatchedFilesCtor_NullConfigurationManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IBackgroundQueue backgroundQueue = new Mock<IBackgroundQueue>().Object;
            IFileMover fileMover = new Mock<IFileMover>().Object;
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(logger, backgroundQueue, fileMover, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IBackgroundQueue backgroundQueue = new Mock<IBackgroundQueue>().Object;
            IFileMover fileMover = new Mock<IFileMover>().Object;
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(logger, backgroundQueue, fileMover, configManager);

            Assert.IsNotNull(actionMatchedFiles);
        }
        #endregion Constructor
    }
}

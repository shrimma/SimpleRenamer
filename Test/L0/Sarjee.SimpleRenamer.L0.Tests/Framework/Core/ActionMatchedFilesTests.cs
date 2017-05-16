using FluentAssertions;
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
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IBackgroundQueue> mockBackgroundQueue;
        private Mock<IFileMover> mockFileMover;
        private Mock<IConfigurationManager> mockConfigurationManager;


        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockBackgroundQueue = mockRepository.Create<IBackgroundQueue>();
            mockFileMover = mockRepository.Create<IFileMover>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_NullArgument_ThrowsArgumentNullException()
        {
            Action action1 = () => new ActionMatchedFiles(null, null, null, null);
            Action action2 = () => new ActionMatchedFiles(mockLogger.Object, null, null, null);
            Action action3 = () => new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, null, null);
            Action action4 = () => new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, mockFileMover.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
            action4.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_Success()
        {
            IActionMatchedFiles actionMatchedFiles = null;
            Action action1 = () => actionMatchedFiles = new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, mockFileMover.Object, mockConfigurationManager.Object);

            action1.ShouldNotThrow();
            actionMatchedFiles.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

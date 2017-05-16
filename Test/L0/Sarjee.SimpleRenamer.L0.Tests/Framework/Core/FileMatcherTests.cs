using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class FileMatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new FileMatcher(null, null);
            Action action2 = () => new FileMatcher(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_Success()
        {
            IFileMatcher fileMatcher = null;
            Action action1 = () => fileMatcher = new FileMatcher(mockLogger.Object, mockConfigurationManager.Object);

            action1.ShouldNotThrow();
            fileMatcher.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Logging;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Logging
{
    [TestClass]
    public class LoggerTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<IConfigurationManager> mockConfigurationManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockConfigurationManager.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            mockConfigurationManager.SetupGet(x => x.OneTrueErrorApplicationKey).Returns("123456789");
            mockConfigurationManager.SetupGet(x => x.OneTrueErrorSharedSecret).Returns("987654321");
        }

        private ILogger GetLogger()
        {
            ILogger logger = new Logger(mockConfigurationManager.Object);
            logger.Should().NotBeNull();
            return logger;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            Action action1 = () => new Logger(null);

            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_NullConfigOTEUrl_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            Action action1 = () => new Logger(configManager);

            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_NullConfigOTEApplication_ThrowsArgumentNullException()
        {
            //setup config
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            IConfigurationManager configManager = config.Object;
            Action action1 = () => new Logger(configManager);

            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_NullConfigOTESharedSecret_ThrowsArgumentNullException()
        {
            //setup config
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            config.SetupGet(x => x.OneTrueErrorApplicationKey).Returns("123456789");
            IConfigurationManager configManager = config.Object;
            Action action1 = () => new Logger(configManager);

            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_Success()
        {
            ILogger logger = null;
            Action action1 = () => logger = GetLogger();

            action1.ShouldNotThrow();
            logger.Should().NotBeNull();
        }
        #endregion Constructor

        #region TraceMessage
        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void Logger_TraceMessage_Success()
        {
            ILogger logger = GetLogger();
            Action action1 = () => logger.TraceMessage("hello world");

            action1.ShouldNotThrow();
        }
        #endregion TraceMessage

        #region TraceException
        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void Logger_TraceException_Success()
        {
            ILogger logger = GetLogger();

            Action action1 = () => logger.TraceException(new ArgumentNullException("lolol"));
            action1.ShouldNotThrow();
        }
        #endregion TraceException
    }
}

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
        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoggerCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Logger(null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoggerCtor_NullConfigOTEUrl_ThrowsArgumentNullException()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ILogger logger = new Logger(configManager);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoggerCtor_NullConfigOTEApplication_ThrowsArgumentNullException()
        {
            //setup config
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            IConfigurationManager configManager = config.Object;
            ILogger logger = new Logger(configManager);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoggerCtor_NullConfigOTESharedSecret_ThrowsArgumentNullException()
        {
            //setup config
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            config.SetupGet(x => x.OneTrueErrorApplicationKey).Returns("123456789");
            IConfigurationManager configManager = config.Object;
            ILogger logger = new Logger(configManager);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Logger)]
        public void LoggerCtor_Success()
        {
            //setup config
            var config = new Mock<IConfigurationManager>();
            config.SetupGet(x => x.OneTrueErrorUrl).Returns("http://localhost/OTE/");
            config.SetupGet(x => x.OneTrueErrorApplicationKey).Returns("123456789");
            config.SetupGet(x => x.OneTrueErrorSharedSecret).Returns("987654321");
            IConfigurationManager configManager = config.Object;
            ILogger logger = new Logger(configManager);

            //check we get a valid object back
            Assert.IsNotNull(logger);
        }
    }
}

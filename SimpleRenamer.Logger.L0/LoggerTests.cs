using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;
using System;

namespace SimpleRenamer.Framework.L0
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
        public void LoggerCtor_Success()
        {
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            ILogger logger = new Logger(configManager);

            //we shouldnt get here so throw if we do
            Assert.IsNotNull(logger);
        }
    }
}

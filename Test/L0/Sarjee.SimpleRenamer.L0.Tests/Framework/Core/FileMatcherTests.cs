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
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMatcherCtor_NullLogger_ThrowsArgumentNullException()
        {
            IFileMatcher fileMatcher = new FileMatcher(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMatcherCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IFileMatcher fileMatcher = new FileMatcher(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IConfigurationManager configManager = new Mock<IConfigurationManager>().Object;
            IFileMatcher fileMatcher = new FileMatcher(logger, configManager);

            Assert.IsNotNull(fileMatcher);
        }
        #endregion Constructor
    }
}

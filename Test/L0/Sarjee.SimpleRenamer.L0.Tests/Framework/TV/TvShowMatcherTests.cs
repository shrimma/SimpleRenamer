using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class TvShowMatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<ITvdbManager> mockTvdbManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockTvdbManager = mockRepository.Create<ITvdbManager>();
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new TVShowMatcher(null, null, null);
            Action action2 = () => new TVShowMatcher(mockLogger.Object, null, null);
            Action action3 = () => new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcherCtor_Success()
        {
            ITVShowMatcher tvShowMatcher = null;
            Action action1 = () => tvShowMatcher = new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, mockTvdbManager.Object);

            action1.ShouldNotThrow();
            tvShowMatcher.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

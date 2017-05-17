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
    public class TvdbManagerTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IRetryHelper> mockRetryHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockRetryHelper = mockRepository.Create<IRetryHelper>();
        }

        private ITvdbManager GetTvdbManager()
        {
            ITvdbManager tvdbManager = new TvdbManager(mockConfigurationManager.Object, mockRetryHelper.Object);
            tvdbManager.Should().NotBeNull();
            return tvdbManager;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManagerCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new TvdbManager(null, null);
            Action action2 = () => new TvdbManager(mockConfigurationManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManagerCtor_Success()
        {
            ITvdbManager tvdbManager = null;
            Action action1 = () => tvdbManager = GetTvdbManager();

            action1.ShouldNotThrow();
            tvdbManager.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

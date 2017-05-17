using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Framework.Movie;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Movie
{
    [TestClass]
    public class TmdbManagerTests
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

        private ITmdbManager GetTmdbManager()
        {
            ITmdbManager tmdbManager = new TmdbManager(mockConfigurationManager.Object, mockRetryHelper.Object);
            tmdbManager.Should().NotBeNull();
            return tmdbManager;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManagerCtor_NullConfigManager_ThrowsArgumentNullException()
        {
            Action action1 = () => new TmdbManager(null, null);
            Action action2 = () => new TmdbManager(mockConfigurationManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManagerCtor_Success()
        {
            ITmdbManager tmdbManager = null;
            Action action1 = () => tmdbManager = GetTmdbManager();

            action1.ShouldNotThrow();
            tmdbManager.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

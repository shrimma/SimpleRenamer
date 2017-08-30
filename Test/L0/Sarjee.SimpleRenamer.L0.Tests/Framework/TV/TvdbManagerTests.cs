using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.Framework.TV;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class TvdbManagerTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IHelper> mockHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockHelper = mockRepository.Create<IHelper>();
        }

        private ITvdbManager GetTvdbManager(bool getTestableVersion = false)
        {
            ITvdbManager tvdbManager = null;
            if (getTestableVersion)
            {
                tvdbManager = new TestableTvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            }
            else
            {
                tvdbManager = new TvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            }
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

        #region SearchSeriesByNameAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_SearchSeriesByNameAsync_NullArgument_ThrowsANE()
        {
            ITvdbManager tvdbManager = GetTvdbManager();
            List<SeriesSearchData> result = null;
            Func<Task> action1 = async () => result = await tvdbManager.SearchSeriesByNameAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_SearchSeriesByNameAsync_Success()
        {
            ITvdbManager tvdbManager = GetTvdbManager(true);
            List<SeriesSearchData> result = null;
            Func<Task> action1 = async () => result = await tvdbManager.SearchSeriesByNameAsync("seriesName");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.Should().Contain(x => x.SeriesName.Equals("Castle"));
            result.Should().Contain(x => x.SeriesName.Equals("KillJoys"));
        }
        #endregion SearchSeriesByNameAsync

        #region GetSeriesByIdAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetSeriesByIdAsync_NullArgument_ThrowsANE()
        {
            ITvdbManager tvdbManager = GetTvdbManager();
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetSeriesByIdAsync_Success()
        {
            ITvdbManager tvdbManager = GetTvdbManager(true);
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync("tmdbId");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Series.SeriesName.Should().Be("Game of Thrones");
            result.Actors.Count.Should().Be(2);
            result.Episodes.Count.Should().Be(3);
            result.SeriesBanners.Count.Should().Be(30);
            result.SeasonPosters.Count.Should().Be(30);
            result.Posters.Count.Should().Be(30);
        }
        #endregion GetSeriesByIdAsync

        #region GetBannerUri
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetBannerUri_NullArgument_ThrowsANE()
        {
            ITvdbManager tvdbManager = GetTvdbManager();
            string result = null;
            Action action1 = () => result = tvdbManager.GetBannerUri(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetBannerUri_Success()
        {
            ITvdbManager tvdbManager = GetTvdbManager();
            string result = null;
            Action action1 = () => result = tvdbManager.GetBannerUri("bannerPath");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Should().Contain("bannerPath");
            result.Should().Contain("tvdb");
        }
        #endregion GetBannerUri

        #region RetryHandling
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_RetryHandling_StatusCode_ReturnsNull_Success()
        {
            //get testable tmdbmanager
            ITvdbManager tvdbManager = new ErrorCodeTestableTvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync("tmdbId");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_RetryHandling_WebException_ReturnsNull_Success()
        {
            //get testable tmdbmanager
            ITvdbManager tvdbManager = new WebExceptionTestableTvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync("tmdbId");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_RetryHandling_Unauthorized_ReturnsNull_Success()
        {
            //get testable tmdbmanager
            ITvdbManager tvdbManager = new UnauthorizedTestableTvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            List<SeriesSearchData> result = null;
            Func<Task> action1 = async () => result = await tvdbManager.SearchSeriesByNameAsync("name");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_RetryHandling_ErrorException_ThrowsException_Success()
        {
            //get testable tmdbmanager
            ITvdbManager tvdbManager = new ErrorExceptionTestableTvdbManager(mockConfigurationManager.Object, mockHelper.Object);
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync("tmdbId");

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }
        #endregion RetryHandling
    }
}

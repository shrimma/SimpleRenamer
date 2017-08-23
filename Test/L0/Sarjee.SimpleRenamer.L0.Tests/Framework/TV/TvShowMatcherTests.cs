using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class TvShowMatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<ITvdbManager> mockTvdbManager;
        private Mock<IHelper> mockHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockTvdbManager = mockRepository.Create<ITvdbManager>();
            mockHelper = mockRepository.Create<IHelper>();
        }

        private ITVShowMatcher GetTVShowMatcher()
        {
            ITVShowMatcher tvShowMatcher = new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, mockTvdbManager.Object, mockHelper.Object);
            tvShowMatcher.Should().NotBeNull();
            tvShowMatcher.RaiseProgressEvent += TvShowMatcher_RaiseProgressEvent;
            return tvShowMatcher;
        }

        private void TvShowMatcher_RaiseProgressEvent(object sender, SimpleRenamer.Common.EventArguments.ProgressTextEventArgs e)
        {
            //do nothing but need this
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new TVShowMatcher(null, null, null, null);
            Action action2 = () => new TVShowMatcher(mockLogger.Object, null, null, null);
            Action action3 = () => new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, null, null);
            Action action4 = () => new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, mockTvdbManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
            action4.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcherCtor_Success()
        {
            ITVShowMatcher tvShowMatcher = null;
            Action action1 = () => tvShowMatcher = GetTVShowMatcher();

            action1.ShouldNotThrow();
            tvShowMatcher.Should().NotBeNull();
        }
        #endregion Constructor

        #region SearchShowByNameAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByNameAsync_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByNameAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByNameAsync_SingleResult_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            SeriesSearchData searchSeries1 = new SeriesSearchData() { SeriesName = "Series1", Id = 1 };
            List<SeriesSearchData> outputList = new List<SeriesSearchData>() { searchSeries1 };
            CompleteSeries outputSeries = new CompleteSeries(new Series(1, "Series1"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>());
            mockTvdbManager.Setup(x => x.SearchSeriesByNameAsync(It.IsAny<string>())).ReturnsAsync(outputList);
            mockTvdbManager.Setup(x => x.GetSeriesByIdAsync(searchSeries1.Id.ToString())).ReturnsAsync(outputSeries);

            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByNameAsync("showName");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Series.Id.Should().Be(outputSeries.Series.Id);
            result.Series.SeriesName.Should().Be(outputSeries.Series.SeriesName);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByNameAsync_MultipleResults_ReturnsNull()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            SeriesSearchData searchSeries1 = new SeriesSearchData() { SeriesName = "Series1", Id = 1 };
            SeriesSearchData searchSeries2 = new SeriesSearchData() { SeriesName = "Series2", Id = 2 };
            List<SeriesSearchData> outputList = new List<SeriesSearchData>() { searchSeries1, searchSeries2 };
            mockTvdbManager.Setup(x => x.SearchSeriesByNameAsync(It.IsAny<string>())).ReturnsAsync(outputList);

            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByNameAsync("showName");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByNameAsync_NoResults_ReturnsNull()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            mockTvdbManager.Setup(x => x.SearchSeriesByNameAsync(It.IsAny<string>())).ReturnsAsync((List<SeriesSearchData>)null);

            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByNameAsync("showName");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }
        #endregion SearchShowByNameAsync

        #region SearchShowByIdAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByIdAsync_NullArgument_ThrowsANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByIdAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_SearchShowByIdAsync_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            CompleteSeries outputSeries = new CompleteSeries(new Series(1, "Series1"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>());
            mockTvdbManager.Setup(x => x.GetSeriesByIdAsync(It.IsAny<string>())).ReturnsAsync(outputSeries);

            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.SearchShowByIdAsync("showId");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Series.Id.Should().Be(outputSeries.Series.Id);
            result.Series.SeriesName.Should().Be(outputSeries.Series.SeriesName);
        }
        #endregion SearchShowByIdAsync

        #region UpdateFileWithSeriesDetails
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateFileWithSeriesDetails_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile("", "");
            CompleteSeries seriesInput = new CompleteSeries(new Series(), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>());

            MatchedFile result = null;
            Action action1 = () => result = tvShowMatcher.UpdateFileWithSeriesDetails(fileInput, seriesInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
        }
        #endregion UpdateFileWithSeriesDetails

        #region FixShowsFromMappings
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_FixShowsFromMappings_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile("", "");

            MatchedFile result = null;
            Action action1 = () => result = tvShowMatcher.FixShowsFromMappings(fileInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
        }
        #endregion FixShowsFromMappings

        #region GetPossibleShowsForEpisode
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetPossibleShowsForEpisode_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            List<DetailView> result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.GetPossibleShowsForEpisodeAsync("showName");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
        }
        #endregion GetPossibleShowsForEpisode

        #region UpdateEpisodeWithMatchedSeries
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateEpisodeWithMatchedSeries_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile("", "");

            MatchedFile result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.UpdateEpisodeWithMatchedSeriesAsync("tvdbId", fileInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
        }
        #endregion UpdateEpisodeWithMatchedSeries

        #region GetShowWithBannerAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetShowWithBannerAsync_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            (CompleteSeries series, BitmapImage banner) result = (null, null);
            Func<Task> action1 = async () => result = await tvShowMatcher.GetShowWithBannerAsync("showId");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.series.Should().NotBeNull();
            result.banner.Should().NotBeNull();
        }
        #endregion GetShowWithBannerAsync
    }
}

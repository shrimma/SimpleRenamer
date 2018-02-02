using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.Framework.TV;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
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

        private ITVShowMatcher GetTVShowMatcher(bool testable = false)
        {
            ITVShowMatcher tvShowMatcher = null;
            if (testable)
            {
                tvShowMatcher = new TestableTvShowMatcher(mockLogger.Object, mockConfigurationManager.Object, mockTvdbManager.Object, mockHelper.Object);
            }
            else
            {
                tvShowMatcher = new TVShowMatcher(mockLogger.Object, mockConfigurationManager.Object, mockTvdbManager.Object, mockHelper.Object);
            }
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
            //setup tvdbmanager
            CompleteSeries outputSeries = new CompleteSeries(new Series(1, "Series1"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>());
            mockTvdbManager.Setup(x => x.GetSeriesByIdAsync(It.IsAny<string>())).ReturnsAsync(outputSeries);
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

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
        public void TVShowMatcher_UpdateFileWithSeriesDetails_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

            MatchedFile result1 = null;
            MatchedFile result2 = null;
            Action action1 = () => result1 = tvShowMatcher.UpdateFileWithSeriesDetails(null, null);
            Action action2 = () => result2 = tvShowMatcher.UpdateFileWithSeriesDetails(new MatchedFile(@"c:\filePath", "Showname", "1", "1"), null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            result1.Should().BeNull();
            result2.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateFileWithSeriesDetails_Success()
        {
            //setup settings
            Settings settings = new Settings() { NewFileNameFormat = "{ShowName},{Season},{Episode},{EpisodeName}" };
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile("filePath", "ShowName", "1", "1");
            CompleteSeries seriesInput = new CompleteSeries(new Series(1, "ShowName"), new List<SeriesActorsData>(), new List<BasicEpisode>() { new BasicEpisode(1, 1, 1, 1, 1, "EpisodeName", 1, "Overview") }, new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) }, new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) }, new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) });

            MatchedFile result = null;
            Action action1 = () => result = tvShowMatcher.UpdateFileWithSeriesDetails(fileInput, seriesInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.ShowName.Should().Be(seriesInput.Series.SeriesName);
            result.EpisodeName.Should().Be(seriesInput.Episodes[0].EpisodeName);
            result.TVDBShowId.Should().Be(seriesInput.Series.Id.ToString());
            result.ActionThis.Should().BeTrue();
            result.SkippedExactSelection.Should().BeFalse();
        }
        #endregion UpdateFileWithSeriesDetails

        #region FixShowsFromMappings
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_FixShowsFromMappings_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

            MatchedFile result = null;
            Action action1 = () => result = tvShowMatcher.FixShowsFromMappings(null);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_FixShowsFromMappings_Success()
        {
            //setup settings
            Mapping mapping = new Mapping("Showname", "tvdbShowName", "tvdbShowId");
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping() { Mappings = new List<Mapping>() { mapping } });

            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile(@"c:\filePath", "Showname", "1", "1");

            MatchedFile result = null;
            Action action1 = () => result = tvShowMatcher.FixShowsFromMappings(fileInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.ShowName.Should().Be(mapping.TVDBShowName);
            result.TVDBShowId.Should().Be(mapping.TVDBShowID);
        }
        #endregion FixShowsFromMappings

        #region GetPossibleShowsForEpisodeAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetPossibleShowsForEpisodeAsync_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

            List<DetailView> result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.GetPossibleShowsForEpisodeAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetPossibleShowsForEpisodeAsync_Success()
        {
            SeriesSearchData seriesShortSearchData = new SeriesSearchData(null, null, "2017-01-01", 1, null, "shortoverview", "showName", null);
            SeriesSearchData seriesLongSearchData = new SeriesSearchData(null, null, "2017-01-01", 1, null, "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglongestlongestlongestlongestlongestlongestlongestoverview", "showName", null);
            mockTvdbManager.Setup(x => x.SearchSeriesByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<SeriesSearchData>() { seriesShortSearchData, seriesLongSearchData });

            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            List<DetailView> result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.GetPossibleShowsForEpisodeAsync("showName");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }
        #endregion GetPossibleShowsForEpisode

        #region UpdateEpisodeWithMatchedSeriesAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateEpisodeWithMatchedSeriesAsync_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

            MatchedFile result1 = null;
            Func<Task> action1 = async () => result1 = await tvShowMatcher.UpdateEpisodeWithMatchedSeriesAsync("id", null);

            action1.ShouldThrow<ArgumentNullException>();
            result1.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateEpisodeWithMatchedSeriesAsync_NoSeries_Success()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile(@"c:\filePath", "Showname", "1", "1");

            MatchedFile result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.UpdateEpisodeWithMatchedSeriesAsync(string.Empty, fileInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeFalse();
            result.SkippedExactSelection.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_UpdateEpisodeWithMatchedSeriesAsync_Success()
        {
            //mock tvdbmanager
            CompleteSeries outputSeries = new CompleteSeries(new Series(1, "Series1"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>());
            mockTvdbManager.Setup(x => x.GetSeriesByIdAsync(It.IsAny<string>())).ReturnsAsync(outputSeries);
            //mock settings
            Settings settings = new Settings() { NewFileNameFormat = "newFileName" };
            ShowNameMapping mapping = new ShowNameMapping();
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(mapping);

            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();
            MatchedFile fileInput = new MatchedFile(@"c:\filePath", "Showname", "1", "1");

            MatchedFile result = null;
            Func<Task> action1 = async () => result = await tvShowMatcher.UpdateEpisodeWithMatchedSeriesAsync("tvdbId", fileInput);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeTrue();
            result.SkippedExactSelection.Should().BeFalse();
            result.ShowName.Should().Be("Series1");
        }
        #endregion UpdateEpisodeWithMatchedSeries

        #region GetShowWithBannerAsync
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetShowWithBannerAsync_NullArguments_ThrowANE()
        {
            ITVShowMatcher tvShowMatcher = GetTVShowMatcher();

            (CompleteSeries series, Uri banner) result = (null, null);
            Func<Task> action1 = async () => result = await tvShowMatcher.GetShowWithBannerAsync(string.Empty);

            action1.ShouldThrow<ArgumentNullException>();
            result.series.Should().BeNull();
            result.banner.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TVShowMatcher_GetShowWithBannerAsync_Success()
        {
            //mock tvdbmanager
            CompleteSeries outputSeries = new CompleteSeries(new Series(1, "Series1"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) }, new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) }, new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "Season", "1", "fileName", 1, "resolution", new RatingsInfo(10, 1)) });
            mockTvdbManager.Setup(x => x.GetSeriesByIdAsync(It.IsAny<string>())).ReturnsAsync(outputSeries);
            mockTvdbManager.Setup(x => x.GetBannerUri(It.IsAny<string>())).Returns("http://www.uri.com");

            ITVShowMatcher tvShowMatcher = GetTVShowMatcher(true);
            (CompleteSeries series, Uri banner) result = (null, null);
            Func<Task> action1 = async () => result = await tvShowMatcher.GetShowWithBannerAsync("showId");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.series.Should().NotBeNull();
            result.banner.Should().NotBeNull();
            result.series.Series.Id = 1;
        }
        #endregion GetShowWithBannerAsync
    }
}

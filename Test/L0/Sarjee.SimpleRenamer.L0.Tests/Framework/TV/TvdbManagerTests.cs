﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Collections.Generic;
using System.Threading;
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

        private ITvdbManager GetTvdbManager()
        {
            ITvdbManager tvdbManager = new TvdbManager(mockConfigurationManager.Object, mockHelper.Object);
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

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManagerCtor_Success()
        {
            ITvdbManager tvdbManager = null;
            Action action1 = () => tvdbManager = GetTvdbManager();

            action1.Should().NotThrow();
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
            Func<Task> action1 = async () => result = await tvdbManager.SearchSeriesByNameAsync(string.Empty, CancellationToken.None);

            action1.Should().Throw<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_SearchSeriesByNameAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<Token>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new Token("jwtToken"));
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SeriesSearchDataList>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new SeriesSearchDataList() { SearchResults = new List<SeriesSearchData>() { new SeriesSearchData(null, null, null, 1, null, null, "Castle", null), new SeriesSearchData(null, null, null, 2, null, null, "KillJoys", null) } });
            ITvdbManager tvdbManager = GetTvdbManager();

            List<SeriesSearchData> result = null;
            Func<Task> action1 = async () => result = await tvdbManager.SearchSeriesByNameAsync("seriesName", CancellationToken.None);

            action1.Should().NotThrow();
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
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync(string.Empty, CancellationToken.None);

            action1.Should().Throw<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetSeriesByIdAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<Token>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new Token("jwtToken"));
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SeriesData>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new SeriesData() { Data = new Series(1, "Game of Thrones") });
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SeriesActors>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new SeriesActors() { Data = new List<SeriesActorsData>() { new SeriesActorsData(1, 1, "Bob") } });
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SeriesEpisodes>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new SeriesEpisodes() { Data = new List<BasicEpisode>() { new BasicEpisode(1, 1, 1, 1, 1, "EpisodeName", 1, "overview") } });
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SeriesImageQueryResults>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Task>>())).ReturnsAsync(new SeriesImageQueryResults() { Data = new List<SeriesImageQueryResult>() { new SeriesImageQueryResult(1, "keytype", "subkey") } });
            ITvdbManager tvdbManager = GetTvdbManager();

            CompleteSeries result = null;
            Func<Task> action1 = async () => result = await tvdbManager.GetSeriesByIdAsync("tmdbId", CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.Series.SeriesName.Should().Be("Game of Thrones");
            result.Actors.Count.Should().Be(1);
            result.Episodes.Count.Should().Be(1);
            result.SeriesBanners.Count.Should().Be(1);
            result.SeasonPosters.Count.Should().Be(1);
            result.Posters.Count.Should().Be(1);
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

            action1.Should().Throw<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void TvdbManager_GetBannerUri_Success()
        {
            ITvdbManager tvdbManager = GetTvdbManager();
            string result = null;
            Action action1 = () => result = tvdbManager.GetBannerUri("bannerPath");

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.Should().Contain("bannerPath");
            result.Should().Contain("tvdb");
        }
        #endregion GetBannerUri        
    }
}

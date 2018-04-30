using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using Sarjee.SimpleRenamer.Framework.Movie;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Movie
{
    [TestClass]
    public class TmdbManagerTests
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

        private ITmdbManager GetTmdbManager()
        {
            ITmdbManager tmdbManager = new TmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            tmdbManager.Should().NotBeNull();
            return tmdbManager;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManagerCtor_NullArguments_ThrowsArgumentNullException()
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

        #region SearchMovieByNameAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_SearchMovieByNameAsync_NullArgument_ThrowsANE()
        {
            ITmdbManager tmdbManager = GetTmdbManager();
            SearchContainer<SearchMovie> result = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByNameAsync(string.Empty, CancellationToken.None, 2016);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_SearchMovieByNameAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SearchContainer<SearchMovie>>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(new SearchContainer<SearchMovie>() { Page = 1, TotalPages = 1, TotalResults = 2 });
            ITmdbManager tmdbManager = GetTmdbManager();

            SearchContainer<SearchMovie> result = null;
            SearchContainer<SearchMovie> result2 = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByNameAsync("searchByMovieName", CancellationToken.None);
            Func<Task> action2 = async () => result2 = await tmdbManager.SearchMovieByNameAsync("searchByMovieName", CancellationToken.None, 2016);

            action1.ShouldNotThrow();
            action2.ShouldNotThrow();
            result.Should().NotBeNull();
            result.TotalResults.Should().Be(2);
            result2.Should().NotBeNull();
            result2.TotalResults.Should().Be(2);
        }
        #endregion SearchMovieByNameAsync

        #region SearchMovieByIdAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_SearchMovieByIdAsync_NullArgument_ThrowsANE()
        {
            ITmdbManager tmdbManager = GetTmdbManager();

            SearchMovie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByIdAsync(string.Empty, CancellationToken.None);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_SearchMovieByIdAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SearchMovie>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(new SearchMovie() { Title = "Fight Club" });
            ITmdbManager tmdbManager = GetTmdbManager();

            SearchMovie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByIdAsync("id", CancellationToken.None);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Title.Should().Be("Fight Club");
        }
        #endregion SearchMovieByIdAsync

        #region GetMovieAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetMovieAsync_NullArgument_ThrowsANE()
        {
            ITmdbManager tmdbManager = GetTmdbManager();

            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync(string.Empty, CancellationToken.None);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetMovieAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<SimpleRenamer.Common.Movie.Model.Movie>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(new SimpleRenamer.Common.Movie.Model.Movie() { Title = "Fight Club", Credits = new Credits() });
            ITmdbManager tmdbManager = GetTmdbManager();

            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync("getMovie", CancellationToken.None);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Title.Should().Be("Fight Club");
            result.Credits.Should().NotBeNull();
        }
        #endregion GetMovieAsync

        #region GetPosterUriAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetPosterUriAsync_NullArgument_ThrowsANE()
        {
            ITmdbManager tmdbManager = GetTmdbManager();

            string result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetPosterUriAsync(string.Empty, CancellationToken.None);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetPosterUriAsync_Null_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<TMDbConfig>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(new TMDbConfig());
            ITmdbManager tmdbManager = GetTmdbManager();

            string result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetPosterUriAsync("getPoster", CancellationToken.None);

            action1.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetPosterUriAsync_Success()
        {
            mockHelper.Setup(x => x.ExecuteRestRequestAsync<TMDbConfig>(It.IsAny<IRestClient>(), It.IsAny<IRestRequest>(), It.IsAny<JsonSerializerSettings>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(new TMDbConfig() { Images = new ConfigImageTypes() { SecureBaseUrl = "https://www.uri.com" } });
            ITmdbManager tmdbManager = GetTmdbManager();

            string result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetPosterUriAsync("getPoster", CancellationToken.None);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Should().Contain("https");
        }
        #endregion GetPosterUriAsync        
    }
}

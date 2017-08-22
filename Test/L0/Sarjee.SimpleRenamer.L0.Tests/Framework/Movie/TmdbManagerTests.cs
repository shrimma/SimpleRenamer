using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
using System;
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

        private ITmdbManager GetTmdbManager(bool testableVersion = false)
        {
            ITmdbManager tmdbManager = null;
            if (testableVersion)
            {
                tmdbManager = new TestableTmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            }
            else
            {
                tmdbManager = new TmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            }
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
        public void TmdbManager_SearchMovieByNameAsync_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = GetTmdbManager(true);
            SearchContainer<SearchMovie> result = null;
            SearchContainer<SearchMovie> result2 = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByNameAsync("searchByMovieName");
            Func<Task> action2 = async () => result2 = await tmdbManager.SearchMovieByNameAsync("searchByMovieName", 2016);

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
        public void TmdbManager_SearchMovieByIdAsync_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = GetTmdbManager(true);
            SearchMovie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByIdAsync("searchByMovieId");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Title.Should().Be("Fight Club");
        }
        #endregion SearchMovieByIdAsync

        #region GetMovieAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetMovieAsync_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = GetTmdbManager(true);
            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync("getMovie");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Title.Should().Be("Fight Club");
            result.Credits.Should().NotBeNull();
        }
        #endregion GetMovieAsync

        #region GetPosterUriAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetPosterUriAsync_Null_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = new EmptyTestableTmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            string result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetPosterUriAsync("getPoster");

            action1.ShouldNotThrow();
            result.Should().BeNullOrEmpty();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_GetPosterUriAsync_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = GetTmdbManager(true);
            string result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetPosterUriAsync("getPoster");

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.Should().Contain("tmdb");
        }
        #endregion GetPosterUriAsync

        #region RetryHandling
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_RetryHandling_StatusCode_ReturnsNull_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = new ErrorCodeTestableTmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync("getMovie");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_RetryHandling_WebException_ReturnsNull_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = new WebExceptionTestableTmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync("getMovie");

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void TmdbManager_RetryHandling_ErrorException_ThrowsException_Success()
        {
            //get testable tmdbmanager
            ITmdbManager tmdbManager = new ErrorExceptionTestableTmdbManager(mockConfigurationManager.Object, mockHelper.Object);
            SimpleRenamer.Common.Movie.Model.Movie result = null;
            Func<Task> action1 = async () => result = await tmdbManager.GetMovieAsync("getMovie");

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }
        #endregion RetryHandling
    }
}

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
            Func<Task> action1 = async () => result = await tmdbManager.SearchMovieByNameAsync("searchByMovieName", 2015);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
            result.TotalResults.Should().Be(2);
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
            result.Credits.Should().NotBeNull();
        }
        #endregion GetMovieAsync

        #region GetPosterUriAsync
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
        }
        #endregion GetPosterUriAsync
    }
}

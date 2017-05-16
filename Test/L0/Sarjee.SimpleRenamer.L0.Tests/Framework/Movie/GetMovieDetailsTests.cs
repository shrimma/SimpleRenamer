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
    public class GetMovieDetailsTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<ITmdbManager> mockTmdbManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockTmdbManager = mockRepository.Create<ITmdbManager>();
        }

        private IGetMovieDetails GetGetMovieDetails()
        {
            IGetMovieDetails getMovieDetails = new GetMovieDetails(mockLogger.Object, mockTmdbManager.Object);
            getMovieDetails.Should().NotBeNull();
            return getMovieDetails;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void GetMovieDetailsCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new GetMovieDetails(null, null);
            Action action2 = () => new GetMovieDetails(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void GetMovieDetailsCtor_Success()
        {
            IGetMovieDetails getMovieDetails = null;
            Action action1 = () => getMovieDetails = GetGetMovieDetails();

            action1.ShouldNotThrow();
            getMovieDetails.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

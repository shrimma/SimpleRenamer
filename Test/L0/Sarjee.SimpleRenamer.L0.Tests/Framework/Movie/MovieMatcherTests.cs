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
    public class MovieMatcherTests
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

        private IMovieMatcher GetMovieMatcher()
        {
            IMovieMatcher movieMatcher = new MovieMatcher(mockLogger.Object, mockTmdbManager.Object);
            movieMatcher.Should().NotBeNull();
            return movieMatcher;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new MovieMatcher(null, null);
            Action action2 = () => new MovieMatcher(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcherCtor_Success()
        {
            IMovieMatcher movieMatcher = null;
            Action action1 = () => movieMatcher = GetMovieMatcher();

            action1.ShouldNotThrow();
            movieMatcher.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

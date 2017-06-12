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
        private Mock<IHelper> mockHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockTmdbManager = mockRepository.Create<ITmdbManager>();
            mockHelper = mockRepository.Create<IHelper>();
        }

        private IMovieMatcher GetMovieMatcher()
        {
            IMovieMatcher movieMatcher = new MovieMatcher(mockLogger.Object, mockTmdbManager.Object, mockHelper.Object);
            movieMatcher.Should().NotBeNull();
            return movieMatcher;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new MovieMatcher(null, null, null);
            Action action2 = () => new MovieMatcher(mockLogger.Object, null, null);
            Action action3 = () => new MovieMatcher(mockLogger.Object, mockTmdbManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
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

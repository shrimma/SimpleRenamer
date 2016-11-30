using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.L0;
using System;


namespace SimpleRenamer.Framework.Movie.L0
{
    [TestClass]
    public class MovieMatcherTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MovieMatcherCtor_NullLogger_ThrowsArgumentNullException()
        {
            IMovieMatcher movieMatcher = new MovieMatcher(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MovieMatcherCtor_NullTmdbManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IMovieMatcher movieMatcher = new MovieMatcher(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcheCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            ITmdbManager tmdbManager = new Mock<ITmdbManager>().Object;
            IMovieMatcher movieMatcher = new MovieMatcher(logger, tmdbManager);

            Assert.IsNotNull(movieMatcher);
        }
    }
}

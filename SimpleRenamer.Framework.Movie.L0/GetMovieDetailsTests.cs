using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.L0;
using System;

namespace SimpleRenamer.Framework.Movie.L0
{
    [TestClass]
    public class GetMovieDetailsTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMovieDetailsCtor_NullLogger_ThrowsArgumentNullException()
        {
            IGetMovieDetails getMovieDetails = new GetMovieDetails(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMovieDetailsCtor_NullTmdbManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IGetMovieDetails getMovieDetails = new GetMovieDetails(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void GetMovieDetailsCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            ITmdbManager tmdbManager = new Mock<ITmdbManager>().Object;
            IGetMovieDetails getMovieDetails = new GetMovieDetails(logger, tmdbManager);

            Assert.IsNotNull(getMovieDetails);
        }
    }
}

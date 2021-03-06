﻿using FluentAssertions;
using LazyCache;
using LazyCache.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using Sarjee.SimpleRenamer.Framework.Movie;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Movie
{
    [TestClass]
    public class MovieMatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<ITmdbManager> mockTmdbManager;
        private Mock<IHelper> mockHelper;
        private IAppCache mockCache;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockTmdbManager = mockRepository.Create<ITmdbManager>();
            mockHelper = mockRepository.Create<IHelper>();
            mockCache = new MockCachingService
            {
                DefaultCachePolicy = new CacheDefaults()
            };
        }

        private IMovieMatcher GetMovieMatcher(bool testable = false)
        {
            IMovieMatcher movieMatcher = null;
            if (testable)
            {
                movieMatcher = new TestableMovieMatcher(mockLogger.Object, mockTmdbManager.Object, mockHelper.Object, mockCache);
            }
            else
            {
                movieMatcher = new MovieMatcher(mockLogger.Object, mockTmdbManager.Object, mockHelper.Object, mockCache);
            }
            movieMatcher.Should().NotBeNull();
            movieMatcher.RaiseProgressEvent += MovieMatcher_RaiseProgressEvent;
            return movieMatcher;
        }

        private void MovieMatcher_RaiseProgressEvent(object sender, SimpleRenamer.Common.EventArguments.ProgressTextEventArgs e)
        {
            //do nothing but need this
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new MovieMatcher(null, null, null, null);
            Action action2 = () => new MovieMatcher(mockLogger.Object, null, null, null);
            Action action3 = () => new MovieMatcher(mockLogger.Object, mockTmdbManager.Object, null, null);
            Action action4 = () => new MovieMatcher(mockLogger.Object, mockTmdbManager.Object, mockHelper.Object, null);

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
            action4.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcherCtor_Success()
        {
            IMovieMatcher movieMatcher = null;
            Action action1 = () => movieMatcher = GetMovieMatcher();

            action1.Should().NotThrow();
            movieMatcher.Should().NotBeNull();
        }
        #endregion Constructor

        #region GetPossibleMoviesForFile
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_GetPossibleMoviesForFile_Success()
        {
            //dummy movies
            SearchContainer<SearchMovie> output = new SearchContainer<SearchMovie>();
            output.Results.Add(new SearchMovie() { Id = 1, Title = "dummyMovie1", Overview = "short and sweet" });
            output.Results.Add(new SearchMovie() { Id = 2, Title = "dummyMovie2", Overview = "so very very very very very very very very very very very very very long and boring and dull and why am I still writing this" });
            output.Results.Add(new SearchMovie() { Id = 3 });

            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(output);
            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(output);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            List<DetailView> result = null;
            Func<Task> action1 = async () => result = await movieMatcher.GetPossibleMoviesForFileAsync("showName", CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }
        #endregion GetPossibleMoviesForFile

        #region ScrapeDetailsAsync
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_ScrapeDetailsAsync_NullResponse_Success()
        {
            //null response                        
            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync((SearchContainer<SearchMovie>)null);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.ScrapeDetailsAsync(input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeFalse();
            result.SkippedExactSelection.Should().BeTrue();
        }
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_ScrapeDetailsAsync_NoMatch_Success()
        {
            //dummy movies
            SearchContainer<SearchMovie> output = new SearchContainer<SearchMovie>();

            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(output);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.ScrapeDetailsAsync(input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeFalse();
            result.SkippedExactSelection.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_ScrapeDetailsAsync_SingleMatch_Success()
        {
            //dummy movies
            SearchContainer<SearchMovie> output = new SearchContainer<SearchMovie>();
            output.Results.Add(new SearchMovie() { Id = 1, Title = "dummyMovie1", Overview = "short and sweet" });

            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(output);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.ScrapeDetailsAsync(input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.TMDBShowId.Should().Be(1);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_ScrapeDetailsAsync_MultiMatch_Success()
        {
            //dummy movies
            SearchContainer<SearchMovie> output = new SearchContainer<SearchMovie>();
            output.Results.Add(new SearchMovie() { Id = 1, Title = "dummyMovie1", Overview = "short and sweet" });
            output.Results.Add(new SearchMovie() { Id = 2, Title = "dummyMovie2", Overview = "so very very very very very very very very very very very very very long and boring and dull and why am I still writing this" });
            output.Results.Add(new SearchMovie() { Id = 3 });

            mockTmdbManager.Setup(x => x.SearchMovieByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(output);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.ScrapeDetailsAsync(input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeFalse();
            result.SkippedExactSelection.Should().BeTrue();
        }
        #endregion ScrapeDetailsAsync

        #region UpdateFileWithMatchedMovie
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_UpdateFileWithMatchedMovie_NullArgument_ThrowsException()
        {
            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            Func<Task> action1 = async () => result = await movieMatcher.UpdateFileWithMatchedMovieAsync("movieId1", null, CancellationToken.None);

            action1.Should().Throw<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_UpdateFileWithMatchedMovie_NoMovieId_Success()
        {
            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.UpdateFileWithMatchedMovieAsync(string.Empty, input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.ActionThis.Should().BeFalse();
            result.SkippedExactSelection.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_UpdateFileWithMatchedMovie_Success()
        {
            //dummy helper
            mockHelper.Setup(x => x.RemoveSpecialCharacters(It.IsAny<string>())).Returns<string>(x => x);
            //dummy movie                        
            SearchMovie output = new SearchMovie() { Id = 1, Title = "dummyMovie1", Overview = "short and sweet" };
            mockTmdbManager.Setup(x => x.SearchMovieByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(output);

            IMovieMatcher movieMatcher = GetMovieMatcher();
            MatchedFile result = null;
            MatchedFile input = new MatchedFile(@"c:\movie", "movieTitle", 2015);
            Func<Task> action1 = async () => result = await movieMatcher.UpdateFileWithMatchedMovieAsync("movieId1", input, CancellationToken.None);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.TMDBShowId.Should().Be(1);
            result.ActionThis.Should().BeTrue();
            result.SkippedExactSelection.Should().BeFalse();
        }
        #endregion UpdateFileWithMatchedMovie

        #region GetMovieWithBanner
        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_GetMovieWithBanner_WithBanner_Success()
        {
            string movieName = "DummyMovie";
            mockTmdbManager.Setup(x => x.GetMovieAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SimpleRenamer.Common.Movie.Model.Movie() { Title = movieName, PosterPath = "notnull" });
            mockTmdbManager.Setup(x => x.GetPosterUriAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("https://i.ytimg.com/vi/yaqe1qesQ8c/maxresdefault.jpg");

            IMovieMatcher movieMatcher = GetMovieMatcher(true);
            (SimpleRenamer.Common.Movie.Model.Movie movie, Uri image) result = (null, null);
            Func<Task> action1 = async () => result = await movieMatcher.GetMovieWithBannerAsync("1", new CancellationToken());

            action1.Should().NotThrow();
            result.movie.Should().NotBeNull();
            result.image.Should().NotBeNull();
            result.movie.Title.Should().Be(movieName);
        }

        [TestMethod]
        [TestCategory(TestCategories.Movie)]
        public void MovieMatcher_GetMovieWithBanner_NoBanner_Success()
        {
            string movieName = "DummyMovie";
            mockTmdbManager.Setup(x => x.GetMovieAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SimpleRenamer.Common.Movie.Model.Movie() { Title = movieName });

            IMovieMatcher movieMatcher = GetMovieMatcher(true);
            (SimpleRenamer.Common.Movie.Model.Movie movie, Uri image) result = (null, null);
            Func<Task> action1 = async () => result = await movieMatcher.GetMovieWithBannerAsync("1", CancellationToken.None);

            action1.Should().NotThrow();
            result.movie.Should().NotBeNull();
            result.image.Should().BeNull();
            result.movie.Title.Should().Be(movieName);
        }
        #endregion GetMovieWithBanner
    }
}

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class ScanFilesTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IFileWatcher> mockFileWatcher;
        private Mock<ITVShowMatcher> mockShowMatcher;
        private Mock<IMovieMatcher> mockMovieMatcher;
        private Mock<IFileMatcher> mockFileMatcher;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockFileWatcher = mockRepository.Create<IFileWatcher>();
            mockShowMatcher = mockRepository.Create<ITVShowMatcher>();
            mockMovieMatcher = mockRepository.Create<IMovieMatcher>();
            mockFileMatcher = mockRepository.Create<IFileMatcher>();
        }

        private IScanFiles GetScanFiles()
        {
            IScanFiles scanFiles = new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, mockMovieMatcher.Object, mockFileMatcher.Object);
            scanFiles.Should().NotBeNull();
            scanFiles.RaiseProgressEvent += ScanFiles_RaiseProgressEvent;
            return scanFiles;
        }

        private void ScanFiles_RaiseProgressEvent(object sender, ProgressTextEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new ScanFiles(null, null, null, null, null, null);
            Action action2 = () => new ScanFiles(mockLogger.Object, null, null, null, null, null);
            Action action3 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, null, null, null, null);
            Action action4 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, null, null, null);
            Action action5 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, null, null);
            Action action6 = () => new ScanFiles(mockLogger.Object, mockConfigurationManager.Object, mockFileWatcher.Object, mockShowMatcher.Object, mockMovieMatcher.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
            action4.ShouldThrow<ArgumentNullException>();
            action5.ShouldThrow<ArgumentNullException>();
            action6.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_Success()
        {
            IScanFiles scanFiles = null;
            Action action1 = () => scanFiles = GetScanFiles();

            action1.ShouldNotThrow();
            scanFiles.Should().NotBeNull();
        }
        #endregion Constructor

        #region Scan
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFiles_Scan_NoFiles_Success()
        {
            //setup the mocks
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileWatcher.Setup(x => x.SearchFoldersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());
            mockFileMatcher.Setup(x => x.SearchFilesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MatchedFile>());
            IScanFiles scanFiles = GetScanFiles();

            List<MatchedFile> scannedFiles = null;
            Func<Task> action1 = async () => scannedFiles = await scanFiles.ScanAsync(new CancellationToken());

            action1.ShouldNotThrow();
            scannedFiles.Should().NotBeNull();
            scannedFiles.Count.Should().Be(0);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFiles_Scan_Movies_Success()
        {
            //setup the mocks
            Settings settings = new Settings { DestinationFolderMovie = @"C:\Movies" };
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileWatcher.Setup(x => x.SearchFoldersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());
            //file that needs moving
            MatchedFile spectre = new MatchedFile(@"C:\Spectre.mkv", "Spectre", 2015) { NewFileName = "Spectre" };
            //file that doesnt need moving
            MatchedFile pomPoko = new MatchedFile(@"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", "Pom Poko", 1994) { NewFileName = "Pom Poko" };
            mockFileMatcher.Setup(x => x.SearchFilesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MatchedFile> { spectre, pomPoko });
            mockMovieMatcher.Setup(x => x.ScrapeDetailsAsync(It.Is<MatchedFile>(i => i == spectre), It.IsAny<CancellationToken>())).ReturnsAsync(spectre);
            mockMovieMatcher.Setup(x => x.ScrapeDetailsAsync(It.Is<MatchedFile>(i => i == pomPoko), It.IsAny<CancellationToken>())).ReturnsAsync(pomPoko);

            IScanFiles scanFiles = GetScanFiles();

            List<MatchedFile> scannedFiles = null;
            Func<Task> action1 = async () => scannedFiles = await scanFiles.ScanAsync(CancellationToken.None);

            action1.ShouldNotThrow();
            scannedFiles.Should().NotBeNullOrEmpty();
            scannedFiles.Count.Should().Be(1);

            mockMovieMatcher.Verify(x => x.ScrapeDetailsAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFiles_Scan_TvShows_Success()
        {
            //setup the mocks
            Settings settings = new Settings { DestinationFolderTV = @"C:\TV" };
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileWatcher.Setup(x => x.SearchFoldersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());
            //file that needs moving
            MatchedFile castle1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle", "1", "1") { NewFileName = "Castle.S01E01", TVDBShowId = "1" };
            //file that doesnt need moving
            MatchedFile castle2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle", "1", "2") { NewFileName = "Castle.S01E02" };
            mockFileMatcher.Setup(x => x.SearchFilesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MatchedFile> { castle1, castle2 });
            mockShowMatcher.Setup(x => x.FixShowsFromMappings(It.Is<MatchedFile>(i => i == castle1))).Returns(castle1);
            mockShowMatcher.Setup(x => x.FixShowsFromMappings(It.Is<MatchedFile>(i => i == castle2))).Returns(castle2);
            mockShowMatcher.Setup(x => x.SearchShowByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CompleteSeries(new Series(1, "Castle"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>()));
            mockShowMatcher.Setup(x => x.SearchShowByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CompleteSeries(new Series(1, "Castle"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>()));
            mockShowMatcher.Setup(x => x.UpdateFileWithSeriesDetails(It.Is<MatchedFile>(i => i == castle1), It.IsAny<CompleteSeries>())).Returns(castle1);
            mockShowMatcher.Setup(x => x.UpdateFileWithSeriesDetails(It.Is<MatchedFile>(i => i == castle2), It.IsAny<CompleteSeries>())).Returns(castle2);

            IScanFiles scanFiles = GetScanFiles();

            List<MatchedFile> scannedFiles = null;
            Func<Task> action1 = async () => scannedFiles = await scanFiles.ScanAsync(CancellationToken.None);

            action1.ShouldNotThrow();
            scannedFiles.Should().NotBeNullOrEmpty();
            scannedFiles.Count.Should().Be(1);

            mockShowMatcher.Verify(x => x.FixShowsFromMappings(It.IsAny<MatchedFile>()), Times.Exactly(2));
            mockShowMatcher.Verify(x => x.SearchShowByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            mockShowMatcher.Verify(x => x.SearchShowByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            mockShowMatcher.Verify(x => x.UpdateFileWithSeriesDetails(It.IsAny<MatchedFile>(), It.IsAny<CompleteSeries>()), Times.Exactly(2));
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFiles_Scan_Unknown_Success()
        {
            //setup the mocks
            Settings settings = new Settings { DestinationFolderTV = @"C:\TV" };
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileWatcher.Setup(x => x.SearchFoldersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());
            //file that needs moving
            MatchedFile unknown1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle");
            //file that doesnt need moving
            MatchedFile unknown2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle");
            mockFileMatcher.Setup(x => x.SearchFilesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MatchedFile> { unknown1, unknown2 });

            IScanFiles scanFiles = GetScanFiles();

            List<MatchedFile> scannedFiles = null;
            Func<Task> action1 = async () => scannedFiles = await scanFiles.ScanAsync(CancellationToken.None);

            action1.ShouldNotThrow();
            scannedFiles.Should().NotBeNullOrEmpty();
            scannedFiles.Count.Should().Be(2);

            mockShowMatcher.Verify(x => x.FixShowsFromMappings(It.IsAny<MatchedFile>()), Times.Never);
            mockShowMatcher.Verify(x => x.SearchShowByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            mockShowMatcher.Verify(x => x.SearchShowByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            mockShowMatcher.Verify(x => x.UpdateFileWithSeriesDetails(It.IsAny<MatchedFile>(), It.IsAny<CompleteSeries>()), Times.Never);
            mockMovieMatcher.Verify(x => x.ScrapeDetailsAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFiles_Scan_Combination_Success()
        {
            //setup the mocks
            Settings settings = new Settings { DestinationFolderTV = @"C:\TV", DestinationFolderMovie = @"C:\Movies" };
            mockConfigurationManager.Setup(x => x.Settings).Returns(settings);
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileWatcher.Setup(x => x.SearchFoldersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>()).Raises(x => x.RaiseProgressEvent += null, new ProgressTextEventArgs("progressing"));
            //file that needs moving
            MatchedFile unknown1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle");
            //file that needs moving
            MatchedFile unknown2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle");
            //file that needs moving
            MatchedFile castle1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle", "1", "1") { NewFileName = "Castle.S01E01", TVDBShowId = "1" };
            //file that doesnt need moving
            MatchedFile castle2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle", "1", "2") { NewFileName = "Castle.S01E02" };
            //file that needs moving
            MatchedFile spectre = new MatchedFile(@"C:\Spectre.mkv", "Spectre", 2015) { NewFileName = "Spectre" };
            //file that doesnt need moving
            MatchedFile pomPoko = new MatchedFile(@"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", "Pom Poko", 1994) { NewFileName = "Pom Poko" };
            mockFileMatcher.Setup(x => x.SearchFilesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MatchedFile> { unknown1, unknown2, castle1, castle2, spectre, pomPoko });
            //mock show matcher
            mockShowMatcher.Setup(x => x.FixShowsFromMappings(It.Is<MatchedFile>(i => i == castle1))).Returns(castle1);
            mockShowMatcher.Setup(x => x.FixShowsFromMappings(It.Is<MatchedFile>(i => i == castle2))).Returns(castle2);
            mockShowMatcher.Setup(x => x.SearchShowByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CompleteSeries(new Series(1, "Castle"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>()));
            mockShowMatcher.Setup(x => x.SearchShowByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CompleteSeries(new Series(1, "Castle"), new List<SeriesActorsData>(), new List<BasicEpisode>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>(), new List<SeriesImageQueryResult>()));
            mockShowMatcher.Setup(x => x.UpdateFileWithSeriesDetails(It.Is<MatchedFile>(i => i == castle1), It.IsAny<CompleteSeries>())).Returns(castle1);
            mockShowMatcher.Setup(x => x.UpdateFileWithSeriesDetails(It.Is<MatchedFile>(i => i == castle2), It.IsAny<CompleteSeries>())).Returns(castle2);
            //mock movie matcher
            mockMovieMatcher.Setup(x => x.ScrapeDetailsAsync(It.Is<MatchedFile>(i => i == spectre), It.IsAny<CancellationToken>())).ReturnsAsync(spectre);
            mockMovieMatcher.Setup(x => x.ScrapeDetailsAsync(It.Is<MatchedFile>(i => i == pomPoko), It.IsAny<CancellationToken>())).ReturnsAsync(pomPoko);

            IScanFiles scanFiles = GetScanFiles();

            List<MatchedFile> scannedFiles = null;
            Func<Task> action1 = async () => scannedFiles = await scanFiles.ScanAsync(CancellationToken.None);

            action1.ShouldNotThrow();
            scannedFiles.Should().NotBeNullOrEmpty();
            scannedFiles.Count.Should().Be(4);

            mockShowMatcher.Verify(x => x.FixShowsFromMappings(It.IsAny<MatchedFile>()), Times.Exactly(2));
            mockShowMatcher.Verify(x => x.SearchShowByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            mockShowMatcher.Verify(x => x.SearchShowByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            mockShowMatcher.Verify(x => x.UpdateFileWithSeriesDetails(It.IsAny<MatchedFile>(), It.IsAny<CompleteSeries>()), Times.Exactly(2));
            mockMovieMatcher.Verify(x => x.ScrapeDetailsAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
        #endregion Scan
    }
}

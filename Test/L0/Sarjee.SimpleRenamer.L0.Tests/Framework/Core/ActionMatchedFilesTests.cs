using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class ActionMatchedFilesTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IBackgroundQueue> mockBackgroundQueue;
        private Mock<IFileMover> mockFileMover;
        private Mock<IConfigurationManager> mockConfigurationManager;
        private Mock<IMessageSender> mockMessageSender;


        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockBackgroundQueue = mockRepository.Create<IBackgroundQueue>();
            mockFileMover = mockRepository.Create<IFileMover>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockMessageSender = mockRepository.Create<IMessageSender>();
        }

        private IActionMatchedFiles GetActionMatchedFiles()
        {
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, mockFileMover.Object, mockConfigurationManager.Object, mockMessageSender.Object);
            actionMatchedFiles.Should().NotBeNull();
            actionMatchedFiles.RaiseFilePreProcessedEvent += ActionMatchedFiles_RaiseFilePreProcessedEvent;
            actionMatchedFiles.RaiseFileMovedEvent += ActionMatchedFiles_RaiseFileMovedEvent;
            actionMatchedFiles.RaiseProgressEvent += ActionMatchedFiles_RaiseProgressEvent;
            return actionMatchedFiles;
        }

        private IActionMatchedFiles GetActionMatchedFilesWithBackgroundQueue()
        {
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(mockLogger.Object, new BackgroundQueue(), mockFileMover.Object, mockConfigurationManager.Object, mockMessageSender.Object);
            actionMatchedFiles.Should().NotBeNull();
            actionMatchedFiles.RaiseFilePreProcessedEvent += ActionMatchedFiles_RaiseFilePreProcessedEvent;
            actionMatchedFiles.RaiseFileMovedEvent += ActionMatchedFiles_RaiseFileMovedEvent;
            actionMatchedFiles.RaiseProgressEvent += ActionMatchedFiles_RaiseProgressEvent;
            return actionMatchedFiles;
        }

        private void ActionMatchedFiles_RaiseFilePreProcessedEvent(object sender, FilePreProcessedEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        private void ActionMatchedFiles_RaiseFileMovedEvent(object sender, FileMovedEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        private void ActionMatchedFiles_RaiseProgressEvent(object sender, ProgressTextEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_NullArgument_ThrowsArgumentNullException()
        {
            Action action1 = () => new ActionMatchedFiles(null, null, null, null, null);
            Action action2 = () => new ActionMatchedFiles(mockLogger.Object, null, null, null, null);
            Action action3 = () => new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, null, null, null);
            Action action4 = () => new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, mockFileMover.Object, null, null);
            Action action5 = () => new ActionMatchedFiles(mockLogger.Object, mockBackgroundQueue.Object, mockFileMover.Object, mockConfigurationManager.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
            action3.ShouldThrow<ArgumentNullException>();
            action4.ShouldThrow<ArgumentNullException>();
            action5.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_Success()
        {
            IActionMatchedFiles actionMatchedFiles = null;
            Action action1 = () => actionMatchedFiles = GetActionMatchedFiles();

            action1.ShouldNotThrow();
            actionMatchedFiles.Should().NotBeNull();
        }
        #endregion Constructor

        #region Action
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFiles_Action_NoFiles_Success()
        {
            IActionMatchedFiles actionMatchedFiles = GetActionMatchedFiles();

            bool result = false;
            Func<Task> action1 = async () => result = await actionMatchedFiles.ActionAsync(new ObservableCollection<MatchedFile>(), new CancellationToken());

            action1.ShouldNotThrow();
            result.Should().BeTrue();
            mockMessageSender.Verify(x => x.SendAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFiles_Action_Movies_Success()
        {
            MatchedFile spectre = new MatchedFile(@"C:\Spectre.mkv", "Spectre", 2015) { NewFileName = "Spectre", DestinationFilePath = @"C:\Movies\Spectre (2015)\Spectre.mkv", ActionThis = true };
            MatchedFile pomPoko = new MatchedFile(@"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", "Pom Poko", 1994) { NewFileName = "Pom Poko", DestinationFilePath = @"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", ActionThis = true };

            //setup mocks            
            mockConfigurationManager.Setup(x => x.Settings).Returns(new Settings());
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == spectre), null, It.IsAny<bool>())).ReturnsAsync(spectre);
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == pomPoko), null, It.IsAny<bool>())).ReturnsAsync(pomPoko);
            mockFileMover.Setup(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            IActionMatchedFiles actionMatchedFiles = GetActionMatchedFilesWithBackgroundQueue();

            bool result = false;
            Func<Task> action1 = async () => result = await actionMatchedFiles.ActionAsync(new ObservableCollection<MatchedFile> { spectre, pomPoko }, new CancellationToken());

            action1.ShouldNotThrow();
            result.Should().BeTrue();
            mockFileMover.Verify(x => x.CreateDirectoriesAndDownloadBannersAsync(It.IsAny<MatchedFile>(), null, It.IsAny<bool>()), Times.Exactly(2));
            mockFileMover.Verify(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mockMessageSender.Verify(x => x.SendAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFiles_Action_TvShows_Success()
        {
            MatchedFile castle1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle", "1", "1") { NewFileName = "Castle.S01E01", TVDBShowId = "1", DestinationFilePath = @"C:\TV\Castle (2012)\Castle.S01E01.mkv", ActionThis = true };
            MatchedFile castle2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle", "1", "2") { NewFileName = "Castle.S01E02", DestinationFilePath = @"C:\TV\Castle (2012)\Castle.S01E02.mkv", ActionThis = true };

            //setup mocks
            mockConfigurationManager.Setup(x => x.Settings).Returns(new Settings());
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == castle1), null, It.IsAny<bool>())).ReturnsAsync(castle1);
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == castle2), null, It.IsAny<bool>())).ReturnsAsync(castle2);
            mockFileMover.Setup(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            IActionMatchedFiles actionMatchedFiles = GetActionMatchedFilesWithBackgroundQueue();

            bool result = false;
            Func<Task> action1 = async () => result = await actionMatchedFiles.ActionAsync(new ObservableCollection<MatchedFile> { castle1, castle2 }, new CancellationToken());

            action1.ShouldNotThrow();
            result.Should().BeTrue();
            mockFileMover.Verify(x => x.CreateDirectoriesAndDownloadBannersAsync(It.IsAny<MatchedFile>(), null, It.IsAny<bool>()), Times.Exactly(2));
            mockFileMover.Verify(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mockMessageSender.Verify(x => x.SendAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFiles_Action_Combination_Success()
        {
            MatchedFile castle1 = new MatchedFile(@"C:\Castle.S01E01.mkv", "Castle", "1", "1") { NewFileName = "Castle.S01E01", TVDBShowId = "1", DestinationFilePath = @"C:\TV\Castle (2012)\Castle.S01E01.mkv", ActionThis = true };
            MatchedFile castle2 = new MatchedFile(@"C:\TV\Castle\Season 1\Castle.S01E02.mkv", "Castle", "1", "2") { NewFileName = "Castle.S01E02", DestinationFilePath = @"C:\TV\Castle (2012)\Castle.S01E02.mkv", ActionThis = true };
            MatchedFile spectre = new MatchedFile(@"C:\Spectre.mkv", "Spectre", 2015) { NewFileName = "Spectre", DestinationFilePath = @"C:\Movies\Spectre (2015)\Spectre.mkv", ActionThis = true };
            MatchedFile pomPoko = new MatchedFile(@"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", "Pom Poko", 1994) { NewFileName = "Pom Poko", DestinationFilePath = @"C:\Movies\Pom Poko (1994)\Pom Poko.mkv", ActionThis = true };

            //setup mocks
            mockConfigurationManager.Setup(x => x.Settings).Returns(new Settings());
            mockConfigurationManager.SetupGet(x => x.ShowNameMappings).Returns(new ShowNameMapping());
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == castle1), null, It.IsAny<bool>())).ReturnsAsync(castle1);
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == castle2), null, It.IsAny<bool>())).ReturnsAsync(castle2);
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == spectre), null, It.IsAny<bool>())).ReturnsAsync(spectre);
            mockFileMover.Setup(x => x.CreateDirectoriesAndDownloadBannersAsync(It.Is<MatchedFile>(i => i == pomPoko), null, It.IsAny<bool>())).ReturnsAsync(pomPoko);
            mockFileMover.Setup(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            IActionMatchedFiles actionMatchedFiles = GetActionMatchedFilesWithBackgroundQueue();

            bool result = false;
            Func<Task> action1 = async () => result = await actionMatchedFiles.ActionAsync(new ObservableCollection<MatchedFile> { castle1, castle2, spectre, pomPoko }, new CancellationToken());

            action1.ShouldNotThrow();
            result.Should().BeTrue();
            mockFileMover.Verify(x => x.CreateDirectoriesAndDownloadBannersAsync(It.IsAny<MatchedFile>(), null, It.IsAny<bool>()), Times.Exactly(4));
            mockFileMover.Verify(x => x.MoveFileAsync(It.IsAny<MatchedFile>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
            mockMessageSender.Verify(x => x.SendAsync(It.IsAny<string>()), Times.Once);
        }
        #endregion Action
    }
}

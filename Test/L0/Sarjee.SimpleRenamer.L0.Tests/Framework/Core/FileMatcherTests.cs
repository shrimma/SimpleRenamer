using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class FileMatcherTests
    {
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<IConfigurationManager> mockConfigurationManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
            mockConfigurationManager.Setup(x => x.RegexExpressions).Returns(new List<RegexExpression>
                    {
                        new RegexExpression("^((?<series_name>.+?)[. _-]+)?s(?<season_num>\\d+)[. _-]*e(?<ep_num>\\d+)(([. _-]*e|-)(?<extra_ep_num>(?!(1080|720)[pi])\\d+))*[. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", true, true),
                        new RegexExpression("^((?<series_name>.+?)[\\[. _-]+)?(?<season_num>\\d+)x(?<ep_num>\\d+)(([. _-]*x|-)(?<extra_ep_num>(?!(1080|720)[pi])(?!(?<=x)264)\\d+))*[\\]. _-]*((?<extra_info>.+?)((?<![. _-])-(?<release_group>[^-]+))?)?$", true, true),
                        new RegexExpression("^((?<series_name>.*[^ (_.])[ (_.]+((?<ShowYearA>\\d{4})([ (_.]+S(?<season_num>\\d{1,2})E(?<ep_num>\\d{1,2}))?|(?<!\\d{4}[ (_.])S(?<SeasonB>\\d{1,2})E(?<EpisodeB>\\d{1,2})|(?<EpisodeC>\\d{3}))|(?<ShowNameB>.+))", true, true),
                        new RegexExpression("^((?<movie_title>.*[^ (_.])[ (_.]+(?!(1080|720)[pi])(?<movie_year>\\d{4})(.*))", true, false)
                    });
        }

        private IFileMatcher GetFileMatcher()
        {
            IFileMatcher fileMatcher = new FileMatcher(mockLogger.Object, mockConfigurationManager.Object);
            fileMatcher.Should().NotBeNull();
            fileMatcher.RaiseProgressEvent += FileMatcher_RaiseProgressEvent;
            return fileMatcher;
        }

        private void FileMatcher_RaiseProgressEvent(object sender, SimpleRenamer.Common.EventArguments.ProgressTextEventArgs e)
        {
            //DONT DO ANYTHING WE JUST NEED THIS
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new FileMatcher(null, null);
            Action action2 = () => new FileMatcher(mockLogger.Object, null);

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_Success()
        {
            IFileMatcher fileMatcher = null;
            Action action1 = () => fileMatcher = GetFileMatcher();

            action1.Should().NotThrow();
            fileMatcher.Should().NotBeNull();
        }
        #endregion Constructor

        #region SearchFilesAsync
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcher_SearchFilesAsync_NullFile_ThrowsArgumentNullException()
        {
            IFileMatcher fileMatcher = GetFileMatcher();
            List<string> input = new List<string>() { null };

            List<MatchedFile> output = null;
            Func<Task> action1 = async () => output = await fileMatcher.SearchFilesAsync(input, new System.Threading.CancellationToken());
            action1.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcher_SearchFilesAsync_Success()
        {
            IFileMatcher fileMatcher = GetFileMatcher();
            List<string> input = new List<string>() { @"C:\Castle.S01E01.mkv", @"C:\Person.Of.Interest.S01E01.mkv", @"Skyfall.mkv", @"Spectre.2016.mkv" };

            List<MatchedFile> output = null;
            Func<Task> action1 = async () => output = await fileMatcher.SearchFilesAsync(input, new System.Threading.CancellationToken());
            action1.Should().NotThrow();

            output.Should().NotBeNull();
            bool containsCastle = output.Select(x => x.ShowName).Contains("Castle");
            bool containsPOI = output.Select(x => x.ShowName).Contains("Person of Interest");
            bool containsSkyfall = output.Select(x => x.ShowName).Contains("Skyfall");
            bool containsSpectre = output.Select(x => x.ShowName).Contains("Spectre");
            containsCastle.Should().BeTrue();
            containsPOI.Should().BeTrue();
            containsSkyfall.Should().BeTrue();
            containsSpectre.Should().BeTrue();
        }
        #endregion SearchFilesAsync
    }
}

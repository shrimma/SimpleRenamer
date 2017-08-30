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
        private static RegexFile regexFile;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader file = File.OpenText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RegexExpressions.json")))
            {
                regexFile = (RegexFile)jsonSerializer.Deserialize(file, typeof(RegexFile));
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockConfigurationManager = mockRepository.Create<IConfigurationManager>();
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

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_Success()
        {
            IFileMatcher fileMatcher = null;
            Action action1 = () => fileMatcher = GetFileMatcher();

            action1.ShouldNotThrow();
            fileMatcher.Should().NotBeNull();
        }
        #endregion Constructor

        #region SearchFilesAsync
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcher_SearchFilesAsync_NullFile_ThrowsArgumentNullException()
        {
            mockConfigurationManager.Setup(x => x.RegexExpressions).Returns(regexFile);
            IFileMatcher fileMatcher = GetFileMatcher();
            List<string> input = new List<string>() { null };

            List<MatchedFile> output = null;
            Func<Task> action1 = async () => output = await fileMatcher.SearchFilesAsync(input, new System.Threading.CancellationToken());
            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcher_SearchFilesAsync_Success()
        {
            mockConfigurationManager.Setup(x => x.RegexExpressions).Returns(regexFile);
            IFileMatcher fileMatcher = GetFileMatcher();
            List<string> input = new List<string>() { @"C:\Castle.S01E01.mkv", @"C:\Person.Of.Interest.S01E01.mkv", @"Skyfall.mkv", @"Spectre.2016.mkv" };

            List<MatchedFile> output = null;
            Func<Task> action1 = async () => output = await fileMatcher.SearchFilesAsync(input, new System.Threading.CancellationToken());
            action1.ShouldNotThrow();

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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class MatchedFileTests
    {
        private MatchedFile GetMatchedFile()
        {
            MatchedFile matchedFile = new MatchedFile("filePath", "fileName");
            matchedFile.Should().NotBeNull();
            return matchedFile;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void MatchedFileCtor_Success()
        {
            MatchedFile matchedFile = null;
            Action action1 = () => matchedFile = GetMatchedFile();

            action1.Should().NotThrow();
            matchedFile.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

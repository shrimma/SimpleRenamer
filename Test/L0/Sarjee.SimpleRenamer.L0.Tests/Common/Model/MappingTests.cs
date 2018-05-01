using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class MappingTests
    {
        private Mapping GetMapping(string fileShowName = "fileShowName", string tvdbShowName = "tvdbShowName", string tvdbShowId = "tvdbShowId")
        {
            Mapping mapping = new Mapping(fileShowName, tvdbShowName, tvdbShowId);
            mapping.Should().NotBeNull();
            return mapping;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void MappingCtor_NullArguments_ThrowArgumentNullExceptions()
        {
            Action action1 = () => new Mapping(string.Empty, string.Empty, string.Empty);
            Action action2 = () => new Mapping("fileShowName", string.Empty, string.Empty);
            Action action3 = () => new Mapping("fileShowName", "tvdbShowName", string.Empty);

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void MappingCtor_Success()
        {
            Mapping mapping = null;
            Action action1 = () => mapping = GetMapping();

            action1.Should().NotThrow();
            mapping.Should().NotBeNull();
            mapping.FileShowName.Should().Be("fileShowName");
            mapping.TVDBShowName.Should().Be("tvdbShowName");
            mapping.TVDBShowID.Should().Be("tvdbShowId");
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Mapping_CustomFolderName_Success()
        {
            string folderName = "folderName";
            Mapping mapping = GetMapping();
            Action action1 = () => mapping.CustomFolderName = folderName;

            action1.Should().NotThrow();
            mapping.CustomFolderName.Should().Be(folderName);
        }
    }
}
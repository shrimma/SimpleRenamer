using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.EventArguments
{
    /// <summary>
    /// Summary description for FileMovedEventArgsTests
    /// </summary>
    [TestClass]
    public class FileMovedEventArgsTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void FileMovedEventArgsCtor_NullArgument_ThrowsException()
        {
            Action action1 = () => new FileMovedEventArgs(null);

            action1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void FileMovedEventArgsCtor_Success()
        {
            FileMovedEventArgs eventArgs = null;
            Action action1 = () => eventArgs = new FileMovedEventArgs(new MatchedFile("filepath", "fileName"));

            action1.ShouldNotThrow();
            eventArgs.Should().NotBeNull();
        }
        #endregion Constructor

        #region File
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void FileMovedEventArgs_GetFile_Success()
        {
            MatchedFile input = new MatchedFile("filePath", "fileName");
            FileMovedEventArgs eventArgs = new FileMovedEventArgs(input);

            eventArgs.Should().NotBeNull();
            eventArgs.File.Should().Be(input);
        }
        #endregion File
    }
}

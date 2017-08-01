using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.EventArguments;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.EventArguments
{
    /// <summary>
    /// Summary description for FilePreProcessedEventArgsTests
    /// </summary>
    [TestClass]
    public class FilePreProcessedEventArgsTests
    {
        #region Constructor        
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void FilePreProcessedEventArgsCtor_Success()
        {
            FilePreProcessedEventArgs eventArgs = null;
            Action action1 = () => eventArgs = new FilePreProcessedEventArgs();

            action1.ShouldNotThrow();
            eventArgs.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

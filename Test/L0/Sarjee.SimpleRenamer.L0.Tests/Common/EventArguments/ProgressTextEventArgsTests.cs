using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.EventArguments;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.EventArguments
{
    /// <summary>
    /// Summary description for FilePreProcessedEventArgs
    /// </summary>
    [TestClass]
    public class ProgressTextEventArgsTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ProgressTextEventArgsCtor_NullArgument_ThrowsException()
        {
            Action action1 = () => new ProgressTextEventArgs(null);
            Action action2 = () => new ProgressTextEventArgs(string.Empty);
            Action action3 = () => new ProgressTextEventArgs("       ");

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ProgressTextEventArgsCtor_Success()
        {
            ProgressTextEventArgs eventArgs = null;
            Action action1 = () => eventArgs = new ProgressTextEventArgs("progressText");

            action1.Should().NotThrow();
            eventArgs.Should().NotBeNull();
        }
        #endregion Constructor

        #region Text
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ProgressTextEventArgs_GetText_Success()
        {
            string input = "progressTextInput";
            ProgressTextEventArgs eventArgs = new ProgressTextEventArgs(input);
            eventArgs.Text.Should().Be(input);
        }
        #endregion Text
    }
}

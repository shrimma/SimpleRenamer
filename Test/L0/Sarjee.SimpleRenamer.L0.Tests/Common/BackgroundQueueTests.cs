using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    /// <summary>
    /// Summary description for BackgroundQueueTests
    /// </summary>
    [TestClass]
    public class BackgroundQueueTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void BackgroundQueueCtor_Success()
        {
            IBackgroundQueue backgroundQueue = null;
            Action action1 = () => backgroundQueue = new BackgroundQueue();

            action1.ShouldNotThrow();
            backgroundQueue.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

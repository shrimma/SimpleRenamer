using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    /// <summary>
    /// Summary description for BackgroundQueueTests
    /// </summary>
    [TestClass]
    public class BackgroundQueueTests
    {
        private IBackgroundQueue GetBackgroundQueue()
        {
            IBackgroundQueue backgroundQueue = new BackgroundQueue();
            backgroundQueue.Should().NotBeNull();
            return backgroundQueue;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void BackgroundQueueCtor_Success()
        {
            IBackgroundQueue backgroundQueue = null;
            Action action1 = () => backgroundQueue = GetBackgroundQueue();

            action1.ShouldNotThrow();
            backgroundQueue.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void BackgroundQueue_QueueAsyncTask_Success()
        {
            IBackgroundQueue backgroundQueue = GetBackgroundQueue();

            Func<Task> action1 = async () => await await backgroundQueue.QueueTaskAsync(() => Task.Delay(TimeSpan.FromMilliseconds(10)));

            action1.ShouldNotThrow();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void BackgroundQueue_QueueAction_Success()
        {
            IBackgroundQueue backgroundQueue = GetBackgroundQueue();

            Func<Task> action1 = async () => await backgroundQueue.QueueTaskAsync(() => Thread.Sleep(10));

            action1.ShouldNotThrow();
        }
    }
}

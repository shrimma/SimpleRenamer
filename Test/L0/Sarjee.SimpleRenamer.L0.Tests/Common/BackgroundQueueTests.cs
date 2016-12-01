﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;

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
            IBackgroundQueue backgroundQueue = new BackgroundQueue();

            Assert.IsNotNull(backgroundQueue);
        }
        #endregion Constructor
    }
}

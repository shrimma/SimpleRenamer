using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class RetryHelperTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RetryHelperCtor_Success()
        {
            IRetryHelper retryHelper = null;
            Action action1 = () => retryHelper = new RetryHelper();

            action1.ShouldNotThrow();
            retryHelper.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

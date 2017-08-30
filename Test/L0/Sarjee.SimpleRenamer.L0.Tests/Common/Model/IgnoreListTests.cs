using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class IgnoreListTests
    {
        private IgnoreList GetIgnoreList()
        {
            IgnoreList ignoreList = new IgnoreList();
            ignoreList.Should().NotBeNull();
            return ignoreList;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void IgnoreListCtor_Success()
        {
            IgnoreList ignoreList = null;
            Action action1 = () => ignoreList = GetIgnoreList();

            action1.ShouldNotThrow();
            ignoreList.Should().NotBeNull();
            ignoreList.IgnoreFiles.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

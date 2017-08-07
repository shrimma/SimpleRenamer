using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class ShowNameMappingTests
    {
        private ShowNameMapping GetShowNameMapping()
        {
            ShowNameMapping showNameMapping = new ShowNameMapping();
            showNameMapping.Should().NotBeNull();
            return showNameMapping;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ShowNameMappingCtor_Success()
        {
            ShowNameMapping showNameMapping = null;
            Action action1 = () => showNameMapping = GetShowNameMapping();

            action1.ShouldNotThrow();
            showNameMapping.Should().NotBeNull();
            showNameMapping.Mappings.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

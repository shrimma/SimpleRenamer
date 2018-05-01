using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class SettingsTests
    {
        private Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.Should().NotBeNull();
            return settings;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void SettingsCtor_Success()
        {
            Settings settings = null;
            Action action1 = () => settings = GetSettings();

            action1.Should().NotThrow();
            settings.Should().NotBeNull();
        }
        #endregion Constructor
    }
}

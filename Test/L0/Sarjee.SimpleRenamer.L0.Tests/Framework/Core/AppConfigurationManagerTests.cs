using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class AppConfigurationManagerTests
    {
        private IConfigurationManager GetConfigurationManager()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            configurationManager.Should().NotBeNull();
            return configurationManager;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManagerCtor_Success()
        {
            IConfigurationManager configurationManager = null;
            Action action1 = () => configurationManager = GetConfigurationManager();

            action1.ShouldNotThrow();
            configurationManager.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_TvdbApiKey()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            string apiKey = string.Empty;
            Action action1 = () => apiKey = configurationManager.TvDbApiKey;

            action1.ShouldNotThrow();
            apiKey.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_TmdbApiKey()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            string apiKey = string.Empty;
            Action action1 = () => apiKey = configurationManager.TmDbApiKey;

            action1.ShouldNotThrow();
            apiKey.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorUrl()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            string oneTrueErrorUrl = string.Empty;
            Action action1 = () => oneTrueErrorUrl = configurationManager.OneTrueErrorUrl;

            action1.ShouldNotThrow();
            oneTrueErrorUrl.Should().NotBeNullOrWhiteSpace();

            Uri uri = null;
            Action action2 = () => uri = new Uri(oneTrueErrorUrl);

            action2.ShouldNotThrow();
            uri.Should().NotBeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorApplicationKey()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            string oneTrueErrorApplicationKey = string.Empty;
            Action action1 = () => oneTrueErrorApplicationKey = configurationManager.OneTrueErrorApplicationKey;

            action1.ShouldNotThrow();
            oneTrueErrorApplicationKey.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorSharedSecret()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            string oneTrueErrorSharedSecret = string.Empty;
            Action action1 = () => oneTrueErrorSharedSecret = configurationManager.OneTrueErrorSharedSecret;

            action1.ShouldNotThrow();
            oneTrueErrorSharedSecret.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_Testable_Settings()
        {
            IConfigurationManager configurationManager = new TestableAppConfigurationManager();

            ISettings settings = null;
            Action action1 = () => settings = configurationManager.Settings;

            action1.ShouldNotThrow();
            settings.Should().NotBeNull();
            //TODO check the settings values!
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_RegexExpressions()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            RegexFile regexExpressions = null;
            Action action1 = () => regexExpressions = configurationManager.RegexExpressions;

            action1.ShouldNotThrow();
            regexExpressions.Should().NotBeNull();
            //TODO check the regex vlaues here
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_IgnoredFiles()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            IgnoreList ignoreList = null;
            Action action1 = () => ignoreList = configurationManager.IgnoredFiles;

            action1.ShouldNotThrow();
            ignoreList.Should().NotBeNull();
            //TODO check the ignore here
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_ShowNameMappings()
        {
            IConfigurationManager configurationManager = GetConfigurationManager();

            ShowNameMapping showNameMapping = null;
            Action action1 = () => showNameMapping = configurationManager.ShowNameMappings;

            action1.ShouldNotThrow();
            showNameMapping.Should().NotBeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_Testable_SaveConfiguration()
        {
            IConfigurationManager configurationManager = new TestableAppConfigurationManager();

            Action action1 = () => configurationManager.SaveConfiguration();

            action1.ShouldNotThrow();
        }
    }
}

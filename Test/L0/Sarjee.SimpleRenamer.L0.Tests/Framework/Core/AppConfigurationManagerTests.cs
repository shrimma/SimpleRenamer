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
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManagerCtor_Success()
        {
            IConfigurationManager configurationManager = null;
            Action action1 = () => configurationManager = new AppConfigurationManager();

            action1.ShouldNotThrow();
            configurationManager.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_TvdbApiKey()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string apiKey = configurationManager.TvDbApiKey;

            Assert.IsNotNull(apiKey);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_TmdbApiKey()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string apiKey = configurationManager.TmDbApiKey;

            Assert.IsNotNull(apiKey);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorUrl()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string oneTrueErrorUrl = configurationManager.OneTrueErrorUrl;

            Assert.IsNotNull(oneTrueErrorUrl);

            Uri uri = new Uri(oneTrueErrorUrl);

            Assert.IsNotNull(uri);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorApplicationKey()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string oneTrueErrorApplicationKey = configurationManager.OneTrueErrorApplicationKey;

            Assert.IsNotNull(oneTrueErrorApplicationKey);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_OneTrueErrorSharedSecret()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string oneTrueErrorSharedSecret = configurationManager.OneTrueErrorSharedSecret;

            Assert.IsNotNull(oneTrueErrorSharedSecret);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_Testable_Settings()
        {
            IConfigurationManager configurationManager = new TestableAppConfigurationManager();
            Settings settings = configurationManager.Settings;

            Assert.IsNotNull(settings);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_RegexExpressions()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            RegexFile regexExpressions = configurationManager.RegexExpressions;

            Assert.IsNotNull(regexExpressions);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_IgnoredFiles()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            IgnoreList ignoreList = configurationManager.IgnoredFiles;

            Assert.IsNotNull(ignoreList);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_ShowNameMappings()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            ShowNameMapping showNameMapping = configurationManager.ShowNameMappings;

            Assert.IsNotNull(showNameMapping);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_Testable_SaveConfiguration()
        {
            IConfigurationManager configurationManager = new TestableAppConfigurationManager();
            configurationManager.SaveConfiguration();

            Assert.IsTrue(true);
        }
    }
}

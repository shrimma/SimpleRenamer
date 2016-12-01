using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
{
    [TestClass]
    public class AppConfigurationManagerTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManagerCtor_Success()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            Assert.IsNotNull(configurationManager);
        }

        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void AppConfigurationManager_TvdbApiKey()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string apiKey = configurationManager.TvDbApiKey;

            Assert.IsNotNull(apiKey);
        }
    }
}

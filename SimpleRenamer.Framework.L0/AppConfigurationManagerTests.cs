using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
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

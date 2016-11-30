using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Framework.Core;

namespace SimpleRenamer.Framework.L0
{
    [TestClass]
    public class AppConfigurationManagerTests
    {
        [TestMethod]
        public void CanGetApiKey()
        {
            IConfigurationManager configurationManager = new AppConfigurationManager();
            string apiKey = configurationManager.TvDbApiKey;

            Assert.IsNotNull(apiKey);
        }
    }
}

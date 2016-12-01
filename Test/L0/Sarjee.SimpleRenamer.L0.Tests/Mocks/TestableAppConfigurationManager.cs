using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableAppConfigurationManager : AppConfigurationManager
    {
        protected override Settings GetSettings()
        {
            Settings settings = new Settings();
            return settings;
        }

        protected override void SaveConfig()
        {
            //do nothing just don't throw!
        }
    }
}

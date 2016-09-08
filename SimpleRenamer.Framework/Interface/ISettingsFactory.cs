using SimpleRenamer.Framework.DataModel;

namespace SimpleRenamer.Framework.Interface
{
    public interface ISettingsFactory
    {
        Settings GetSettings();
        void SaveSettings(Settings settings);
    }
}

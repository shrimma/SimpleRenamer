using MahApps.Metro;

namespace Sarjee.SimpleRenamer.ThemeManagerHelper
{
    public class AccentItem
    {
        public string AccentName { get; set; }
        public string AccentBaseColor { get; set; }
        public Accent Accent { get; set; }

        public AccentItem(string accentName, string accentBaseColor, Accent accent)
        {
            AccentName = accentName;
            AccentBaseColor = accentBaseColor;
            Accent = accent;
        }
    }
}

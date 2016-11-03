using System.Windows.Media;

namespace SimpleRenamer.ThemeManagerHelper
{
    public class ColorItem
    {
        public string ColorName { get; set; }
        public Color ColorValue { get; set; }

        public ColorItem(string colorName, Color value)
        {
            ColorName = colorName;
            ColorValue = value;
        }
    }
}

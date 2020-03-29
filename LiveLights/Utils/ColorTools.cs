using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

namespace LiveLights.Utils
{
    internal static class ColorTools
    {
        public static ILookup<int, Color> ARGBToKnownColorLookup { get; } = Enum.GetValues(typeof(KnownColor)).Cast<KnownColor>().Select(Color.FromKnownColor).ToLookup(c => c.ToArgb());
        public static Color ToNamedColor(this Color color)
        {
            if(color.IsNamedColor)
            {
                return color;
            }
            int key = color.ToArgb();
            return ARGBToKnownColorLookup[key].FirstOrDefault();
        }

        public static string DisplayText(this Color color)
        {
            Color knownColor = color.ToNamedColor();
            if (knownColor.ToArgb() != 0)
            {
                return knownColor.Name;
            }
            return string.Format("0x{0:X8}", color.ToArgb());
        }

        public static bool IsEmpty(this Color color) => color.ToArgb() == 0;
    }
}

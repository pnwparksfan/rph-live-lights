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
        private static Dictionary<int, string> namedColors = new Dictionary<int, string>();

        public static void AddNamedColor(string name, Color color)
        {
            namedColors[color.ToArgb()] = name;
        }

        public static ILookup<int, Color> ARGBToKnownColorLookup { get; } = Enum.GetValues(typeof(KnownColor)).Cast<KnownColor>().Select(Color.FromKnownColor).ToLookup(c => c.ToArgb());
        public static Color ToNamedColor(this Color color, bool emptyOnNoName = true)
        {
            if(color.IsNamedColor)
            {
                return color;
            }
            int key = color.ToArgb();
            Color knownColor = ARGBToKnownColorLookup[key].OrderBy(x => x.Name.Length).FirstOrDefault();
            if(knownColor.IsNamedColor && knownColor.IsValid())
            {
                return knownColor;
            }

            if(!emptyOnNoName)
            {
                return color;
            }

            return Color.Empty;
        }

        public static string DisplayText(this Color color)
        {
            if(namedColors.TryGetValue(color.ToArgb(), out string name))
            {
                return name;
            }

            Color knownColor = color.ToNamedColor();
            if (knownColor.ToArgb() != 0)
            {
                return knownColor.Name;
            }

            return color.ToFormattedHex();
        }

        public static bool GetColorByName(string input, out Color value)
        {
            if(namedColors.ContainsValue(input))
            {
                value = Color.FromArgb(namedColors.First(x => x.Value == input).Key);
                return true;
            }

            value = Color.FromName(input);
            if (value.IsValid())
            {
                return true;
            }

            try
            {
                string tempInput = input.ToLower().Replace("0x", "").Replace("#", "").Replace("x", "").Trim();
                value = ColorTranslator.FromHtml("#" + tempInput);
                if (value.IsValid())
                {
                    return true;
                }
            }
            catch (Exception) { }

            // If no matches, return empty color
            value = Color.Empty;
            return false;
        }

        public static Color HexToColor(string hex) => Color.FromArgb(Convert.ToInt32(hex, 16));

        public static string ToFormattedHex(this Color color) => string.Format("0x{0:X8}", color.ToArgb());

        public static bool IsValid(this Color color) => color.ToArgb() != 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights
{
    using Rage;
    using System.Windows.Forms;
    using System.Drawing;
    using Utils;

    internal static class Settings
    {
        internal static InitializationFile INI = new InitializationFile(@"Plugins\LiveLights\LiveLightsSettings.ini");

        // Keybindings
        public static Keys MenuModifier { get; } = INI.ReadEnum("Keybindings", "MenuModifier", Keys.None);
        public static Keys MenuKey { get; } = INI.ReadEnum("Keybindings", "MenuKey", Keys.OemMinus);

        // Updates
        public static bool CheckForUpdates { get; } = INI.ReadBoolean("Updates", "CheckForUpdates", true);

        // Export
        public static bool DefaultOverwrite { get; } = INI.ReadBoolean("Export", "DefaultAllowOverwriteOnExport", false);

        public static Color[] DefaultColors { get; }

        static Settings()
        {
            List<Color> colors = new List<Color>();

            foreach (string colorName in INI.GetKeyNames("Default Color Options"))
            {
                string hex = INI.ReadString("Default Color Options", colorName);
                try
                {
                    Color color = ColorTools.HexToColor(hex);
                    ColorTools.AddNamedColor(colorName, color);
                    colors.Add(color);
                    Game.LogTrivial($"Added color option {colorName} = {hex}");
                } catch (Exception e)
                {
                    Game.LogTrivial($"Could not parse color value {colorName} = {hex}: {e.Message}");
                }
            }

            if(colors.Count > 0)
            {
                DefaultColors = colors.ToArray();
            } else
            {
                DefaultColors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.White, Color.Orange, Color.Green, Color.Purple };
            }

            
        }
    }
}

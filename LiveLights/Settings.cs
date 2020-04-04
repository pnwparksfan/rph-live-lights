using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights
{
    using Rage;
    using System.Windows.Forms;

    internal static class Settings
    {
        internal static InitializationFile INI = new InitializationFile(@"Plugins\LiveLights\LiveLightsSettings.ini");

        public static Keys MenuModifier { get; } = INI.ReadEnum("Keybindings", "MenuModifier", Keys.None);
        public static Keys MenuKey { get; } = INI.ReadEnum("Keybindings", "MenuKey", Keys.OemMinus);

        public static bool CheckForUpdates { get; } = INI.ReadBoolean("Updates", "CheckForUpdates", true);
    }
}

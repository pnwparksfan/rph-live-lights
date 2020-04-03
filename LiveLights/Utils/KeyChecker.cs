using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    using Rage;
    using System.Windows.Forms;

    internal static class KeyChecker
    {
        public static bool AreKeysDownExclusively(Keys Modifier, Keys Key)
        {
            bool isTextEntryOpen = (Rage.Native.NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0);
            if (!isTextEntryOpen && (Game.IsKeyDownRightNow(Modifier) || Modifier == Keys.None) && Game.IsKeyDown(Key))
            {
                if (Modifier == Keys.None) return !(Game.IsAltKeyDownRightNow || Game.IsControlKeyDownRightNow || Game.IsShiftKeyDownRightNow);
                else return true;
            }
            return false;
        }
    }
}

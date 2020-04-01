using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    using Rage;
    using Rage.Native;

    internal static class UserInput
    {
        public static string GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);

            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, windowTitle, 0, defaultText, 0, 0, 0, maxLength);
            Game.DisplaySubtitle(windowTitle, 100000);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
            {
                GameFiber.Yield();
            }

            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            Game.DisplaySubtitle("", 5);

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }
    }
}

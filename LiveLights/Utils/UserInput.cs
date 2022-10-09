using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace LiveLights.Utils
{
    using Rage;
    using Rage.Native;

    internal class UserInput
    {
        public static string GetUserInput(string windowTitle, string defaultText, int maxLength, bool canCopy = true, bool canPaste = true)
        {
            string help = "";
            if (canPaste) help += $"~{Keys.LControlKey.GetInstructionalId()}~ ~{Keys.V.GetInstructionalId()}~   overwrite input with clipboard\n";
            if (canCopy) help += $"~{Keys.LControlKey.GetInstructionalId()}~ ~{Keys.C.GetInstructionalId()}~   copy initial input to clipboard\n";
            help += "~INPUT_FRONTEND_ACCEPT~   commit changes\n~INPUT_FRONTEND_CANCEL~   discard changes";

            if (Game.IsPaused)
            {
                Game.IsPaused = false;
                Game.DisplayHelp(help, true);
                GameFiber.Yield();
                Game.IsPaused = true;
            }
            else
            {
                Game.DisplayHelp(help, true);
            }

            Localization.SetText("TEXTBOX_TMP_LABEL", windowTitle);
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(0, "TEXTBOX_TMP_LABEL", 0, defaultText ?? "", 0, 0, 0, maxLength);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
            {
                if (canPaste && Game.IsControlKeyDownRightNow && Game.IsKeyDown(Keys.V))
                {
                    string text = Game.GetClipboardText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(0, "TEXTBOX_TMP_LABEL", 0, text, 0, 0, 0, maxLength);
                        Game.DisplayNotification($"Pasted \"~b~{text}~w~\" from clipboard. Note: Ctrl+V overwrites any data currently in text box.");
                    }
                }

                if (canCopy && Game.IsControlKeyDownRightNow && Game.IsKeyDown(Keys.C))
                {
                    Game.SetClipboardText(defaultText);
                    Game.DisplayNotification($"Copied \"~b~{defaultText}~w~\" to clipboard. Note: Ctrl+C can only copy the starting value, not any uncommitted changes.");
                }
                GameFiber.Yield();
            }

            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            Game.HideHelp();

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }
    }
}

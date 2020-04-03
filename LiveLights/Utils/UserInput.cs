using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Game.DisplayHelp("~b~Ctrl~w~ + ~b~V~w~: overwrite input with clipboard\n~b~Ctrl~w~ + ~b~C~w~: copy initial input to clipboard\n~INPUT_FRONTEND_ACCEPT~    commit changes\n~INPUT_FRONTEND_CANCEL~    discard changes", true);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
            {
                if(Game.IsControlKeyDownRightNow && Game.IsKeyDown(Keys.V))
                {
                    string text = GetClipboardText();
                    if(!string.IsNullOrEmpty(text))
                    {
                        NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, windowTitle, 0, text, 0, 0, 0, maxLength);
                        Game.DisplayNotification($"Pasted \"~b~{text}~w~\" from clipboard. Note: Ctrl+V overwrites any data currently in text box.");
                    }
                }

                if(Game.IsControlKeyDownRightNow && Game.IsKeyDown(Keys.C))
                {
                    SetClipboardText(defaultText);
                    Game.DisplayNotification($"Copied \"~b~{defaultText}~w~\" to clipboard. Note: Ctrl+C can only copy the starting value, not any uncommitted changes.");
                }
                GameFiber.Yield();
            }

            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            Game.DisplaySubtitle("", 5);
            Game.HideHelp();

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }

        // STA thread required to use clipboard: https://stackoverflow.com/a/518724
        public static string GetClipboardText()
        {
            string text = null;
            Thread staThread = new Thread(
                delegate()
                {
                    try
                    {
                        text = Clipboard.GetText();
                    } catch(Exception e)
                    {
                        Game.LogTrivialDebug("Could not access clipboard: " + e.Message);
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
            return text;
        }

        public static void SetClipboardText(string text)
        {
            Thread staThread = new Thread(
                delegate ()
                {
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivialDebug("Could not access clipboard: " + e.Message);
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}

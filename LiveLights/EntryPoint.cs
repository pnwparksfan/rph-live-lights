using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using Rage;
using Rage.Native;
using Rage.Attributes;


[assembly: Rage.Attributes.Plugin("Live Carcols and Siren Editor", Description = "Enable/disable emergency lights, configure siren settings, and create custom carcols entries", Author = "PNWParksFan")]

namespace LiveLights
{
    public class EntryPoint
    {
        private static void Main()
        {
            // Older versions of RPH do not support the EmergencyLighting API properly
            FileVersionInfo rphVer = FileVersionInfo.GetVersionInfo("ragepluginhook.exe");
            Game.LogTrivial("Detected RPH " + rphVer.FileVersion);
            if(rphVer.FileMinorPart < 78)
            {
                Game.LogTrivial("RPH 78+ is required to use LiveLights");
                Game.DisplayNotification($"~y~Unable to load LiveLights~w~\nRagePluginHook version ~b~78~w~ or later is required, you are on version ~b~{rphVer.FileMinorPart}");
                return;
            }

            AssemblyName pluginInfo = Assembly.GetExecutingAssembly().GetName();
            Game.LogTrivial($"Loaded {pluginInfo.Name} {pluginInfo.Version}");
            if(Settings.MenuKey != Keys.None)
            {
                Game.LogTrivial("Press " + (Settings.MenuModifier == Keys.None ? "" : (Settings.MenuModifier.ToString() + " + ")) + Settings.MenuKey.ToString() + " to open the menu");
            } else
            {
                Game.LogTrivial("Use the OpenLiveLightsMenu console command to open the menu");
            }
            
            GameFiber.ExecuteWhile(Menu.MenuController.Process, () => true);
            GameFiber.Hibernate();
        }
    }
}

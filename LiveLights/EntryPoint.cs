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


[assembly: Rage.Attributes.Plugin("Live Carcols and Siren Editor", Description = "Enable/disable emergency lights, configure siren settings, and create custom carcols entries", Author = "PNWParksFan", PrefersSingleInstance = true, SupportUrl = "https://github.com/pnwparksfan/rph-live-lights")]

namespace LiveLights
{
    using Utils;

    internal class EntryPoint
    {
        public static GithubVersionCheck VersionCheck;
        public static Version CurrentFileVersion;
        public static int MinRPHBuild = 98;

        private static void Main()
        {
            // Older versions of RPH do not support the EmergencyLighting API properly
            FileVersionInfo rphVer = FileVersionInfo.GetVersionInfo("ragepluginhook.exe");
            Game.LogTrivial("Detected RPH " + rphVer.FileVersion);
            if(rphVer.FileMinorPart < MinRPHBuild)
            {
                Game.LogTrivial($"RPH 1.{MinRPHBuild}+ is required to use this version of LiveLights");
                Game.DisplayNotification($"~y~Unable to load LiveLights~w~\nRagePluginHook version ~b~{MinRPHBuild}~w~ or later is required, you are on version ~b~{rphVer.FileMinorPart}");
                return;
            }

            AssemblyName pluginInfo = Assembly.GetExecutingAssembly().GetName();
            CurrentFileVersion = pluginInfo.Version;
            Game.LogTrivial($"Loaded {pluginInfo.Name} {pluginInfo.Version}");
            if(Settings.MenuKey != Keys.None)
            {
                Game.LogTrivial("Press " + (Settings.MenuModifier == Keys.None ? "" : (Settings.MenuModifier.ToString() + " + ")) + Settings.MenuKey.ToString() + " to open the menu");
            } else
            {
                Game.LogTrivial("Use the OpenLiveLightsMenu console command to open the menu");
            }

            if(Settings.CheckForUpdates)
            {
                VersionCheck = new GithubVersionCheck("pnwparksfan", "rph-live-lights", 29759134);
                Game.LogTrivial($"Latest release on github: {VersionCheck.LatestRelease?.TagName}");
                if (VersionCheck.IsUpdateAvailable())
                {
                    Game.LogTrivial("Current version is out of date");
                    Game.DisplayNotification("commonmenu", "mp_alerttriangle", "LiveLights by PNWParksFan", "~y~Update Available", $"~b~LiveLights {VersionCheck.LatestRelease.TagName}~w~ is available!\n\n~y~<i>{VersionCheck.LatestRelease.Name}</i>");
                }
                else
                {
                    Game.LogTrivial($"Current version is up to date");
                }
            }
            
            
            GameFiber.ExecuteWhile(Menu.MenuController.Process, () => true);
            GameFiber.Hibernate();
        }
    }
}

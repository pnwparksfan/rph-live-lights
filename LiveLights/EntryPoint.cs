using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        public static Version InstalledSSLAVersion;
        public static LCPDFRVersionCheck SSLAVersionCheck;
        public static GithubVersionCheck LiveLightsVersionCheck;
        public static Version CurrentFileVersion;
        public static int MinRPHBuild = 109;
        public static Version MinRNUIVersion = new Version(1, 9);

        private static void Main()
        {
            // Older versions of RPH do not support the EmergencyLighting API properly
            FileVersionInfo rphVer = FileVersionInfo.GetVersionInfo("ragepluginhook.exe");
            Game.LogTrivial($"Detected RPH {rphVer.FileVersion}");
            if(rphVer.FileMinorPart < MinRPHBuild)
            {
                Game.LogTrivial($"RPH 1.{MinRPHBuild}+ is required to use this version of LiveLights");
                Game.DisplayNotification($"~y~Unable to load LiveLights~w~\nRagePluginHook version ~b~{MinRPHBuild}~w~ or later is required, you are on version ~b~{rphVer.FileMinorPart}");
                return;
            }

            bool rnuiOK = true;
            if (!File.Exists("ragenativeui.dll"))
            {
                rnuiOK = false;
                Game.LogTrivial("RageNativeUI.dll is not installed, and is required to use this mod.");
                Game.DisplayNotification("~y~LiveLights cannot start~s~ - you must download and install RageNativeUI");
            } else
            {
                FileVersionInfo rnuiVer = FileVersionInfo.GetVersionInfo("ragenativeui.dll");
                Game.LogTrivial($"Detected RNUI {rnuiVer.FileVersion}");
                if (new Version(rnuiVer.FileVersion) < MinRNUIVersion)
                {
                    rnuiOK = false;
                    Game.LogTrivial($"RageNativeUI version {MinRNUIVersion}+ is required to use this version of LiveLights");
                    Game.DisplayNotification("~y~LiveLights cannot start~s~ - old version of RageNativeUI detected, please update");
                }
            }

            if (!rnuiOK)
            {
                Game.LogTrivial("Download the latest version at https://github.com/alexguirre/RAGENativeUI/releases/latest");
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

            string sslaFilename = "SirenSetting_Limit_Adjuster.asi";
            bool sslaInstalled = File.Exists(sslaFilename);
            InstalledSSLAVersion = sslaInstalled ? new Version(FileVersionInfo.GetVersionInfo(sslaFilename).FileVersion) : null;

            if (Settings.CheckForUpdates)
            {
                LiveLightsVersionCheck = new GithubVersionCheck("pnwparksfan", "rph-live-lights");
                Game.LogTrivial($"Latest release on github: {LiveLightsVersionCheck.LatestRelease?.TagName}");
                if (LiveLightsVersionCheck.UpdateAvailable)
                {
                    Game.LogTrivial($"Current version is out of date, update is available ({LiveLightsVersionCheck.LatestRelease.TagName})");
                    Game.DisplayNotification("commonmenu", "mp_alerttriangle", "LiveLights by PNWParksFan", "~y~Update Available", $"~b~LiveLights {LiveLightsVersionCheck.LatestRelease.TagName}~w~ is available!\n\n~y~<i>{LiveLightsVersionCheck.LatestRelease.Name}</i>");
                }
                else
                {
                    Game.LogTrivial($"Current version is up to date");
                }

                
                if (sslaInstalled)
                {
                    SSLAVersionCheck = new LCPDFRVersionCheck(28560, InstalledSSLAVersion);
                    Game.LogTrivial($"Siren Setting Limit Adjuster version {SSLAVersionCheck.CurrentVersion} detected");
                    if (SSLAVersionCheck.UpdateAvailable) Game.LogTrivial($"Update to SSLA version {SSLAVersionCheck.LatestVersion} is available");
                } else
                {
                    Game.LogTrivial("Siren Setting Limit Adjuster is not installed");
                }
            }
            
            GameFiber.ExecuteWhile(Menu.MenuController.Process, () => true);
            GameFiber.Hibernate();
        }
    }
}

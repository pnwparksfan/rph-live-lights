using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveLights.Menu
{
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal static class ImportExportMenu
    {
        public static string exportFolder = @"Plugins\LiveLights\carcols\";

        public static void CreateExportFolder()
        {
            if(!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }
        }

        public static void OnImportCarcols(EmergencyLightingMenu menu)
        {
            Game.DisplayNotification("~y~Export not implemented yet");
        }

        public static bool ExportCarcols(EmergencyLightingMenu menu)
        {
            CreateExportFolder();
            string filename = UserInput.GetUserInput("Select an export filename", "Enter a filename", 1000);
            if(!string.IsNullOrWhiteSpace(filename))
            {
                string filepath = Path.Combine(exportFolder, filename);
                if(File.Exists(filename))
                {
                    Game.DisplayNotification($"~y~Unable to export~w~ {filename}~y~: File already exists.");
                    return false;
                }

                try
                {
                    CarcolsFile carcols = new CarcolsFile();
                    SirenSetting setting = menu.ELS.ExportEmergencyLightingToSirenSettings();
                    carcols.SirenSettings.Add(setting);
                    Serializer.SaveItemToXML(carcols, filepath);
                    Game.DisplayNotification($"~g~Successfully exported~w~ \"{menu.ELS.Name}\" ~g~to~w~ \"{filepath}\"");
                    return true;
                } catch (Exception e)
                {
                    Game.DisplayNotification($"~y~Unable to export~w~ {filename}~y~: {e.Message}");
                    return false;
                }
            }

            return false;
        }
    }
}

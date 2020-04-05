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

        public static bool ExportCarcols(EmergencyLighting els, bool allowOverwrite = false)
        {
            CreateExportFolder();
            string filename = UserInput.GetUserInput("Type or paste an export filename (e.g. ~c~~h~carcols-police.meta~h~~w~) or absolute path (e.g. ~c~~h~C:\\mods\\police\\carcols.meta~h~~w~)", "Enter a filename", 1000);
            if(!string.IsNullOrWhiteSpace(filename))
            {
                try
                {
                    // If the user pasted (or manually typed) an absolute path or 
                    // a valid path relative to the GTA root folder, use that 
                    // location. Otherwise, create file in the export folder
                    string filepath = filename;
                    if(!Directory.Exists(Path.GetDirectoryName(filename)))
                    {
                        filepath = Path.Combine(exportFolder, filename);
                        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                    }
                    
                    if(!allowOverwrite && File.Exists(filepath))
                    {
                        Game.DisplayNotification($"~y~Unable to export~w~ {filename}~y~: File already exists.");
                        return false;
                    }

                    CarcolsFile carcols = new CarcolsFile();
                    SirenSetting setting = els.ExportEmergencyLightingToSirenSettings();
                    carcols.SirenSettings.Add(setting);
                    Serializer.SaveItemToXML(carcols, filepath);
                    Game.DisplayNotification($"~g~Successfully exported~w~ \"{els.Name}\" ~g~to~w~ \"{Path.GetFullPath(filepath)}\"");
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

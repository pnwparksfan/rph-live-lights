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

        public static (string filename, string filepath) GetFilepath()
        {
            CreateExportFolder();
            string filename = UserInput.GetUserInput("Type or paste an export filename (e.g. ~c~~h~carcols-police.meta~h~~w~) or absolute path (e.g. ~c~~h~C:\\mods\\police\\carcols.meta~h~~w~)", "Enter a filename", 1000);
            if (!string.IsNullOrWhiteSpace(filename))
            {
                // If the user pasted (or manually typed) an absolute path or 
                // a valid path relative to the GTA root folder, use that 
                // location. Otherwise, use the export folder
                string filepath = filename;
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    filepath = Path.Combine(exportFolder, filename);
                }

                return (filename, filepath);
            }

            return (null, null);
        }

        public static void OnImportCarcols(EmergencyLightingMenu menu)
        {
            (string filename, string filepath) = GetFilepath();
            
            if (!File.Exists(filepath))
            {
                Game.DisplayNotification($"~y~Unable to import~w~ {filename}~y~: File does not exist.");
            }

            try
            {
                CarcolsFile carcols = Serializer.LoadItemFromXML<CarcolsFile>(filepath);
                foreach (var setting in carcols.SirenSettings)
                {
                    Game.LogTrivial($"Importing {setting.Name} from {filename}");
                    var els = new EmergencyLighting();
                    setting.ApplySirenSettingsToEmergencyLighting(els);
                    Game.LogTrivial($"\tImported as {els.Name}");
                }
                Game.DisplayNotification($"Imported ~b~{carcols.SirenSettings.Count}~w~ siren settings from ~b~{filename}");
            } catch (Exception e)
            {
                Game.DisplayNotification($"~y~Error importing~w~ {filename}~y~: {e.Message}");
            }

        }

        public static bool ExportCarcols(EmergencyLighting els, bool allowOverwrite = false)
        {
            (string filename, string filepath) = GetFilepath();
            if(!string.IsNullOrWhiteSpace(filepath))
            {
                try
                {
                    if(!Directory.Exists(Path.GetDirectoryName(filepath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                    }
                    
                    if(!allowOverwrite && File.Exists(filepath))
                    {
                        Game.DisplayNotification($"~y~Unable to export~w~ {filename}~y~: File already exists.");
                        return false;
                    }

                    CarcolsFile carcols = new CarcolsFile();
                    SirenSetting setting = els.ExportEmergencyLightingToSirenSettings();

                    string sirenIdStr = UserInput.GetUserInput("Enter desired siren ID", "", 3);
                    if (byte.TryParse(sirenIdStr, out byte sirenId))
                    {
                        setting.ID = sirenId;
                    } else
                    {
                        Game.DisplayNotification("Unable to parse a valid siren ID, defaulting to ~y~0~w~. Make sure to update the siren ID when using the exported file.");
                    }

                    carcols.SirenSettings.Add(setting);
                    Serializer.SaveItemToXML(carcols, filepath);
                    Game.DisplayNotification($"~g~Successfully exported~w~ \"{els.Name}\" ~g~to~w~ \"{Path.GetFullPath(filepath)}\"");
                    Game.LogTrivial($"Exported {els.Name} to \"{Path.GetFullPath(filepath)}\"");
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

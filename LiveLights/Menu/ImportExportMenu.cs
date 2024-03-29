﻿using System;
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
        static ImportExportMenu()
        {
            ExportMenu = new UIMenu("Export Siren Settings", "");
            
            if (EmergencyLighting.MaxLights > 20)
            {
                MaxExportSirensItem = new UIMenuListItemSelector<int>("Export # siren items", $"Choose the number of siren items to be exported to the carcols.meta file. If you are only using sirens 1-20, export 20. If you are using SSLA to enable >20 sirens, export up to {EmergencyLighting.MaxLights}.", EmergencyLighting.MaxLights, 20, EmergencyLighting.MaxLights);
                MaxExportSirensItem.MenuUpdateBinding = (x) => { if (x < 20 || x > EmergencyLighting.MaxLights) throw new Exception($"Must export between 20 and {EmergencyLighting.MaxLights} sirens"); };
                ExportMenu.AddItem(MaxExportSirensItem);
            }

            ExportAllowOverwriteItem = new UIMenuCheckboxItem("Allow overwrite on export", Settings.DefaultOverwrite, "Allow exported carcols.meta files to overwrite existing files with the same name");
            ExportSelectSettingsMenu = new SirenSettingsSelectionMenuMulti(returnEditable: false);
            ExportSelectSettingsMenu.CreateAndBindToSubmenuItem(ExportMenu, "Select settings to export", "Select one or more siren settings to be exported in a single file");
            ExportItem = new UIMenuItem("Export carcols.meta file", "Exports the selected siren settings to a carcols.meta file");
            ExportMenu.AddItems(ExportAllowOverwriteItem, ExportItem);
            ExportItem.Activated += OnExportActivated;

            ImportActiveSettingMenu = new SirenSettingsSelectionMenu(null, custom: true, builtIn: false, returnEditable: false);
            ImportActiveSettingMenu.Menu.ParentMenu = VehicleMenu.Menu;
            ImportActiveSettingMenu.Menu.ParentItem = VehicleMenu.ImportSelectorItem;
            ImportActiveSettingMenu.Menu.SubtitleText = "Select an imported setting to activate";
            ImportActiveSettingMenu.OnSirenSettingSelected += OnImportedSettingSelected;

            MenuController.Pool.AddAfterYield(ExportMenu);
        }

        private static void OnImportedSettingSelected(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting)
        {
            if (VehicleMenu.Vehicle && VehicleMenu.Vehicle.HasSiren && setting != null && setting.IsValid())
            {
                Game.DisplayNotification($"Activated imported siren setting ~b~{setting.Name}~w~ on current vehicle");
                VehicleMenu.Vehicle.EmergencyLightingOverride = setting;
                VehicleMenu.Refresh();
            }
        }

        public static void OnImportActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            ImportCarcols();
        }

        private static void OnExportActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            ExportCarcols(ExportAllowOverwriteItem.Checked, ExportSelectSettingsMenu.SelectedItems);
        }

        public static UIMenu ExportMenu { get; }
        public static UIMenuListItemSelector<int> MaxExportSirensItem { get; }
        public static UIMenuCheckboxItem ExportAllowOverwriteItem { get; }
        public static SirenSettingsSelectionMenuMulti ExportSelectSettingsMenu { get; }
        public static UIMenuItem ExportItem { get; }

        public static SirenSettingsSelectionMenu ImportActiveSettingMenu { get; }

        public static string exportFolder = @"Plugins\LiveLights\carcols\";

        public static void CreateExportFolder()
        {
            if(!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }
        }

        public static (string filename, string filepath) GetFilepath(string action)
        {
            CreateExportFolder();
            Game.DisplaySubtitle($@"Type or paste an {action.ToLower()} filename (e.g. ~c~~h~carcols-police.meta~h~~w~) or absolute path (e.g. ~c~~h~C:\mods\police\carcols.meta~h~~w~)", 10000);
            string filename = UserInput.GetUserInput($"{action} filename", "", 1000);
            Game.DisplaySubtitle("", 1);
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

        private static void ImportCarcols()
        {
            (string filename, string filepath) = GetFilepath("import");
            
            if (!File.Exists(filepath))
            {
                Game.DisplayNotification($"~y~Unable to import~w~ {filename}~y~: File does not exist.");
                return;
            }

            try
            {
                List<EmergencyLighting> newItems = new List<EmergencyLighting>();
                CarcolsFile carcols = Serializer.LoadItemFromXML<CarcolsFile>(filepath);
                foreach (var setting in carcols.SirenSettings)
                {
                    Game.LogTrivial($"Importing {setting.Name} from {filename}");
                    var els = new EmergencyLighting();
                    setting.ApplySirenSettingsToEmergencyLighting(els);
                    els.SetSource(setting.ID, EmergencyLightingSource.Imported);
                    newItems.Add(els);
                    Game.LogTrivial($"\tImported as {els.Name}");
                }
                Game.DisplayNotification($"Imported ~b~{carcols.SirenSettings.Count}~w~ siren settings from ~b~{filename}");

                VehicleMenu.SirenSettingMenu.RefreshSirenSettingList(true);

                if (VehicleMenu.Vehicle && VehicleMenu.Vehicle.HasSiren)
                {
                    ImportActiveSettingMenu.CustomEntries = newItems;
                    
                    VehicleMenu.Menu.Visible = false;
                    ImportActiveSettingMenu.Menu.Visible = true;
                }

            } catch (Exception e)
            {
                Game.DisplayNotification($"~y~Error importing~w~ {filename}~y~: {e.Message}");
            }

        }

        public static bool ExportCarcols(bool allowOverwrite, IEnumerable<EmergencyLighting> settings) => ExportCarcols(allowOverwrite, settings.ToArray());

        public static bool ExportCarcols(bool allowOverwrite, params EmergencyLighting[] settings)
        {
            int count = settings.Length;
            if (count == 0)
            {
                Game.DisplayNotification("~y~Unable to export~w~ because no siren settings were selected");
                Game.LogTrivial("Unable to export because no siren settings were selected");
                return false;
            }

            (string filename, string filepath) = GetFilepath("export");
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
                        Game.LogTrivial($"Unable to export to \"{filename}\" because file already exists and overwrite is not enabled.");
                        return false;
                    }

                    CarcolsFile carcols = new CarcolsFile();
                    foreach (var els in settings)
                    {
                        Game.LogTrivial($"  Serializing \"{els.Name}\"");
                        SirenSetting setting = els.ExportEmergencyLightingToSirenSettings(MaxExportSirensItem?.ItemValue);
                        var src = els.GetSource();
                        if (src != null) setting.ID = src.SourceId;
                        else setting.ID = 0;

                        carcols.SirenSettings.Add(setting);
                    }

                    Serializer.SaveItemToXML(carcols, filepath);
                    Game.DisplayNotification($"~g~Successfully exported~w~ {count} siren settings ~g~to~w~ \"{Path.GetFullPath(filepath)}\"");
                    Game.LogTrivial($"Exported {count} siren settings to \"{Path.GetFullPath(filepath)}\"");
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

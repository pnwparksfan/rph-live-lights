﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LiveLights.Menu
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal static class VehicleMenu
    {
        static VehicleMenu()
        {
            Menu = new UIMenuRefreshable("Siren Configuration", "~y~No vehicle selected");
            MenuController.Pool.AddAfterYield(Menu);
            Menu.WidthOffset = 250;
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            BannerItem = new UIMenuItem("LiveLights by PNWParksFan", $"LiveLights was created by ~g~PNWParksFan~w~ using the RPH emergency lighting SDK. If you found this plugin useful and made something cool with it, ~y~please mention it in your credits/readme~w~. If you'd like to say thanks, you can donate to support my various modding projects at ~b~parksmods.com/donate~w~ and get member-exclusive perks. Press Enter to learn more!");
            BannerItem.RightLabel = "v" + EntryPoint.CurrentFileVersion.ToString();
            BannerItem.LeftBadge = UIMenuItem.BadgeStyle.Heart;
            BannerItem.BackColor = Color.Black;
            BannerItem.ForeColor = Color.LightSkyBlue;
            BannerItem.HighlightedBackColor = Color.LightSkyBlue;
            BannerItem.Activated += OnBannerClicked;
            Menu.AddItem(BannerItem);

            if(EntryPoint.VersionCheck?.IsUpdateAvailable() == true)
            {
                UpdateItem = new UIMenuItem("Update Available", $"Version ~y~{EntryPoint.VersionCheck.LatestRelease.TagName}~w~ is available for download. Press ~b~Enter~w~ to download ~y~{EntryPoint.VersionCheck.LatestRelease.Name}~w~.");
                UpdateItem.RightLabel = "~o~" + EntryPoint.VersionCheck.LatestRelease.TagName;
                UpdateItem.LeftBadge = UIMenuItem.BadgeStyle.Alert;
                UpdateItem.BackColor = Color.Black;
                UpdateItem.ForeColor = Color.LightSkyBlue;
                UpdateItem.HighlightedBackColor = Color.LightSkyBlue;
                Menu.AddItem(UpdateItem);
                UpdateItem.Activated += OnUpdateClicked;
            }
            
            SirenSettingMenu = new SirenSettingsSelectionMenu(null, true, true, true, false);
            SirenSettingSelectorItem = SirenSettingMenu.CreateAndBindToSubmenuItem(Menu);
            SirenConfigMenu = null; // new EmergencyLightingMenu(null);
            SirenConfigItem = new UIMenuItem("Edit Emergency Lighting", defaultConfigMenuDesc);
            SirenConfigItem.RightLabel = "→";
            Menu.AddItem(SirenConfigItem);
            // Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
            
            EmergencyLightsOnItem = new UIMenuRefreshableCheckboxItem("Emergency Lights Enabled", false, "Toggle flashing lights on this vehicle");
            Menu.AddMenuDataBinding(EmergencyLightsOnItem, (x) => Vehicle.IsSirenOn = x, () => Vehicle.IsSirenOn);

            SirenAudioOnItem = new UIMenuRefreshableCheckboxItem("Siren Audio Enabled", false, "Toggle siren audio on this vehicle");
            Menu.AddMenuDataBinding(SirenAudioOnItem, (x) => Vehicle.IsSirenSilent = !x, () => !Vehicle.IsSirenSilent);

            SirenSettingMenu.OnSirenSettingSelected += OnSirenSelectionChanged;

            Refresh();
            Menu.RefreshIndex();
            
            if(UpdateItem != null)
            {
                Menu.CurrentSelection = 2;
            } else
            {
                Menu.CurrentSelection = 1;
            }
        }

        private static void OnBannerClicked(UIMenu sender, UIMenuItem selectedItem)
        {
            selectedItem.OpenUrl("https://parksmods.com/donate/");
        }

        private static void OnUpdateClicked(UIMenu sender, UIMenuItem selectedItem)
        {
            selectedItem.OpenUrl(EntryPoint.VersionCheck.LatestRelease.HtmlUrl);
        }

        public static void Refresh()
        {
            bool validVehicle = Vehicle.Exists();
            foreach (UIMenuItem menuItem in Menu.MenuItems)
            {
                if(menuItem != UpdateItem && menuItem != BannerItem)
                {
                    menuItem.Enabled = validVehicle;
                }
            }

            if(Vehicle)
            {
                string vehicleName = NativeFunction.Natives.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL<string>(Vehicle.Model.Hash);
                vehicleName = NativeFunction.Natives.x7B5280EBA9840C72<string>(vehicleName);
                Menu.SubtitleText = $"~b~Configure sirens for {vehicleName} ({Vehicle.Model.Name})";
                // EmergencyLighting els = Vehicle.GetOrCreateOverrideEmergencyLighting();
                SirenSettingMenu.SelectedEmergencyLighting = Vehicle.EmergencyLighting;
                ResetConfigMenu();
                Menu.RefreshData(false);
            } else
            {
                SirenSettingMenu.SelectedEmergencyLighting = null;
                Menu.SubtitleText = "~y~No valid vehicle detected";
                SirenSettingSelectorItem.RightLabel = "";
            }
        }

        private static void OnSirenSelectionChanged(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingsSelectionMenu.SirenSettingMenuItem item, EmergencyLighting setting)
        {
            // EmergencyLighting els = setting.GetCustomOrClone();
            if (Vehicle)
            {
                Vehicle.EmergencyLightingOverride = setting;
                Vehicle.RefreshSiren();
            }
            ResetConfigMenu();
        }

        private static void ResetConfigMenu()
        {
            EmergencyLighting els = (Vehicle.Exists()) ? Vehicle.EmergencyLighting : null;

            if(SirenConfigMenu?.ELS.Name != els?.Name || els == null)
            {
                Menu.ReleaseMenuFromItem(SirenConfigItem);
                SirenConfigItem.Activated -= OnNonEditableConfigSelected;
                
                if (els.Exists() && els.IsCustomSetting())
                {
                    SirenConfigMenu = new EmergencyLightingMenu(els);
                    Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
                    SirenConfigItem.Enabled = true;
                    SirenConfigItem.LeftBadge = UIMenuItem.BadgeStyle.None;
                    SirenConfigItem.Description = defaultConfigMenuDesc;
                } else if(els.Exists())
                {
                    SirenConfigMenu = null;
                    SirenConfigItem.Enabled = true;
                    SirenConfigItem.LeftBadge = UIMenuItem.BadgeStyle.Alert;
                    SirenConfigItem.Activated += OnNonEditableConfigSelected;
                    SirenConfigItem.Description = defaultConfigMenuDesc + builtInConfigMenuDesc + new string(' ', 20);
                } else
                {
                    SirenConfigMenu = null;
                    SirenConfigItem.LeftBadge = UIMenuItem.BadgeStyle.None;
                    SirenConfigItem.Enabled = false;
                    SirenConfigItem.Description = "No selected siren setting";
                }
            }
        }

        private static string defaultConfigMenuDesc = "Modify siren settings including flash patterns, environmental lighting, etc.";
        private static string builtInConfigMenuDesc = "~n~~y~This is a built-in siren setting and cannot be directly edited. An editable copy will be created if you edit this setting.";

        private static void OnNonEditableConfigSelected(UIMenu sender, UIMenuItem selectedItem)
        {
            if(Vehicle && Vehicle.EmergencyLighting.Exists())
            {
                Vehicle.EmergencyLightingOverride = Vehicle.EmergencyLighting.Clone();
                SirenSettingMenu.RefreshSirenSettingList();
                SirenSettingMenu.SelectedEmergencyLighting = Vehicle.EmergencyLighting;
                ResetConfigMenu();
            }
        }

        public static Vehicle Vehicle => Game.LocalPlayer.Character.LastVehicle;

        public static UIMenuRefreshable Menu { get; }
        public static UIMenuItem UpdateItem { get; }
        public static SirenSettingsSelectionMenu SirenSettingMenu { get; }
        public static UIMenuItem SirenSettingSelectorItem { get; }
        public static EmergencyLightingMenu SirenConfigMenu { get; private set; }
        public static UIMenuItem SirenConfigItem { get; }
        public static UIMenuRefreshableCheckboxItem EmergencyLightsOnItem { get; }
        public static UIMenuRefreshableCheckboxItem SirenAudioOnItem { get; }
        public static UIMenuItem BannerItem { get; }
    }
}

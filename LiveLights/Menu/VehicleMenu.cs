using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Menu.SetMenuWidthOffset(250);
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            
            SirenSettingMenu = new SirenSettingsSelectionMenu(null, true, true, true, false);
            SirenSettingSelectorItem = SirenSettingMenu.CreateAndBindToSubmenuItem(Menu);
            SirenConfigMenu = null; // new EmergencyLightingMenu(null);
            SirenConfigItem = new UIMenuItem("Edit Emergency Lighting", "Modify siren settings including flash patterns, environmental lighting, etc.");
            Menu.AddItem(SirenConfigItem);
            // Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
            
            EmergencyLightsOnItem = new UIMenuRefreshableCheckboxItem("Emergency Lights Enabled", false, "Toggle flashing lights on this vehicle");
            Menu.AddMenuDataBinding(EmergencyLightsOnItem, (x) => Vehicle.IsSirenOn = x, () => Vehicle.IsSirenOn);

            SirenAudioOnItem = new UIMenuRefreshableCheckboxItem("Siren Audio Enabled", false, "Toggle siren audio on this vehicle");
            Menu.AddMenuDataBinding(SirenAudioOnItem, (x) => Vehicle.IsSirenSilent = !x, () => !Vehicle.IsSirenSilent);

            SirenSettingMenu.OnSirenSettingSelected += OnSirenSelectionChanged;

            Refresh();
        }

        public static void Refresh()
        {
            bool validVehicle = Vehicle.Exists();
            foreach (UIMenuItem menuItem in Menu.MenuItems)
            {
                menuItem.Enabled = validVehicle;
            }

            if(Vehicle)
            {
                string vehicleName = NativeFunction.Natives.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL<string>(Vehicle.Model.Hash);
                vehicleName = NativeFunction.Natives.x7B5280EBA9840C72<string>(vehicleName);
                Menu.Subtitle.Caption = $"~b~Configure sirens for {vehicleName} ({Vehicle.Model.Name})";
                // EmergencyLighting els = Vehicle.GetOrCreateOverrideEmergencyLighting();
                SirenSettingMenu.SelectedEmergencyLighting = Vehicle.EmergencyLighting;
                ResetConfigMenu();
                Menu.RefreshData(false);
            } else
            {
                Menu.Subtitle.Caption = "~y~No valid vehicle detected";
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

            if(SirenConfigMenu?.ELS.Name != els.Name)
            {
                Menu.ReleaseMenuFromItem(SirenConfigItem);
                if(els.Exists() && els.IsCustomSetting())
                {
                    SirenConfigMenu = new EmergencyLightingMenu(els);
                    Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
                    SirenConfigItem.Enabled = true;
                } else
                {
                    SirenConfigItem.Enabled = false;
                    SirenConfigMenu = null;
                }
            }
        }

        public static Vehicle Vehicle => Game.LocalPlayer.Character.LastVehicle;

        public static UIMenuRefreshable Menu { get; }
        public static SirenSettingsSelectionMenu SirenSettingMenu { get; }
        public static UIMenuItem SirenSettingSelectorItem { get; }
        public static EmergencyLightingMenu SirenConfigMenu { get; private set; }
        public static UIMenuItem SirenConfigItem { get; }
        public static UIMenuRefreshableCheckboxItem EmergencyLightsOnItem { get; }
        public static UIMenuRefreshableCheckboxItem SirenAudioOnItem { get; }
    }
}

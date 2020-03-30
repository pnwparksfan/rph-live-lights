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

    internal class VehicleMenu
    {
        public VehicleMenu(Vehicle v)
        {
            Vehicle = v;
            
            Menu = new UIMenu("Siren Configuration", $"~b~Configure emergency lighting for {v.Model.Name}");
            MenuController.Pool.Add(Menu);
            Menu.SetMenuWidthOffset(250);
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            EmergencyLighting els = v.GetOrCreateOverrideEmergencyLighting();
            SirenSettingMenu = new SirenSettingsSelectionMenu(els, true, true, true);
            SirenSettingSelectorItem = SirenSettingMenu.CreateAndBindToSubmenuItem(Menu);
            SirenConfigMenu = new EmergencyLightingMenu(els);
            SirenConfigItem = new UIMenuItem("Edit Emergency Lighting", "Modify siren settings including flash patterns, environmental lighting, etc.");
            Menu.AddItem(SirenConfigItem);
            Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
            SirenSettingMenu.OnSirenSettingSelected += OnSirenSelectionChanged;
        }

        private void OnSirenSelectionChanged(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingsSelectionMenu.SirenSettingMenuItem item, EmergencyLighting setting)
        {
            EmergencyLighting els = setting.GetCustomOrClone();
            if (Vehicle)
            {
                Vehicle.EmergencyLightingOverride = els;
            }
            Menu.ReleaseMenuFromItem(SirenConfigItem);
            SirenConfigMenu = new EmergencyLightingMenu(els);
            Menu.BindMenuToItem(SirenConfigMenu.Menu, SirenConfigItem);
        }

        public Vehicle Vehicle { get; }

        public UIMenu Menu { get; }
        public SirenSettingsSelectionMenu SirenSettingMenu { get; }
        public UIMenuItem SirenSettingSelectorItem { get; }
        public EmergencyLightingMenu SirenConfigMenu { get; private set; }
        public UIMenuItem SirenConfigItem { get; }
    }
}

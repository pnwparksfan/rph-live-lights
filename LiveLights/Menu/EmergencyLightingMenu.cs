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

    internal class EmergencyLightingMenu
    {
        public EmergencyLightingMenu(EmergencyLighting els)
        {
            this.ELS = els;

            Menu = new UIMenu("Emergency Lighting Settings", "");
            Menu.SetMenuWidthOffset(150);
            Menu.ControlDisablingEnabled = false;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            NameItem = new UIMenuStringSelector("Name", ELS.Name, "Siren setting name as shown in carcols.meta");
            Menu.AddItem(NameItem);
            NameItem.OnValueChanged += (string name) => ELS.Name = name;
            
            BpmItem = new UIMenuUIntSelector("BPM", ELS.SequencerBpm, "Beats per minute");
            Menu.AddItem(BpmItem);
            BpmItem.OnValueChanged += (uint bpm) => ELS.SequencerBpm = bpm;

            TextureHashItem = new UIMenuStringSelector("Texture Hash", Utils.TextureHash.HashToString(ELS.TextureHash));
            Menu.AddItem(TextureHashItem);

            MenuController.Pool.Add(Menu);
        }

        private void OnNameChanged(UIMenu sender, UIMenuItem menuItem, UIMenuValueEntrySelector<string, UIMenuItem> selector, string value)
        {
            ELS.Name = value;
        }

        public EmergencyLighting ELS { get; }

        public UIMenu Menu { get; } 
        public UIMenuStringSelector NameItem { get; } 
        public UIMenuUIntSelector BpmItem { get; }
        public UIMenuStringSelector TextureHashItem { get; }

    }
}

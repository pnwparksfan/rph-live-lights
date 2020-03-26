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
            Menu = new UIMenu("Emergency Lighting Settings", "");
            Menu.SetMenuWidthOffset(150);
            
            NameItem = new UIMenuStringSelector("Name", els.Name, "Siren setting name as shown in carcols.meta");
            Menu.AddItem(NameItem);
            
            BpmItem = new UIMenuUIntSelector("BPM", els.SequencerBpm, "Beats per minute");
            Menu.AddItem(BpmItem);

            TextureHashItem = new UIMenuStringSelector("Texture Hash", Utils.TextureHash.HashToString(els.TextureHash));
            Menu.AddItem(TextureHashItem);

            MenuController.Pool.Add(Menu);
        }

        public UIMenu Menu { get; } 
        public UIMenuStringSelector NameItem { get; } 
        public UIMenuUIntSelector BpmItem { get; }
        public UIMenuStringSelector TextureHashItem { get; }

    }
}

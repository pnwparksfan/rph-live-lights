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
            NameItem = new UIMenuStringSelector("Name", els.Name, "Siren setting name as shown in carcols.meta");
            BpmItem = new UIMenuUIntSelector("BPM", els.SequencerBpm, "Beats per minute");
        }

        public UIMenu Menu { get; } 
        public UIMenuStringSelector NameItem { get; } 
        public UIMenuUIntSelector BpmItem { get; }

    }
}

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

    internal class EmergencyLightMenu : IDisplayItem
    {
        public int SirenID { get; }
        
        public EmergencyLightMenu(EmergencyLighting els, int i)
        {
            this.Siren = els.Lights[i];
            this.SirenID = (i + 1);

            Menu = new UIMenuRefreshable("Edit Siren", $"~b~Siren Setting \"{els.Name}\" > {DisplayText}");

            FlashSequenceItem = new UIMenuStringSelector("Flash Sequence", Siren.FlashinessSequence, $"32-bit flash sequence for siren {SirenID}. 1 represents on, 0 represents off.");
            Menu.AddMenuDataBinding(FlashSequenceItem, (x) => Siren.FlashinessSequence = x, () => Siren.FlashinessSequence);
        }

        public EmergencyLight Siren { get; }

        public UIMenuRefreshable Menu { get; }

        public object Value => Menu;

        public string DisplayText => $"Siren {SirenID}";

        public bool Equals(IDisplayItem other)
        {
            return (other is EmergencyLightMenu) && ((EmergencyLightMenu)other).Siren == Siren;
        }

        public override int GetHashCode() => Siren.GetHashCode();

        // Siren settings
        public UIMenuStringSelector FlashSequenceItem { get; }

    }
}

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
            
            // Set up flashiness sub-menu
            FlashinessMenu = new UIMenuRefreshable(Menu.Title.Caption, Menu.Subtitle.Caption + " > Flashiness");
            FlashinessMenuItem = new UIMenuItem("Flashiness Settings", "Configure sequence, multiples, angle, and other settings for ~y~flashing~w~ light");
            FlashinessMenuItem.SetRightLabel("→");
            Menu.AddItem(FlashinessMenuItem);
            Menu.BindMenuToItem(FlashinessMenu, FlashinessMenuItem);

            // Set up rotation sub-menu
            RotationMenu = new UIMenuRefreshable(Menu.Title.Caption, Menu.Subtitle.Caption + " > Rotation");
            RotationMenuItem = new UIMenuItem("Rotation Settings", "Configure sequence, multiples, angle, and other settings for ~y~rotating~w~ light");
            RotationMenuItem.SetRightLabel("→");
            Menu.AddItem(RotationMenuItem);
            Menu.BindMenuToItem(RotationMenu, RotationMenuItem);

            // Set up corona sub-menu
            CoronaMenu = new UIMenuRefreshable(Menu.Title.Caption, Menu.Subtitle.Caption + " > Corona");
            CoronaMenuItem = new UIMenuItem("Corona Settings", "Configure size, intensity, and other corona settings");
            CoronaMenuItem.SetRightLabel("→");
            Menu.AddItem(CoronaMenuItem);
            Menu.BindMenuToItem(CoronaMenu, CoronaMenuItem);


            FlashSequenceItem = new UIMenuStringSelector("Flash Sequence", Siren.FlashinessSequence, $"32-bit flash sequence for siren {SirenID}. ~g~1~w~ represents on, ~y~0~w~ represents off.");
            FlashinessMenu.AddMenuDataBinding(FlashSequenceItem, (x) => Siren.FlashinessSequence = x, () => Siren.FlashinessSequence);

            
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

        // Flashing submenu
        public UIMenuRefreshable FlashinessMenu { get; }
        public UIMenuItem FlashinessMenuItem { get; }
        public UIMenuStringSelector FlashSequenceItem { get; }

        // Rotation submenu
        public UIMenuRefreshable RotationMenu { get; }
        public UIMenuItem RotationMenuItem { get; }

        // Corona submenu
        public UIMenuRefreshable CoronaMenu { get; }
        public UIMenuItem CoronaMenuItem { get; }


    }
}

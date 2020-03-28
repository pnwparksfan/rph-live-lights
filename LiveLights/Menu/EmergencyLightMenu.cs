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

            // Flashiness menu items
            FlashEnabledItem = new UIMenuRefreshableCheckboxItem("Flash Enabled", Siren.Flash, "Enable/disable this siren from flashing. Note: setting to False for a siren which was previously on may result in the siren being stuck on temporarily. Cycle siren off/on to reset.");
            FlashinessMenu.AddMenuDataBinding(FlashEnabledItem, (x) => Siren.Flash = x, () => Siren.Flash);
            
            FlashSequenceItem = new UIMenuStringSelector("Flash Sequence", Siren.FlashinessSequence, $"32-bit flash sequence for siren {SirenID}. ~g~1~w~ represents on, ~y~0~w~ represents off.") { MaxLength = 32 };
            FlashinessMenu.AddMenuDataBinding(FlashSequenceItem, (x) => Siren.FlashinessSequence = x, () => Siren.FlashinessSequence);

            FlashSequenceRawItem = new UIMenuUIntSelector("Flash Sequence (raw)", Siren.FlashinessSequenceRaw, "32-bit unsigned integer representation of siren sequence. This value is how the sequence is represented in carcols.meta. Automatically updates/updated by binary formatted sequence above.");
            FlashinessMenu.AddMenuDataBinding(FlashSequenceRawItem, (x) => Siren.FlashinessSequenceRaw = x, () => Siren.FlashinessSequenceRaw);

            FlashMultiplesItem = new UIMenuListItemSelector<byte>("Flash Multiples", "How many times the corona flashes for each sequence step the light is on", Siren.FlashinessMultiples, CommonSelectionItems.MultiplesBytes);
            FlashinessMenu.AddMenuDataBinding(FlashMultiplesItem, (x) => Siren.FlashinessMultiples = x, () => Siren.FlashinessMultiples);

            FlashDeltaItem = new UIMenuListItemSelector<float>("Flash Delta", "Angle the light should flash at", Siren.FlashinessDelta, CommonSelectionItems.UnitCircleDegrees);
            FlashinessMenu.AddMenuDataBinding(FlashDeltaItem, (x) => Siren.FlashinessDelta = x, () => Siren.FlashinessDelta);

            FlashDeltaRadItem = new UIMenuFloatSelector("Flash Delta (Radians)", MathHelper.ConvertDegreesToRadians(Siren.FlashinessDelta), "Angle the light should flash at in radians");
            FlashinessMenu.AddMenuDataBinding(FlashDeltaRadItem, (x) => Siren.FlashinessDelta = MathHelper.ConvertRadiansToDegrees(x), () => MathHelper.ConvertDegreesToRadians(Siren.FlashinessDelta));

            FlashStartItem = new UIMenuListItemSelector<float>("Flash Start", "Starting rotation angle of the light (usually 0 for flashing sirens)", Siren.FlashinessStart, CommonSelectionItems.UnitCircleDegrees);
            FlashinessMenu.AddMenuDataBinding(FlashStartItem, (x) => Siren.FlashinessStart = x, () => Siren.FlashinessStart);

            FlashSpeedItem = new UIMenuFloatSelector("Flash Speed", Siren.FlashinessSpeed, "How fast the light flashes within each beat");
            FlashinessMenu.AddMenuDataBinding(FlashSpeedItem, (x) => Siren.FlashinessSpeed = x, () => Siren.FlashinessSpeed);

            FlashDirectionItem = new UIMenuRefreshableCheckboxItem("Flash Direction Enabled", Siren.FlashinessDirection, "Enable/disable flash direction");
            FlashinessMenu.AddMenuDataBinding(FlashDirectionItem, (x) => Siren.FlashinessDirection = x, () => Siren.FlashinessDirection);

            FlashSyncBpmItem = new UIMenuRefreshableCheckboxItem("Sync Flash to BPM", Siren.FlashinessSynchronizeToBpm, "Sync flash pattern to BPM");
            FlashinessMenu.AddMenuDataBinding(FlashSyncBpmItem, (x) => Siren.FlashinessSynchronizeToBpm = x, () => Siren.FlashinessSynchronizeToBpm);
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
        public UIMenuRefreshableCheckboxItem FlashEnabledItem { get; }
        public UIMenuListItemSelector<float> FlashDeltaItem { get; }
        public UIMenuFloatSelector FlashDeltaRadItem { get; }
        public UIMenuListItemSelector<float> FlashStartItem { get; }
        public UIMenuFloatSelector FlashStartRadItem { get; }
        public UIMenuFloatSelector FlashSpeedItem { get; }
        public UIMenuStringSelector FlashSequenceItem { get; }
        public UIMenuUIntSelector FlashSequenceRawItem { get; }
        public UIMenuListItemSelector<byte> FlashMultiplesItem { get; }
        public UIMenuRefreshableCheckboxItem FlashDirectionItem { get; }
        public UIMenuRefreshableCheckboxItem FlashSyncBpmItem { get; }

        // Rotation submenu
        public UIMenuRefreshable RotationMenu { get; }
        public UIMenuItem RotationMenuItem { get; }

        // Corona submenu
        public UIMenuRefreshable CoronaMenu { get; }
        public UIMenuItem CoronaMenuItem { get; }


    }
}

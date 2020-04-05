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
    using System.Drawing;

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
            FlashEnabledItem = new UIMenuRefreshableCheckboxItem("Flash Enabled", Siren.Flash, "Enable/disable this siren from flashing. Note: setting to False for a siren which was previously on may result in the siren being stuck on temporarily. Toggle vehicle's sirens off/on to reset.");
            FlashinessMenu.AddMenuDataBinding(FlashEnabledItem, (x) => Siren.Flash = x, () => Siren.Flash);
            
            FlashSequenceItem = new UIMenuStringSelector("Flash Sequence", Siren.FlashinessSequence, $"32-bit flash sequence for siren {SirenID}. ~g~1~w~ represents on, ~y~0~w~ represents off.") { MaxLength = 32 };
            FlashinessMenu.AddMenuDataBinding(FlashSequenceItem, (x) => Siren.FlashinessSequence = x, () => Siren.FlashinessSequence, () => FlashSequenceRawItem);

            FlashSequenceRawItem = new UIMenuUIntSelector("Flash Sequence (raw)", Siren.FlashinessSequenceRaw, "32-bit unsigned integer representation of siren sequence. This value is how the sequence is represented in carcols.meta. Automatically updates/updated by binary formatted sequence above.");
            FlashinessMenu.AddMenuDataBinding(FlashSequenceRawItem, (x) => Siren.FlashinessSequenceRaw = x, () => Siren.FlashinessSequenceRaw, () => FlashSequenceItem);

            FlashMultiplesItem = new UIMenuListItemSelector<byte>("Flash Multiples", "How many times the corona flashes for each sequence step the light is on", Siren.FlashinessMultiples, CommonSelectionItems.MultiplesBytes);
            FlashinessMenu.AddMenuDataBinding(FlashMultiplesItem, (x) => Siren.FlashinessMultiples = x, () => Siren.FlashinessMultiples);

            FlashDeltaItem = new UIMenuListItemSelector<float>("Flash Delta", "Angle the light should flash at", Siren.FlashinessDelta, CommonSelectionItems.UnitCircleDegrees);
            FlashinessMenu.AddMenuDataBinding(FlashDeltaItem, (x) => Siren.FlashinessDelta = x, () => Siren.FlashinessDelta, () => FlashDeltaRadItem);

            FlashDeltaRadItem = new UIMenuFloatSelector("Flash Delta (Radians)", MathHelper.ConvertDegreesToRadians(Siren.FlashinessDelta), "Angle the light should flash at in radians");
            FlashinessMenu.AddMenuDataBinding(FlashDeltaRadItem, (x) => Siren.FlashinessDelta = MathHelper.ConvertRadiansToDegrees(x), () => MathHelper.ConvertDegreesToRadians(Siren.FlashinessDelta), () => FlashDeltaItem);

            FlashStartItem = new UIMenuListItemSelector<float>("Flash Start", "Starting rotation angle of the light (usually 0 for flashing sirens)", Siren.FlashinessStart, CommonSelectionItems.UnitCircleDegrees);
            FlashinessMenu.AddMenuDataBinding(FlashStartItem, (x) => Siren.FlashinessStart = x, () => Siren.FlashinessStart, () => FlashStartRadItem);

            FlashStartRadItem = new UIMenuFloatSelector("Flash Start (Radians)", MathHelper.ConvertDegreesToRadians(Siren.FlashinessStart), "Starting rotation angle of the light in radians");
            FlashinessMenu.AddMenuDataBinding(FlashStartRadItem, (x) => Siren.FlashinessStart = MathHelper.ConvertRadiansToDegrees(x), () => MathHelper.ConvertDegreesToRadians(Siren.FlashinessStart), () => FlashStartItem);

            FlashSpeedItem = new UIMenuFloatSelector("Flash Speed", Siren.FlashinessSpeed, "How fast the light flashes within each beat");
            FlashinessMenu.AddMenuDataBinding(FlashSpeedItem, (x) => Siren.FlashinessSpeed = x, () => Siren.FlashinessSpeed);

            FlashDirectionItem = new UIMenuRefreshableCheckboxItem("Flash Direction Enabled", Siren.FlashinessDirection, "Enable/disable flash direction");
            FlashinessMenu.AddMenuDataBinding(FlashDirectionItem, (x) => Siren.FlashinessDirection = x, () => Siren.FlashinessDirection);

            FlashSyncBpmItem = new UIMenuRefreshableCheckboxItem("Sync Flash to BPM", Siren.FlashinessSynchronizeToBpm, "Sync flash pattern to BPM");
            FlashinessMenu.AddMenuDataBinding(FlashSyncBpmItem, (x) => Siren.FlashinessSynchronizeToBpm = x, () => Siren.FlashinessSynchronizeToBpm);

            // Rotation menu items
            RotationEnabledItem = new UIMenuRefreshableCheckboxItem("Rotation Enabled", Siren.Rotate, "Enable/disable this siren from rotating. Note: setting to False for a siren which was previously on may result in the siren being stuck on temporarily. Toggle vehicle's sirens off/on to reset.");
            RotationMenu.AddMenuDataBinding(RotationEnabledItem, (x) => Siren.Rotate = x, () => Siren.Rotate);

            RotationSequenceItem = new UIMenuStringSelector("Rotation Sequence", Siren.RotationSequence, $"32-bit Rotation sequence for siren {SirenID}. ~g~1~w~ represents on, ~y~0~w~ represents off.") { MaxLength = 32 };
            RotationMenu.AddMenuDataBinding(RotationSequenceItem, (x) => Siren.RotationSequence = x, () => Siren.RotationSequence, () => RotationSequenceRawItem);

            RotationSequenceRawItem = new UIMenuUIntSelector("Rotation Sequence (raw)", Siren.RotationSequenceRaw, "32-bit unsigned integer representation of siren sequence. This value is how the sequence is represented in carcols.meta. Automatically updates/updated by binary formatted sequence above.");
            RotationMenu.AddMenuDataBinding(RotationSequenceRawItem, (x) => Siren.RotationSequenceRaw = x, () => Siren.RotationSequenceRaw, () => RotationSequenceItem);

            RotationMultiplesItem = new UIMenuListItemSelector<byte>("Rotation Multiples", "How many times the corona rotates for each sequence step the light is on", Siren.RotationMultiples, CommonSelectionItems.MultiplesBytes);
            RotationMenu.AddMenuDataBinding(RotationMultiplesItem, (x) => Siren.RotationMultiples = x, () => Siren.RotationMultiples);

            RotationDeltaItem = new UIMenuListItemSelector<float>("Rotation Delta", "Angle the light should rotate to", Siren.RotationDelta, CommonSelectionItems.UnitCircleDegrees);
            RotationMenu.AddMenuDataBinding(RotationDeltaItem, (x) => Siren.RotationDelta = x, () => Siren.RotationDelta, () => RotationDeltaRadItem);

            RotationDeltaRadItem = new UIMenuFloatSelector("Rotation Delta (Radians)", MathHelper.ConvertDegreesToRadians(Siren.RotationDelta), "Angle the light should rotate to in radians");
            RotationMenu.AddMenuDataBinding(RotationDeltaRadItem, (x) => Siren.RotationDelta = MathHelper.ConvertRadiansToDegrees(x), () => MathHelper.ConvertDegreesToRadians(Siren.RotationDelta), () => RotationDeltaItem);

            RotationStartItem = new UIMenuListItemSelector<float>("Rotation Start", "Starting rotation angle of the light", Siren.RotationStart, CommonSelectionItems.UnitCircleDegrees);
            RotationMenu.AddMenuDataBinding(RotationStartItem, (x) => Siren.RotationStart = x, () => Siren.RotationStart, () => RotationStartRadItem);

            RotationStartRadItem = new UIMenuFloatSelector("Rotation Start (Radians)", MathHelper.ConvertDegreesToRadians(Siren.RotationStart), "Starting rotation angle of the light in radians");
            RotationMenu.AddMenuDataBinding(RotationStartRadItem, (x) => Siren.RotationStart = MathHelper.ConvertRadiansToDegrees(x), () => MathHelper.ConvertDegreesToRadians(Siren.RotationStart), () => RotationStartItem);

            RotationSpeedItem = new UIMenuFloatSelector("Rotation Speed", Siren.RotationSpeed, "How fast the light rotates within each beat");
            RotationMenu.AddMenuDataBinding(RotationSpeedItem, (x) => Siren.RotationSpeed = x, () => Siren.RotationSpeed);

            RotationDirectionItem = new UIMenuRefreshableCheckboxItem("Rotation Direction Enabled", Siren.RotationDirection, "Enable/disable rotation direction");
            RotationMenu.AddMenuDataBinding(RotationDirectionItem, (x) => Siren.RotationDirection = x, () => Siren.RotationDirection);

            RotationSyncBpmItem = new UIMenuRefreshableCheckboxItem("Sync Rotation to BPM", Siren.RotationSynchronizeToBpm, "Sync Rotation pattern to BPM");
            RotationMenu.AddMenuDataBinding(RotationSyncBpmItem, (x) => Siren.RotationSynchronizeToBpm = x, () => Siren.RotationSynchronizeToBpm);

            // Corona menu items
            CoronaIntensityItem = new UIMenuFloatSelector("Corona Intensity", Siren.CoronaIntensity, "Brightness/intensity of the corona for this siren");
            CoronaMenu.AddMenuDataBinding(CoronaIntensityItem, (x) => Siren.CoronaIntensity = x, () => Siren.CoronaIntensity);

            CoronaSizeItem = new UIMenuFloatSelector("Corona Size", Siren.CoronaSize, "Size of corona for this siren");
            CoronaMenu.AddMenuDataBinding(CoronaSizeItem, (x) => Siren.CoronaSize = x, () => Siren.CoronaSize);

            CoronaPullItem = new UIMenuFloatSelector("Corona Pull", Siren.CoronaPull, "Corona pull, affects how visible corona is through vehicle mesh");
            CoronaMenu.AddMenuDataBinding(CoronaPullItem, (x) => Siren.CoronaPull = x, () => Siren.CoronaPull);

            CoronaFaceCameraItem = new UIMenuRefreshableCheckboxItem("Corona Face Camera", Siren.CoronaFaceCamera, "Enable/disable corona to always be visible regardless of camera angle");
            CoronaMenu.AddMenuDataBinding(CoronaFaceCameraItem, (x) => Siren.CoronaFaceCamera = x, () => Siren.CoronaFaceCamera);

            // Remaining main menu items
            EnvLightItem = new UIMenuRefreshableCheckboxItem("Env Light", Siren.Light, "Enable/disable environmental lighting from this siren");
            Menu.AddMenuDataBinding(EnvLightItem, (x) => Siren.Light = x, () => Siren.Light);

            ColorItem = new UIMenuColorSelector("Color", "Color of corona and environmental lighting from this siren. You can any typical recognized color name (e.g. \"Indigo\"), or hex format as 0xAARRGGBB or 0xRRGGBB (e.g. \"0xFFFF00AA\").", Siren.Color, CommonSelectionItems.CommonColors);
            Menu.AddMenuDataBinding(ColorItem, (x) => Siren.Color = x, () => Siren.Color);

            ScaleToggleItem = new UIMenuRefreshableCheckboxItem("Scale Sirens", Siren.Scale, "Enable/disable scaling sirens up when they flash. Should be enabled for flashing lights and disabled for rotating lights.");
            Menu.AddMenuDataBinding(ScaleToggleItem, (x) => Siren.Scale = x, () => Siren.Scale);

            ScaleFactorItem = new UIMenuListItemSelector<byte>("Scale Factor", "How much to scale up siren when siren is flashed on. Default is 2 for most flashing sirens.", Siren.ScaleFactor, CommonSelectionItems.ScaleFactorByte);
            Menu.AddMenuDataBinding(ScaleFactorItem, (x) => Siren.ScaleFactor = x, () => Siren.ScaleFactor);
            
            IntensityItem = new UIMenuListItemSelector<float>("Intensity", "Intensity of environmental lighting emitted by this light", Siren.Intensity, CommonSelectionItems.IntensityFloat);
            Menu.AddMenuDataBinding(IntensityItem, (x) => Siren.Intensity = x, () => Siren.Intensity);

            SpotLightItem = new UIMenuRefreshableCheckboxItem("Spot Light", Siren.SpotLight, "Enable/disable spotlight effect on environmental light from this siren");
            Menu.AddMenuDataBinding(SpotLightItem, (x) => Siren.SpotLight = x, () => Siren.SpotLight);

            CastShadowsItem = new UIMenuRefreshableCheckboxItem("Cast Shadows", Siren.CastShadows, "Enable/disable casting shadows with environmental light from this siren");
            Menu.AddMenuDataBinding(CastShadowsItem, (x) => Siren.CastShadows = x, () => Siren.CastShadows);

            LightGroupItem = new UIMenuListItemSelector<byte>("Light Group", "Light group for siren. Usage not documented.", Siren.LightGroup, CommonSelectionItems.LightGroupByte);
            Menu.AddMenuDataBinding(LightGroupItem, (x) => Siren.LightGroup = x, () => Siren.LightGroup);

            // Final setup
            FlashinessMenu.RefreshIndex();
            RotationMenu.RefreshIndex();
            CoronaMenu.RefreshIndex();
            Menu.RefreshIndex();
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
        public UIMenuRefreshableCheckboxItem RotationEnabledItem { get; }
        public UIMenuListItemSelector<float> RotationDeltaItem { get; }
        public UIMenuFloatSelector RotationDeltaRadItem { get; }
        public UIMenuListItemSelector<float> RotationStartItem { get; }
        public UIMenuFloatSelector RotationStartRadItem { get; }
        public UIMenuFloatSelector RotationSpeedItem { get; }
        public UIMenuStringSelector RotationSequenceItem { get; }
        public UIMenuUIntSelector RotationSequenceRawItem { get; }
        public UIMenuListItemSelector<byte> RotationMultiplesItem { get; }
        public UIMenuRefreshableCheckboxItem RotationDirectionItem { get; }
        public UIMenuRefreshableCheckboxItem RotationSyncBpmItem { get; }

        // Corona submenu
        public UIMenuRefreshable CoronaMenu { get; }
        public UIMenuItem CoronaMenuItem { get; }
        public UIMenuFloatSelector CoronaIntensityItem { get; }
        public UIMenuFloatSelector CoronaSizeItem { get; }
        public UIMenuFloatSelector CoronaPullItem { get; }
        public UIMenuRefreshableCheckboxItem CoronaFaceCameraItem { get; }

        // Other siren properties
        // public UIMenuListItemSelector<KnownColor> ColorItem { get; }
        public UIMenuRefreshableCheckboxItem EnvLightItem { get; }
        public UIMenuColorSelector ColorItem { get; }
        public UIMenuListItemSelector<float> IntensityItem { get; }
        public UIMenuListItemSelector<byte> LightGroupItem { get; }
        public UIMenuRefreshableCheckboxItem ScaleToggleItem { get; }
        public UIMenuListItemSelector<byte> ScaleFactorItem { get; }
        public UIMenuRefreshableCheckboxItem SpotLightItem { get; }
        public UIMenuRefreshableCheckboxItem CastShadowsItem { get; }

    }
}

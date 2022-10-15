using System;
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

    internal class EmergencyLightingMenu
    {
        public EmergencyLightingMenu(EmergencyLighting els)
        {
            this.ELS = els;

            Menu = new UIMenuRefreshable("Emergency Lighting Settings", $"~b~Siren Setting \"{els.Name}\"");
            Menu.WidthOffset = 250;
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;
            Menu.MaxItemsOnScreen = 15;

            // Main siren settings

            NameItem = new UIMenuStringSelector("Name", ELS.Name, "Siren setting name as shown in carcols.meta");
            Menu.AddMenuDataBinding(NameItem, (x) => ELS.Name = x, () => ELS.Name);

            IdItem = new UIMenuUIntSelector("Siren Setting ID", GetSourceID(), "Siren setting ID which will be exported to carcols.meta");
            Menu.AddMenuDataBinding(IdItem, SetSourceID, GetSourceID);

            BpmItem = new UIMenuUIntSelector("BPM", ELS.SequencerBpm, "Beats per minute");
            Menu.AddMenuDataBinding(BpmItem, (x) => ELS.SequencerBpm = x, () => ELS.SequencerBpm);

            TimeMultiplierItem = new UIMenuListItemSelector<float>("Time Multiplier", "Adjusts how fast BPM is scaled", ELS.TimeMultiplier, CommonSelectionItems.MultiplierFloat);
            Menu.AddMenuDataBinding(TimeMultiplierItem, (x) => ELS.TimeMultiplier = x, () => ELS.TimeMultiplier);

            TextureHashItem = new UIMenuListItemSelector<string>("Texture Hash", "Texture which is shown for environmental lighting from this vehicle", TextureHash.HashToString(ELS.TextureHash), TextureHash.lightTextureHashes.Values);
            TextureHashItem.ListMenuItem.SetAddNewItems((x) => x);
            Menu.AddMenuDataBinding(TextureHashItem, (x) => ELS.TextureHash = Utils.TextureHash.StringToHash(x), () => Utils.TextureHash.HashToString(ELS.TextureHash));

            FalloffMaxItem = new UIMenuListItemSelector<float>("Falloff Max", "Affects how far environmental lighting shines", ELS.LightFalloffMax, 8, 16, 32, 75, 100, 150, 200, 250, 300);
            Menu.AddMenuDataBinding(FalloffMaxItem, (x) => ELS.LightFalloffMax = x, () => ELS.LightFalloffMax);

            FalloffExponentItem = new UIMenuListItemSelector<float>("Falloff Exponent", "Affects how far environmental lighting shines", ELS.LightFalloffExponent, 8, 16, 32, 75, 100, 150, 200, 250, 300);
            Menu.AddMenuDataBinding(FalloffExponentItem, (x) => ELS.LightFalloffExponent = x, () => ELS.LightFalloffExponent);

            InnerConeAngleItem = new UIMenuListItemSelector<float>("Inner Cone Angle", "Inner angle of environmental lighting cone emitted from sirens", ELS.LightInnerConeAngle, 0f, 3f, 10f, 15f);
            Menu.AddMenuDataBinding(InnerConeAngleItem, (x) => ELS.LightInnerConeAngle = x, () => ELS.LightInnerConeAngle);

            OuterConeAngleItem = new UIMenuListItemSelector<float>("Outer Cone Angle", "Outer angle of environmental lighting cone emitted from sirens", ELS.LightOuterConeAngle, 30f, 40f, 50f, 60f, 70f, 80f, 90f);
            Menu.AddMenuDataBinding(OuterConeAngleItem, (x) => ELS.LightOuterConeAngle = x, () => ELS.LightOuterConeAngle);

            RealLightsItem = new UIMenuRefreshableCheckboxItem("Use Real Lights", ELS.UseRealLights, "Configured whether to use real lights (exact effect unknown)");
            Menu.AddMenuDataBinding(RealLightsItem, (x) => ELS.UseRealLights = x, () => ELS.UseRealLights);

            LightOffsetItem = new UIMenuFloatSelector("Light Offset", ELS.LightOffset, "Lighting offset");
            Menu.AddMenuDataBinding(LightOffsetItem, (x) => ELS.LightOffset = x, () => ELS.LightOffset);

            // Headlights

            HeadlightsMenu = new UIMenuRefreshable(Menu.TitleText, "~b~Headlights");
            HeadlightsMenuItem = new UIMenuItem("Headlights", "Modify headlight sequences and multipliers");
            HeadlightsMenuItem.RightLabel = "→";
            Menu.AddItem(HeadlightsMenuItem, 3);
            Menu.BindMenuAndCopyProperties(HeadlightsMenu, HeadlightsMenuItem);

            LeftHeadlightMultiplesItem = new UIMenuListItemSelector<byte>("Front Left Multiples", "Left headlight multiples per flash", ELS.LeftHeadLightMultiples, CommonSelectionItems.MultiplesBytes);
            HeadlightsMenu.AddMenuDataBinding(LeftHeadlightMultiplesItem, (x) => ELS.LeftHeadLightMultiples = x, () => ELS.LeftHeadLightMultiples);

            LeftHeadlightSequenceItem = new UIMenuSequenceItemSelector("Front Left Sequence", ELS.LeftHeadLightSequence, "Left headlight flash pattern sequence");
            LeftHeadlightSequenceItem.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Blank;
            HeadlightsMenu.AddMenuDataBinding(LeftHeadlightSequenceItem, (x) => ELS.LeftHeadLightSequence = x, () => ELS.LeftHeadLightSequence);

            RightHeadlightMultiplesItem = new UIMenuListItemSelector<byte>("Front Right Multiples", "Right headlight multiples per flash", ELS.RightHeadLightMultiples, CommonSelectionItems.MultiplesBytes);
            HeadlightsMenu.AddMenuDataBinding(RightHeadlightMultiplesItem, (x) => ELS.RightHeadLightMultiples = x, () => ELS.RightHeadLightMultiples);

            RightHeadlightSequenceItem = new UIMenuSequenceItemSelector("Front Right Sequence", ELS.RightHeadLightSequence, "Right headlight flash pattern sequence");
            RightHeadlightSequenceItem.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Blank;
            HeadlightsMenu.AddMenuDataBinding(RightHeadlightSequenceItem, (x) => ELS.RightHeadLightSequence = x, () => ELS.RightHeadLightSequence);

            // Taillights

            TaillightsMenu = new UIMenuRefreshable(Menu.TitleText, "~b~Taillights");
            TaillightsMenuItem = new UIMenuItem("Taillights", "Modify Taillight sequences and multipliers");
            TaillightsMenuItem.RightLabel = "→";
            Menu.AddItem(TaillightsMenuItem, 4);
            Menu.BindMenuAndCopyProperties(TaillightsMenu, TaillightsMenuItem);

            LeftTaillightMultiplesItem = new UIMenuListItemSelector<byte>("Left Rear Multiples", "Left Taillight multiples per flash", ELS.LeftTailLightMultiples, CommonSelectionItems.MultiplesBytes);
            TaillightsMenu.AddMenuDataBinding(LeftTaillightMultiplesItem, (x) => ELS.LeftTailLightMultiples = x, () => ELS.LeftTailLightMultiples);

            LeftTaillightSequenceItem = new UIMenuSequenceItemSelector("Left Rear Sequence", ELS.LeftTailLightSequence, "Left Taillight flash pattern sequence");
            LeftTaillightSequenceItem.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Blank;
            TaillightsMenu.AddMenuDataBinding(LeftTaillightSequenceItem, (x) => ELS.LeftTailLightSequence = x, () => ELS.LeftTailLightSequence);

            RightTaillightMultiplesItem = new UIMenuListItemSelector<byte>("Right Rear Multiples", "Right Taillight multiples per flash", ELS.RightTailLightMultiples, CommonSelectionItems.MultiplesBytes);
            TaillightsMenu.AddMenuDataBinding(RightTaillightMultiplesItem, (x) => ELS.RightTailLightMultiples = x, () => ELS.RightTailLightMultiples);

            RightTaillightSequenceItem = new UIMenuSequenceItemSelector("Right Rear Sequence", ELS.RightTailLightSequence, "Right Taillight flash pattern sequence");
            RightTaillightSequenceItem.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Blank;
            TaillightsMenu.AddMenuDataBinding(RightTaillightSequenceItem, (x) => ELS.RightTailLightSequence = x, () => ELS.RightTailLightSequence);

            // Sirens 

            SirensMenuItem = new UIMenuItem("Sirens", "Edit sequences and other settings for individual sirens");
            SirensMenuItem.RightLabel = "→";
            Menu.AddItem(SirensMenuItem, 3);
            SirensMenuItem.Activated += OnSirenSubmenuActivated;
            
            // Create each siren menu
            for (int i = 0; i < Settings.MaxSirens; i++)
            {
                EmergencyLightMenu sirenMenu = new EmergencyLightMenu(ELS, i);
                sirenMenu.Menu.ParentItem = SirensMenuItem;
                sirenMenu.Menu.ParentMenu = Menu;
                Menu.AddSubMenuBinding(sirenMenu.Menu);
                Menu.CopyMenuProperties(sirenMenu.Menu, true);
                MenuController.Pool.AddMenuAndSubMenusToPool(sirenMenu.Menu, true);
                SirenMenus[i] = sirenMenu;
            }

            // Create switcher and add to menus
            // This has to be after the for loop above because all the menus need to be created before the switcher can be created
            SirenSwitcherItem = new UIMenuSwitchSelectable("Siren", "Select the siren to edit", SirenMenus);
            foreach (var sirenMenu in SirenMenus)
            {
                sirenMenu.Menu.AddItem(SirenSwitcherItem, 0);
                sirenMenu.Menu.RefreshIndex();
            }

            // Final stuff

            SequenceQuickEdit = new SequenceQuickEditMenu(ELS, this);
            SequenceQuickEditItem = new UIMenuItem("Siren Sequence Quick Edit", "Edit flashiness sequences for all sirens on this siren setting at once");
            Menu.BindMenuAndCopyProperties(SequenceQuickEdit.Menu, SequenceQuickEditItem);
            SequenceQuickEditItem.Activated += OnQuickEditMenuOpened;
            Menu.AddItem(SequenceQuickEditItem, 4);
            SequenceQuickEditItem.RightLabel = "→";

            SequenceImportItem = new UIMenuItem("Quick Sequence Import", "Import multiple sequences from the clipboard (one per line) to quickly apply them to multiple sirens. ~y~CAUTION!~s~ Immediately overwrites all siren sequences when selected!");
            Menu.AddItem(SequenceImportItem, 5);
            SequenceImportItem.RightLabel = "→";
            SequenceImportItem.Activated += OnSequenceQuickImport;

            RefreshItem = new UIMenuItem("Refresh Siren Setting Data", "Refreshes the menu with the siren setting data for the current vehicle. Use this if the data may have been changed outside the menu.");
            Menu.AddRefreshItem(RefreshItem);

            CopyMenu = new CopyMenu(this);
            CopyMenuItem = new UIMenuItem("Copy", "Copy properties to/from this siren setting");
            CopyMenuItem.RightLabel = "→";
            Menu.BindMenuAndCopyProperties(CopyMenu.Menu, CopyMenuItem);
            Menu.AddItem(CopyMenuItem);
            
            ExportCarcolsItem = new UIMenuItem("Export carcols.meta file", "Exports the single siren setting currently being modified to a carcols.meta file. To export multiple settings into a single file, use the bulk export tool from the main menu.");
            Menu.AddItem(ExportCarcolsItem);
            ExportCarcolsItem.Activated += OnExportClicked;

            MenuController.Pool.AddAfterYield(Menu, HeadlightsMenu, TaillightsMenu, SequenceQuickEdit.Menu, CopyMenu.Menu);

            Menu.RefreshIndex();
        }

        private void OnSequenceQuickImport(UIMenu sender, UIMenuItem selectedItem)
        {
            string clipboard = Game.GetClipboardText();
            if (string.IsNullOrWhiteSpace(clipboard))
            {
                Game.DisplayNotification("~y~Clipboard is empty");
            } else
            {
                int siren = 0;
                foreach (string line in clipboard.Trim().Split('\n'))
                {
                    string clean = string.Concat(line.Trim().Where(c => c == '1' || c == '0').Take(32));
                    if (clean.Length == 32)
                    {
                        SirenMenus[siren].FlashSequenceItem.ItemValue = clean;
                        siren++;
                    }

                    if (siren >= SirenMenus.Length) break;
                }

                Game.DisplayNotification($"Imported sequences for ~b~{siren}~s~ sirens");
            }
        }

        private void OnQuickEditMenuOpened(UIMenu sender, UIMenuItem selectedItem)
        {
            // Sequences may have been changed by other menus, need to refresh before showing
            SequenceQuickEdit.Menu.RefreshData();
            SequenceQuickEdit.Menu.RefreshIndex();
        }

        private void OnExportClicked(UIMenu sender, UIMenuItem selectedItem)
        {
            if(selectedItem == ExportCarcolsItem)
            {
                ImportExportMenu.ExportSelectSettingsMenu.SelectItems(ELS);
                Menu.Visible = false;
                ImportExportMenu.ExportMenu.Visible = true;
            }
        }

        private void OnSirenSubmenuActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            sender.Visible = false;
            SirenSwitcherItem.SwitchMenuItem.CurrentMenu.Visible = true;
        }

        public void ShowSirenInfo(Vehicle v)
        {
            int currentSirenId = SirenSwitcherItem.ItemValue;
            var switcher = SirenSwitcherItem.MenuItem;
            switcher.Description = "Select the siren to edit. Press ~b~ENTER~w~ to type a siren ID.\n";

            if (v)
            {   
                switcher.Description += $"The current vehicle ({v.Model.Name}) ";
                if (v.HasSiren(currentSirenId))
                {
                    switcher.Description += $"~g~has~w~ Siren {currentSirenId}";
                    switcher.RightBadge = UIMenuItem.BadgeStyle.Tick;
                    switcher.RightBadgeInfo.Color = Color.Green;
                }
                else
                {
                    switcher.Description += $"~r~does not have~s~ Siren {currentSirenId}, but other vehicle models using this siren setting might";
                    switcher.RightBadge = UIMenuItem.BadgeStyle.Alert;
                    switcher.RightBadgeInfo.Color = Color.Yellow;
                }
            } else
            {
                switcher.Description += "~c~No vehicle is currently selected";
                switcher.RightBadge = UIMenuItem.BadgeStyle.Alert;
                switcher.RightBadgeInfo.Color = Color.DarkGray;
            }

            var currentMenu = SirenMenus[currentSirenId - 1];
            if (v && currentMenu.Menu.Visible || currentMenu.Menu.Children.Values.Any(c => c.Visible))
            {
                v.ShowSirenMarker(currentSirenId);
            }

            if (SequenceQuickEdit.Menu.Visible)
            {
                for (int i = 0; i < SequenceQuickEdit.SirenSequenceItems.Length - 4; i++)
                {
                    int sirenId = i + 1;
                    var item = SequenceQuickEdit.SirenSequenceItems[i];
                    if (v)
                    {
                        if (item.MenuItem.Selected) v.ShowSirenMarker(i + 1);

                        if (v.HasSiren(sirenId))
                        {
                            item.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Car;
                            item.MenuItem.RightBadgeInfo.Color = Color.DarkGray;
                        } else
                        {
                            item.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Alert;
                            item.MenuItem.RightBadgeInfo.Color = Color.DarkGray;
                        }
                    } else
                    {
                        item.MenuItem.RightBadge = UIMenuItem.BadgeStyle.Blank;
                    }
                }
            }
            
            if (CopyMenu.Menu.Visible) CopyMenu.ProcessShowSirens(v);
        }

        private uint GetSourceID()
        {
            SirenSource source = ELS.GetSource();
            if (source != null) return source.SourceId;
            else return 0;
        }

        private void SetSourceID(uint id) => ELS.SetSource(id, EmergencyLightingSource.Manual);

        public EmergencyLighting ELS { get; }

        // Core lighting settings
        public UIMenuRefreshable Menu { get; }
        public UIMenuStringSelector NameItem { get; }
        public UIMenuUIntSelector IdItem { get; }
        public UIMenuUIntSelector BpmItem { get; }
        public UIMenuListItemSelector<string> TextureHashItem { get; }
        public UIMenuListItemSelector<float> TimeMultiplierItem { get; }
        public UIMenuListItemSelector<float> FalloffMaxItem { get; }
        public UIMenuListItemSelector<float> FalloffExponentItem { get; }
        public UIMenuListItemSelector<float> InnerConeAngleItem { get; }
        public UIMenuListItemSelector<float> OuterConeAngleItem { get; }
        public UIMenuRefreshableCheckboxItem RealLightsItem { get; }
        public UIMenuFloatSelector LightOffsetItem { get; }
        public UIMenuItem RefreshItem { get; }

        // Headlights menu
        public UIMenuItem HeadlightsMenuItem { get; }
        public UIMenuRefreshable HeadlightsMenu { get; }
        public UIMenuListItemSelector<byte> LeftHeadlightMultiplesItem { get; }
        public UIMenuSequenceItemSelector LeftHeadlightSequenceItem { get; }
        public UIMenuListItemSelector<byte> RightHeadlightMultiplesItem { get; }
        public UIMenuSequenceItemSelector RightHeadlightSequenceItem { get; }

        // Taillights menu 
        public UIMenuItem TaillightsMenuItem { get; }
        public UIMenuRefreshable TaillightsMenu { get; }
        public UIMenuListItemSelector<byte> LeftTaillightMultiplesItem { get; }
        public UIMenuSequenceItemSelector LeftTaillightSequenceItem { get; }
        public UIMenuListItemSelector<byte> RightTaillightMultiplesItem { get; }
        public UIMenuSequenceItemSelector RightTaillightSequenceItem { get; }

        // Sirens menu
        public UIMenuItem SirensMenuItem { get; }
        public UIMenuSwitchSelectable SirenSwitcherItem { get; }
        public EmergencyLightMenu[] SirenMenus { get; } = new EmergencyLightMenu[Settings.MaxSirens];

        // Quick edit menu
        public UIMenuItem SequenceQuickEditItem { get; }
        public SequenceQuickEditMenu SequenceQuickEdit { get; }

        // Import
        public UIMenuItem SequenceImportItem { get; }

        // Copy menu
        public CopyMenu CopyMenu { get; }
        public UIMenuItem CopyMenuItem { get; }

        // Import/export
        public UIMenuItem ExportCarcolsItem { get; }
    }
}

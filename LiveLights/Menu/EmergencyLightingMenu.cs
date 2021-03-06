﻿using System;
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

            // Main siren settings

            NameItem = new UIMenuStringSelector("Name", ELS.Name, "Siren setting name as shown in carcols.meta");
            Menu.AddMenuDataBinding(NameItem, (x) => ELS.Name = x, () => ELS.Name);

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
            HeadlightsMenu.AddMenuDataBinding(LeftHeadlightSequenceItem, (x) => ELS.LeftHeadLightSequence = x, () => ELS.LeftHeadLightSequence);

            RightHeadlightMultiplesItem = new UIMenuListItemSelector<byte>("Front Right Multiples", "Right headlight multiples per flash", ELS.RightHeadLightMultiples, CommonSelectionItems.MultiplesBytes);
            HeadlightsMenu.AddMenuDataBinding(RightHeadlightMultiplesItem, (x) => ELS.RightHeadLightMultiples = x, () => ELS.RightHeadLightMultiples);

            RightHeadlightSequenceItem = new UIMenuSequenceItemSelector("Front Right Sequence", ELS.RightHeadLightSequence, "Right headlight flash pattern sequence");
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
            TaillightsMenu.AddMenuDataBinding(LeftTaillightSequenceItem, (x) => ELS.LeftTailLightSequence = x, () => ELS.LeftTailLightSequence);

            RightTaillightMultiplesItem = new UIMenuListItemSelector<byte>("Right Rear Multiples", "Right Taillight multiples per flash", ELS.RightTailLightMultiples, CommonSelectionItems.MultiplesBytes);
            TaillightsMenu.AddMenuDataBinding(RightTaillightMultiplesItem, (x) => ELS.RightTailLightMultiples = x, () => ELS.RightTailLightMultiples);

            RightTaillightSequenceItem = new UIMenuSequenceItemSelector("Right Rear Sequence", ELS.RightTailLightSequence, "Right Taillight flash pattern sequence");
            TaillightsMenu.AddMenuDataBinding(RightTaillightSequenceItem, (x) => ELS.RightTailLightSequence = x, () => ELS.RightTailLightSequence);

            // Sirens 

            SirensMenuItem = new UIMenuItem("Sirens", "Edit sequences and other settings for individual sirens");
            SirensMenuItem.RightLabel = "→";
            Menu.AddItem(SirensMenuItem, 3);
            SirensMenuItem.Activated += onSirenSubmenuActivated;
            SirenMenus = new List<EmergencyLightMenu>();

            // Create each siren menu
            for (int i = 0; i < 20; i++)
            {
                EmergencyLightMenu sirenMenu = new EmergencyLightMenu(ELS, i);
                sirenMenu.Menu.ParentItem = SirensMenuItem;
                sirenMenu.Menu.ParentMenu = Menu;
                Menu.AddSubMenuBinding(sirenMenu.Menu);
                Menu.CopyMenuProperties(sirenMenu.Menu, true);
                MenuController.Pool.AddMenuAndSubMenusToPool(sirenMenu.Menu, true);
                SirenMenus.Add(sirenMenu);
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
            
            RefreshItem = new UIMenuItem("Refresh Siren Setting Data", "Refreshes the menu with the siren setting data for the current vehicle. Use this if the data may have been changed outside the menu.");
            Menu.AddRefreshItem(RefreshItem);

            CopyMenu = new CopyMenu(this);
            CopyMenuItem = new UIMenuItem("Copy", "Copy properties to/from this siren setting");
            CopyMenuItem.RightLabel = "→";
            Menu.BindMenuAndCopyProperties(CopyMenu.Menu, CopyMenuItem);
            Menu.AddItem(CopyMenuItem);

            /*
            ImportCarcolsItem = new UIMenuItem("Import carcols.meta file", "Imports all siren settings in selected carcols.meta file");
            Menu.AddItem(ImportCarcolsItem);
            ImportCarcolsItem.Activated += OnImportExportClicked;
            */

            ExportCarcolsItem = new UIMenuItem("Export carcols.meta file", "Exports the siren setting currently being modified to a carcols.meta file");
            Menu.AddItem(ExportCarcolsItem);
            ExportCarcolsItem.Activated += OnImportExportClicked;

            ExportAllowOverwriteItem = new UIMenuCheckboxItem("Allow overwrite on export", Settings.DefaultOverwrite, "Allow exported carcols.meta files to overwrite existing files with the same name");
            Menu.AddItem(ExportAllowOverwriteItem);

            MenuController.Pool.AddAfterYield(Menu, HeadlightsMenu, TaillightsMenu, SequenceQuickEdit.Menu, CopyMenu.Menu);

            Menu.RefreshIndex();
        }

        private void OnQuickEditMenuOpened(UIMenu sender, UIMenuItem selectedItem)
        {
            // Sequences may have been changed by other menus, need to refresh before showing
            SequenceQuickEdit.Menu.RefreshData();
            SequenceQuickEdit.Menu.RefreshIndex();
        }

        private void OnImportExportClicked(UIMenu sender, UIMenuItem selectedItem)
        {
            if(selectedItem == ImportCarcolsItem)
            {
                ImportExportMenu.OnImportCarcols(this);
            } else if(selectedItem == ExportCarcolsItem)
            {
                ImportExportMenu.ExportCarcols(this.ELS, ExportAllowOverwriteItem.Checked);
            }
        }

        private void onSirenSubmenuActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            sender.Visible = false;
            SirenSwitcherItem.SwitchMenuItem.CurrentMenu.Visible = true;
        }

        public void ShowSirenPositions(Vehicle v, bool selectedOnly)
        {
            if (!v) return;

            foreach (EmergencyLightMenu sirenMenu in SirenSubMenus)
            {
                if (!selectedOnly || sirenMenu.Menu.Visible || sirenMenu.Menu.Children.Values.Any(c => c.Visible))
                {
                    v.ShowSirenMarker(sirenMenu.SirenID);
                }
            }

            for (int i = 0; i < SequenceQuickEdit.SirenSequenceItems.Length; i++)
            {
                int sirenId = i + 1;
                UIMenuStringSelector item = SequenceQuickEdit.SirenSequenceItems[i];
                if(SequenceQuickEdit.Menu.Visible && item.MenuItem.Selected)
                {
                    v.ShowSirenMarker(i+1);
                }
            }

            CopyMenu.ProcessShowSirens(v);
        }

        public EmergencyLighting ELS { get; }

        // Core lighting settings
        public UIMenuRefreshable Menu { get; }
        public UIMenuStringSelector NameItem { get; }
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
        private List<EmergencyLightMenu> SirenMenus { get; }
        public EmergencyLightMenu[] SirenSubMenus => SirenMenus.ToArray();

        // Quick edit menu
        public UIMenuItem SequenceQuickEditItem { get; }
        public SequenceQuickEditMenu SequenceQuickEdit { get; }

        // Copy menu
        public CopyMenu CopyMenu { get; }
        public UIMenuItem CopyMenuItem { get; }

        // Import/export
        public UIMenuItem ExportCarcolsItem { get; }
        public UIMenuCheckboxItem ExportAllowOverwriteItem { get; }
        public UIMenuItem ImportCarcolsItem { get; }
    }
}

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

            Menu = new UIMenuRefreshable("Emergency Lighting Settings", "");
            Menu.SetMenuWidthOffset(250);
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            // Main siren settings

            NameItem = new UIMenuStringSelector("Name", ELS.Name, "Siren setting name as shown in carcols.meta");
            Menu.AddMenuDataBinding(NameItem, (x) => ELS.Name = x, () => ELS.Name);

            BpmItem = new UIMenuUIntSelector("BPM", ELS.SequencerBpm, "Beats per minute");
            Menu.AddMenuDataBinding(BpmItem, (x) => ELS.SequencerBpm = x, () => ELS.SequencerBpm);

            TimeMultiplierItem = new UIMenuListItemSelector<float>("Time Multiplier", "Adjusts how fast BPM is scaled", ELS.TimeMultiplier, 0.1f, 0.25f, 0.5f, 0.75f, 0.9f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 10f);
            Menu.AddMenuDataBinding(TimeMultiplierItem, (x) => ELS.TimeMultiplier = x, () => ELS.TimeMultiplier);

            TextureHashItem = new UIMenuListItemSelector<string>("Texture Hash", "Texture which is shown for environmental lighting from this vehicle", TextureHash.HashToString(ELS.TextureHash), TextureHash.lightTextureHashes.Values);
            TextureHashItem.ListMenuItem.SetAddNewItems((x) => x);
            Menu.AddMenuDataBinding(TextureHashItem, (x) => ELS.TextureHash = Utils.TextureHash.StringToHash(x), () => Utils.TextureHash.HashToString(ELS.TextureHash));

            FalloffMaxItem = new UIMenuListItemSelector<float>("Falloff Max", "Affects how far environmental lighting shines", ELS.LightFalloffMax, 8, 16, 32, 75, 100, 150, 200, 250, 300);
            Menu.AddMenuDataBinding(FalloffMaxItem, (x) => ELS.LightFalloffMax = x, () => ELS.LightFalloffMax);

            FalloffExponentItem = new UIMenuListItemSelector<float>("Falloff Exponent", "Affects how far environmental lighting shines", ELS.LightFalloffMax, 8, 16, 32, 75, 100, 150, 200, 250, 300);
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

            HeadlightsMenu = new UIMenuRefreshable(Menu.Title.Caption, "~b~Headlights");
            HeadlightsMenuItem = new UIMenuItem("Headlights", "Modify headlight sequences and multipliers");
            Menu.AddItem(HeadlightsMenuItem);
            Menu.BindMenuAndCopyProperties(HeadlightsMenu, HeadlightsMenuItem);

            LeftHeadlightMultiplesItem = new UIMenuListItemSelector<byte>("Front Left Multiples", "Left headlight multiples per flash", ELS.LeftHeadLightMultiples, Enumerable.Range(1, 4).Select(x => (byte)x));
            HeadlightsMenu.AddMenuDataBinding(LeftHeadlightMultiplesItem, (x) => ELS.LeftHeadLightMultiples = x, () => ELS.LeftHeadLightMultiples);

            LeftHeadlightSequenceItem = new UIMenuStringSelector("Front Left Sequence", ELS.LeftHeadLightSequence, "Left headlight flash pattern sequence") { MaxLength = 32 };
            HeadlightsMenu.AddMenuDataBinding(LeftHeadlightSequenceItem, (x) => ELS.LeftHeadLightSequence = x, () => ELS.LeftHeadLightSequence);

            RightHeadlightMultiplesItem = new UIMenuListItemSelector<byte>("Front Right Multiples", "Right headlight multiples per flash", ELS.RightHeadLightMultiples, Enumerable.Range(1, 4).Select(x => (byte)x));
            HeadlightsMenu.AddMenuDataBinding(RightHeadlightMultiplesItem, (x) => ELS.RightHeadLightMultiples = x, () => ELS.RightHeadLightMultiples);

            RightHeadlightSequenceItem = new UIMenuStringSelector("Front Right Sequence", ELS.RightHeadLightSequence, "Right headlight flash pattern sequence") { MaxLength = 32 };
            HeadlightsMenu.AddMenuDataBinding(RightHeadlightSequenceItem, (x) => ELS.RightHeadLightSequence = x, () => ELS.RightHeadLightSequence);

            // Final stuff

            RefreshItem = new UIMenuItem("Refresh Siren Setting Data", "Refreshes the menu with the siren setting data for the current vehicle. Use this if the data may have been changed outside the menu.");
            Menu.AddRefreshItem(RefreshItem);

            MenuController.Pool.Add(Menu);
            MenuController.Pool.Add(HeadlightsMenu);

            Menu.RefreshIndex();
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
        public UIMenuStringSelector LeftHeadlightSequenceItem { get; }
        public UIMenuListItemSelector<byte> RightHeadlightMultiplesItem { get; }
        public UIMenuStringSelector RightHeadlightSequenceItem { get; }


    }
}
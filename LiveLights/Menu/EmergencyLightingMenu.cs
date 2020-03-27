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
            Menu.SetMenuWidthOffset(250);
            Menu.ControlDisablingEnabled = true;
            Menu.MouseControlsEnabled = false;
            Menu.AllowCameraMovement = true;

            NameItem = new UIMenuStringSelector("Name", ELS.Name, "Siren setting name as shown in carcols.meta");
            // Menu.AddItem(NameItem);
            // NameItem.OnValueChanged += (string name) => ELS.Name = name;
            AddMenuDataBinding(NameItem, (x) => ELS.Name = x, () => ELS.Name);

            BpmItem = new UIMenuUIntSelector("BPM", ELS.SequencerBpm, "Beats per minute");
            AddMenuDataBinding(BpmItem, (x) => ELS.SequencerBpm = x, () => ELS.SequencerBpm);

            TextureHashItem = new UIMenuStringSelector("Texture Hash", Utils.TextureHash.HashToString(ELS.TextureHash));
            Menu.AddItem(TextureHashItem);

            LeftHeadlightMultiplesItem = new UIMenuListItemSelector<byte>(new UIMenuCustomListItem<byte>("Front Left Multiples", "Left headlight multiples per flash", Enumerable.Range(1, 4).Select(x => (byte)x).ToArray()), els.LeftHeadLightMultiples);
            AddMenuDataBinding(LeftHeadlightMultiplesItem, (x) => ELS.LeftHeadLightMultiples = x, () => ELS.LeftHeadLightMultiples);

            LeftHeadlightSequenceItem = new UIMenuStringSelector("Front Left Sequence", ELS.LeftHeadLightSequence, "Left headlight flash pattern sequence") { MaxLength = 32 };
            AddMenuDataBinding(LeftHeadlightSequenceItem, (x) => ELS.LeftHeadLightSequence = x, () => ELS.LeftHeadLightSequence);

            RightHeadlightMultiplesItem = new UIMenuListItemSelector<byte>(new UIMenuCustomListItem<byte>("Front Right Multiples", "Right headlight multiples per flash", Enumerable.Range(1, 4).Select(x => (byte)x).ToArray()), els.RightHeadLightMultiples);
            AddMenuDataBinding(RightHeadlightMultiplesItem, (x) => ELS.RightHeadLightMultiples = x, () => ELS.RightHeadLightMultiples);

            RightHeadlightSequenceItem = new UIMenuStringSelector("Front Right Sequence", ELS.RightHeadLightSequence, "Right headlight flash pattern sequence") { MaxLength = 32 };
            AddMenuDataBinding(RightHeadlightSequenceItem, (x) => ELS.RightHeadLightSequence = x, () => ELS.RightHeadLightSequence);


            MenuController.Pool.Add(Menu);
        }

        private void AddMenuDataBinding<TMenuItem, TData>(TMenuItem menuItem, Action<TData> menuBinding, Func<TData> dataBinding) where TMenuItem : UIMenuValueEntrySelector<TData>, IRefreshableItemWrapper where TData : IEquatable<TData>
        {
            menuItem.SetBindings(menuBinding, dataBinding);
            Menu.AddItem(menuItem.MenuItem);
            bindings.Add(menuItem);
        }

        public EmergencyLighting ELS { get; }

        private List<IRefreshableItemWrapper> bindings = new List<IRefreshableItemWrapper>();

        public UIMenu Menu { get; } 
        public UIMenuStringSelector NameItem { get; } 
        public UIMenuUIntSelector BpmItem { get; }
        public UIMenuStringSelector TextureHashItem { get; }

        public UIMenuListItemSelector<byte> LeftHeadlightMultiplesItem { get; }
        public UIMenuStringSelector LeftHeadlightSequenceItem { get; }

        public UIMenuListItemSelector<byte> RightHeadlightMultiplesItem { get; }
        public UIMenuStringSelector RightHeadlightSequenceItem { get; }

    }
}

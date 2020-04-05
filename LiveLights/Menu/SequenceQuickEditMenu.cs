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

    internal class SequenceQuickEditMenu
    {
        public SequenceQuickEditMenu(EmergencyLighting els, EmergencyLightingMenu parent)
        {
            this.ELS = els;
            this.Parent = parent;

            Menu = new UIMenuRefreshable("Edit Sequences", parent.Menu.Subtitle.Caption + " > Sequence Quick Edit");

            for (int i = 0; i < ELS.Lights.Length; i++)
            {
                EmergencyLight siren = ELS.Lights[i];
                string sirenId = $"Siren {i + 1}";
                UIMenuStringSelector item = new UIMenuStringSelector($"{sirenId} Sequence", siren.FlashinessSequence, $"Edit 32-bit sequence for {sirenId}") { MaxLength = 32 };
                Menu.AddMenuDataBinding(item, (x) => siren.FlashinessSequence = x, () => siren.FlashinessSequence);
                SirenSequenceItems.Add(item);
            }

            Menu.AddItem(Parent.LeftHeadlightSequenceItem);
            Menu.AddItem(Parent.RightHeadlightSequenceItem);
            Menu.AddItem(Parent.LeftTaillightSequenceItem);
            Menu.AddItem(Parent.RightTaillightSequenceItem);

            Menu.OnMenuClose += OnMenuClose;
        }

        private void OnMenuClose(UIMenu sender)
        {
            // This menu changes sequences, so we need to refresh all other 
            // sub-menus where those sequences may appear
            Parent.Menu.RefreshData();
        }

        public EmergencyLightingMenu Parent { get; }
        public EmergencyLighting ELS { get; }
        public UIMenuRefreshable Menu { get; }

        private List<UIMenuItem> SirenSequenceItems = new List<UIMenuItem>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LiveLights.Menu
{
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal class SirenIdMultiselectMenu
    {
        public SirenIdMultiselectMenu(string desc = "Select {siren}")
        {
            Menu.AddItem(SelectAllItem);

            for (int i = 0; i < 20; i++)
            {
                string sirenId = $"Siren {i + 1}";
                UIMenuCheckboxItem checkbox = new UIMenuCheckboxItem(sirenId, false, desc.Replace("{siren}", sirenId));
                checkboxes[i] = checkbox;
                Menu.AddItem(checkbox);
            }

            Menu.RefreshIndex();
            MenuController.Pool.AddAfterYield(Menu);
            Menu.OnCheckboxChange += OnCheckboxChanged;
        }

        private void OnCheckboxChanged(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked)
        {
            if(checkboxItem == SelectAllItem)
            {
                foreach (UIMenuCheckboxItem checkbox in checkboxes)
                {
                    checkbox.Checked = Checked;
                }
            } else
            {
                SelectAllItem.Checked = false;
            }

            if(Menu.ParentItem != null)
            {
                string label = "";
                int[] sirens = SelectedSirenIDs.ToArray();
                if(sirens.Length == 0)
                {
                    label = "None selected";
                } else if (sirens.Length == 20)
                {
                    label = "All";
                } else
                {
                    label = string.Join(", ", sirens);
                }
                label += " →";
                Menu.ParentItem.SetRightLabel(label);
            }
        }

        public UIMenu Menu { get; } = new UIMenu("Select Siren IDs", "~b~");
        public UIMenuCheckboxItem SelectAllItem { get; } = new UIMenuCheckboxItem("Select All", false, "Select or deselect all siren IDs");
        // 0-indexed array
        private UIMenuCheckboxItem[] checkboxes = new UIMenuCheckboxItem[20];
        public IEnumerable<UIMenuCheckboxItem> Checkboxes => checkboxes;

        // Uses 1-indexed siren ID
        public IEnumerable<int> SelectedSirenIDs
        {
            get
            {
                List<int> selected = new List<int>();
                for (int i = 0; i < checkboxes.Length; i++)
                {
                    if(checkboxes[i].Checked)
                    {
                        selected.Add(i + 1);
                    }
                }
                return selected;
            }

            set
            {
                for (int i = 0; i < checkboxes.Length; i++)
                {
                    checkboxes[i].Checked = value.Contains(i + 1);
                }
            }
        }
        
    }
}

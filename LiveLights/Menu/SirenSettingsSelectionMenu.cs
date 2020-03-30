using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Menu
{
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal class SirenSettingsSelectionMenu
    {
        public UIMenu Menu { get; }
        private Dictionary<EmergencyLighting, SirenSettingMenuItem> elsEntries = new Dictionary<EmergencyLighting, SirenSettingMenuItem>();

        public bool CloseOnSelection { get; }
        public bool IncludeBuiltInSettings { get; }
        public bool IncludeCustomSettings { get; }

        public delegate void SirenSettingSelectedEvent(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting);
        public event SirenSettingSelectedEvent OnSirenSettingSelected;

        public SirenSettingsSelectionMenu(EmergencyLighting initialSelected, bool closeOnSelect = true, bool builtIn = true, bool custom = true)
        {
            this.CloseOnSelection = closeOnSelect;
            this.IncludeBuiltInSettings = builtIn;
            this.IncludeCustomSettings = custom;

            Menu = new UIMenu("Siren Selection", "");
            MenuController.Pool.Add(Menu);
            RefreshSirenSettingList();
            Menu.OnItemSelect += OnMenuItemSelected;

            SelectedEmergencyLighting = initialSelected;
        }

        public UIMenuItem CreateAndBindToSubmenuItem(UIMenu parentMenu) => CreateAndBindToSubmenuItem(parentMenu, "Select Siren Setting", "");

        public UIMenuItem CreateAndBindToSubmenuItem(UIMenu parentMenu, string text, string description, bool addItem = true)
        {
            UIMenuItem item = new UIMenuItem(text, description);
            if(addItem)
            {
                parentMenu.AddItem(item);
            }
            parentMenu.BindMenuAndCopyProperties(Menu, item, false);

            this.OnSirenSettingSelected += OnBoundMenuUpdated;
            item.Activated += OnBoundMenuItemActivated;
            return item;
        }

        private void OnBoundMenuItemActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            this.RefreshSirenSettingList();
        }

        private void OnBoundMenuUpdated(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting)
        {
            menu.ParentItem.SetRightLabel(setting.Name);
        }

        private void OnMenuItemSelected(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            SirenSettingMenuItem selectedSetting = selectedItem as SirenSettingMenuItem;
            if(selectedSetting != null)
            {
                SetSelectedSetting(selectedSetting);
                if (CloseOnSelection)
                {
                    Menu.GoBack();
                }
            }
        }

        private void SetSelectedSetting(SirenSettingMenuItem item)
        {
            if(item != selectedSetting?.Item2)
            {
                selectedSetting?.Item2.SetRightBadge(UIMenuItem.BadgeStyle.None);
                if(item == null)
                {
                    selectedSetting = null;
                } else
                {
                    selectedSetting = Tuple.Create(item.ELS, item);
                    item.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                }
                OnSirenSettingSelected?.Invoke(this, Menu, item, item?.ELS);
            }
        }

        private Tuple<EmergencyLighting, SirenSettingMenuItem> selectedSetting = null;
        public EmergencyLighting SelectedEmergencyLighting 
        {
            get
            {
                // return (Menu.MenuItems[Menu.CurrentSelection] as SirenSettingEntry)?.ELS;
                return selectedSetting?.Item1;
            }

            set
            {
                SetSelectedSetting(elsEntries[value]);
                // elsEntries[value].Selected = true;
                // Menu.CurrentSelection = Menu.MenuItems.IndexOf(elsEntries[value]);
            }
        }

        public void RefreshSirenSettingList()
        {
            EmergencyLighting[] elsToShow = EmergencyLighting.Get(IncludeBuiltInSettings, IncludeCustomSettings);

            // Remove any lighting entries which are no longer valid
            foreach (EmergencyLighting els in elsEntries.Keys.ToArray())
            {
                if(!els.IsValid() || !elsToShow.Contains(els))
                {
                    if(elsEntries.ContainsKey(els))
                    {
                        Menu.RemoveItemAt(Menu.MenuItems.IndexOf(elsEntries[els]));
                        if(SelectedEmergencyLighting == els)
                        {
                            SetSelectedSetting(null);
                        }
                    }
                    elsEntries.Remove(els);
                    Game.LogTrivialDebug("Removed EmergencyLighting entry " + (els.IsValid() ? " of undesired type" : "for being invalid"));
                }
            }

            // Add any new lighting entries
            foreach (EmergencyLighting els in elsToShow)
            {
                if(!elsEntries.ContainsKey(els))
                {
                    SirenSettingMenuItem newMenuEntry = new SirenSettingMenuItem(els);
                    elsEntries.Add(els, newMenuEntry);
                    Menu.AddItem(newMenuEntry);
                    Game.LogTrivialDebug("Added EmergencyLighting entry " + els.Name);
                }
            }

            Menu.RefreshIndex();
        }

        internal class SirenSettingMenuItem : UIMenuItem
        {
            public EmergencyLighting ELS { get; }

            public SirenSettingMenuItem(EmergencyLighting els) : base(els.Name)
            {
                this.ELS = els;
            }
        }
    }
}

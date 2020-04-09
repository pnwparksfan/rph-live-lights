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

    internal class SirenSettingsSelectionMenu
    {
        public UIMenu Menu { get; }
        private Dictionary<EmergencyLightingWrapper, SirenSettingMenuItem> elsEntries = new Dictionary<EmergencyLightingWrapper, SirenSettingMenuItem>();

        public bool CloseOnSelection { get; set; }

        private bool includeBuiltIn;
        public bool IncludeBuiltInSettings
        {
            set
            {
                includeBuiltIn = value;
                RefreshSirenSettingList(true);
            }

            get => includeBuiltIn;
        }

        private bool includeCustom;
        public bool IncludeCustomSettings
        {
            set
            {
                includeCustom = value;
                RefreshSirenSettingList(true);
            }

            get => includeCustom;
        }

        private bool alwaysReturnEditable;
        public bool AlwaysReturnEditableSetting
        {
            set
            {
                alwaysReturnEditable = value;
                RefreshSirenSettingList(true);
            }

            get => alwaysReturnEditable;
        }

        public delegate void SirenSettingSelectedEvent(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting);
        public event SirenSettingSelectedEvent OnSirenSettingSelected;

        public SirenSettingsSelectionMenu(EmergencyLighting initialSelected, bool closeOnSelect = true, bool builtIn = true, bool custom = true, bool returnEditable = true)
        {
            this.CloseOnSelection = closeOnSelect;
            this.includeBuiltIn = builtIn;
            this.includeCustom = custom;
            this.alwaysReturnEditable = (returnEditable && custom);
            if(returnEditable && !custom)
            {
                Game.LogTrivialDebug("Warning: Attempted to create siren setting selection menu without custom entries but with editable required");
            }

            Menu = new UIMenu("Siren Selection", "~b~Select a siren setting to use");
            MenuController.Pool.AddAfterYield(Menu);
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
            UpdateBoundMenuLabel(item);

            this.OnSirenSettingSelected += OnBoundMenuUpdated;
            item.Activated += OnBoundMenuItemActivated;
            return item;
        }

        public void UpdateBoundMenuLabel(UIMenuItem item)
        {
            if(selectedSetting?.Item1?.ELS?.Exists() == true)
            {
                item.SetRightLabel(selectedSetting.Item1.ELS.Name + " →");
            } else
            {
                item.SetRightLabel("~c~none~w~");
            }
        }

        private void OnBoundMenuItemActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            this.RefreshSirenSettingList();
        }

        private void OnBoundMenuUpdated(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting)
        {
            UpdateBoundMenuLabel(menu.ParentItem);
            // menu.ParentItem.SetRightLabel(setting.Name);
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
                if(selectedSetting?.Item2 != null)
                {
                    selectedSetting.Item2.SetRightBadge(UIMenuItem.BadgeStyle.None);
                    selectedSetting.Item2.BackColor = Color.Empty;
                }
                
                if(item == null)
                {
                    selectedSetting = null;
                } else
                {
                    EmergencyLighting selectedEls = item.ELSWrapper.ELS;
                    if(AlwaysReturnEditableSetting && !selectedEls.IsCustomSetting())
                    {
                        selectedEls = selectedEls.Clone();
                        RefreshSirenSettingList();
                        item = elsEntries[selectedEls];
                    }
                    selectedSetting = Tuple.Create(item.ELSWrapper, item);
                    item.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
                    item.BackColor = Color.DarkGray;
                }
                OnSirenSettingSelected?.Invoke(this, Menu, item, item?.ELSWrapper?.ELS);
                SelectedSelectedItem();
            }
        }

        private Tuple<EmergencyLightingWrapper, SirenSettingMenuItem> selectedSetting = null;
        public EmergencyLightingWrapper SelectedEmergencyLighting 
        {
            get
            {
                // return (Menu.MenuItems[Menu.CurrentSelection] as SirenSettingEntry)?.ELS;
                return selectedSetting?.Item1;
            }

            set
            {
                SirenSettingMenuItem item = null;
                if(value != null)
                {
                    elsEntries.TryGetValue(value, out item);
                }
                SetSelectedSetting(item);
                // elsEntries[value].Selected = true;
                // Menu.CurrentSelection = Menu.MenuItems.IndexOf(elsEntries[value]);
            }
        }

        public void RefreshSirenSettingList(bool forceUpdateAll = false)
        {
            // EmergencyLighting[] elsToShow = EmergencyLighting.Get(IncludeBuiltInSettings, IncludeCustomSettings);
            EmergencyLightingWrapper[] elsToShow = EmergencyLighting.Get(IncludeBuiltInSettings, IncludeCustomSettings).Select(l => new EmergencyLightingWrapper(l)).ToArray();

            // Remove any lighting entries which are no longer valid
            foreach (EmergencyLightingWrapper els in elsEntries.Keys.ToArray())
            {
                if(!els.ELS.IsValid() || !elsToShow.Contains(els))
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
                    Game.LogTrivialDebug("Removed EmergencyLighting entry " + (els.ELS.IsValid() ? " of undesired type" : "for being invalid"));
                }
            }

            // Add any new lighting entries
            foreach (EmergencyLightingWrapper els in elsToShow)
            {
                if(forceUpdateAll || !elsEntries.ContainsKey(els))
                {
                    bool isCustom = els.ELS.IsCustomSetting();
                    if(!elsEntries.TryGetValue(els, out SirenSettingMenuItem menuEntry))
                    {
                        menuEntry = new SirenSettingMenuItem(els);
                        elsEntries.Add(els, menuEntry);
                        Menu.AddItem(menuEntry);
                    }
                    
                    if(isCustom)
                    {
                        menuEntry.SetLeftBadge(UIMenuItem.BadgeStyle.Car);
                        menuEntry.Description = "~g~Editable~w~ siren setting entry";
                    } else
                    {
                        menuEntry.SetLeftBadge(UIMenuItem.BadgeStyle.Lock);
                        menuEntry.Description = "~y~Built-in~w~ siren setting entry";
                        if(AlwaysReturnEditableSetting)
                        {
                            menuEntry.Description += ". An ~g~editable~w~ copy will be created if you select this setting.";
                        }
                    }
                    
                    Game.LogTrivialDebug("Added EmergencyLighting entry " + els.ELS.Name);
                }
            }

            SelectedSelectedItem();
        }

        internal class SirenSettingMenuItem : UIMenuItem
        {
            public EmergencyLightingWrapper ELSWrapper { get; }

            public SirenSettingMenuItem(EmergencyLightingWrapper els) : base(els.ELS.Name)
            {
                this.ELSWrapper = els;
            }
        }

        private void SelectedSelectedItem()
        {
            Menu.RefreshIndex();
            if(selectedSetting != null)
            {
                Menu.CurrentSelection = Menu.MenuItems.IndexOf(selectedSetting.Item2);
            }
        }
    }
}

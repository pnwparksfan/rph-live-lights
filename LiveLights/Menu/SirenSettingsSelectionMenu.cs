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
        private Dictionary<EmergencyLighting, SirenSettingMenuItem> elsEntries = new Dictionary<EmergencyLighting, SirenSettingMenuItem>();

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
            if(selectedSetting?.Item1?.Exists() == true)
            {
                item.RightLabel = selectedSetting.Item1.Name + " →";
            } else
            {
                item.RightLabel = "~c~none~w~";
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
                    selectedSetting.Item2.RightBadge = UIMenuItem.BadgeStyle.None;
                    selectedSetting.Item2.BackColor = Color.Empty;
                }
                
                if(item == null)
                {
                    selectedSetting = null;
                } else
                {
                    EmergencyLighting selectedEls = item.ELS;
                    if(AlwaysReturnEditableSetting && !selectedEls.IsCustomSetting())
                    {
                        selectedEls = selectedEls.Clone();
                        RefreshSirenSettingList();
                        item = elsEntries[selectedEls];
                    }
                    selectedSetting = Tuple.Create(item.ELS, item);
                    item.RightBadge = UIMenuItem.BadgeStyle.Tick;
                    item.BackColor = Color.DarkGray;
                }
                OnSirenSettingSelected?.Invoke(this, Menu, item, item?.ELS);
                SelectedSelectedItem();
            }
        }

        private Tuple<EmergencyLighting, SirenSettingMenuItem> selectedSetting = null;
        public EmergencyLighting SelectedEmergencyLighting 
        {
            get
            {
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
            }
        }

        public void RefreshSirenSettingList(bool forceUpdateAll = false)
        {
            IEnumerable<EmergencyLighting> elsToShow = EmergencyLighting.Get(IncludeBuiltInSettings, IncludeCustomSettings).Where(e => e.Exists());

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
                } else
                {
                    elsEntries[els].Text = els.Name;
                }
            }

            // Add any new lighting entries
            foreach (EmergencyLighting els in elsToShow)
            {
                if(forceUpdateAll || !elsEntries.ContainsKey(els))
                {
                    bool isCustom = els.IsCustomSetting();
                    if(!elsEntries.TryGetValue(els, out SirenSettingMenuItem menuEntry))
                    {
                        menuEntry = new SirenSettingMenuItem(els);
                        elsEntries.Add(els, menuEntry);
                        Menu.AddItem(menuEntry);
                    }
                    
                    if(isCustom)
                    {
                        menuEntry.LeftBadge = UIMenuItem.BadgeStyle.Car;
                        menuEntry.Description = "~g~Editable~w~ siren setting entry";
                    } else
                    {
                        menuEntry.LeftBadge = UIMenuItem.BadgeStyle.Lock;
                        menuEntry.Description = "~y~Built-in~w~ siren setting entry";
                        if(AlwaysReturnEditableSetting)
                        {
                            menuEntry.Description += ". An ~g~editable~w~ copy will be created if you select this setting.";
                        }
                    }
                    
                    Game.LogTrivialDebug("Added EmergencyLighting entry " + els.Name);
                }
            }

            SelectedSelectedItem();
        }

        internal class SirenSettingMenuItem : UIMenuItem
        {
            public EmergencyLighting ELS { get; }

            public SirenSettingMenuItem(EmergencyLighting els) : base(els.Name)
            {
                this.ELS = els;
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

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

    internal interface ISirenSettingMenuItem
    {
        EmergencyLighting ELS { get; }
    }

    internal class SirenSettingMenuItem : UIMenuItem, ISirenSettingMenuItem
    {
        public EmergencyLighting ELS { get; }

        public SirenSettingMenuItem(EmergencyLighting els) : base(els.Name)
        {
            this.ELS = els;
            this.RightBadge = BadgeStyle.Blank;
        }
    }

    internal class MultiSirenSettingMenuItem : UIMenuCheckboxItem, ISirenSettingMenuItem
    {
        public EmergencyLighting ELS { get; }

        public MultiSirenSettingMenuItem(EmergencyLighting els) : base(els.Name, false)
        {
            this.ELS = els;
        }
    }

    internal abstract class BaseSirenSettingsSelectionMenu<T> where T: UIMenuItem, ISirenSettingMenuItem
    {
        public UIMenu Menu { get; }
        protected Dictionary<EmergencyLighting, T> elsEntries = new Dictionary<EmergencyLighting, T>();
        
        protected IEnumerable<EmergencyLighting> customEntries;
        public IEnumerable<EmergencyLighting> CustomEntries
        {
            get => customEntries;
            set
            {
                customEntries = value;
                RefreshSirenSettingList(true);
            }
        }

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

        public BaseSirenSettingsSelectionMenu(bool builtIn = true, bool custom = true, bool returnEditable = true, IEnumerable<EmergencyLighting> customList = null)
        {
            this.includeBuiltIn = builtIn;
            this.includeCustom = custom;
            this.alwaysReturnEditable = (returnEditable && custom);
            this.customEntries = customList;

            if(returnEditable && !custom)
            {
                Game.LogTrivialDebug("Warning: Attempted to create siren setting selection menu without custom entries but with editable required");
            }

            Menu = new UIMenu("Siren Selection", "~b~Select a siren setting to use");
            MenuController.Pool.AddAfterYield(Menu);
            RefreshSirenSettingList();
        }

        public UIMenuItem CreateAndBindToSubmenuItem(UIMenu parentMenu) => CreateAndBindToSubmenuItem(parentMenu, "Select Siren Setting", "");

        public UIMenuItem CreateAndBindToSubmenuItem(UIMenu parentMenu, string text, string description, bool addItem = true)
        {
            UIMenuItem item = new UIMenuItem(text, description);
            if (addItem)
            {
                parentMenu.AddItem(item);
            }
            parentMenu.BindMenuAndCopyProperties(Menu, item, false);
            UpdateBoundMenuLabel(item);

            item.Activated += OnBoundMenuItemActivated;
            return item;
        }

        public abstract void UpdateBoundMenuLabel(UIMenuItem item);

        protected void OnBoundMenuItemActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            this.RefreshSirenSettingList();
        }

        protected abstract T CreateNewSirenSettingMenuItem(EmergencyLighting els);

        public virtual void RefreshSirenSettingList(bool forceUpdateAll = false)
        {
            IEnumerable<EmergencyLighting> elsToShow;
            if (customEntries != null)
            {
                elsToShow = customEntries.Where(e => e.Exists() && ((IncludeBuiltInSettings && !e.IsCustomSetting()) || (IncludeCustomSettings && e.IsCustomSetting())));
            } else
            {
                elsToShow = EmergencyLighting.Get(IncludeBuiltInSettings, IncludeCustomSettings).Where(e => e.Exists());
            }

            // Add any new lighting entries
            foreach (EmergencyLighting els in elsToShow)
            {
                if (forceUpdateAll || !elsEntries.ContainsKey(els))
                {
                    if (!elsEntries.TryGetValue(els, out T menuEntry))
                    {
                        menuEntry = CreateNewSirenSettingMenuItem(els);
                        elsEntries.Add(els, menuEntry);
                        Menu.AddItem(menuEntry);
                    }

                    Game.LogTrivialDebug("Added EmergencyLighting entry " + els.Name);
                }
            }

            // Remove any lighting entries which are no longer valid and update labels for valid items
            foreach (EmergencyLighting els in elsEntries.Keys.ToArray())
            {
                if(!els.IsValid() || !elsToShow.Contains(els))
                {
                    RemoveEntry(els);
                } else
                {
                    var item = elsEntries[els];
                    item.Text = els.Name;
                    bool isCheckbox = (item is UIMenuCheckboxItem);
                    bool isCustom = els.IsCustomSetting();
                    SirenSource src = els.GetSource();

                    if (isCustom)
                    {
                        item.LeftBadge = UIMenuItem.BadgeStyle.Car;
                        item.Description = "~g~Editable~w~ siren setting entry";
                        if (src != null && (src.SourceId > 0 || src.Source == EmergencyLightingSource.Manual))
                        {
                            if (!isCheckbox) item.RightLabel = $"~c~[{src.SourceId}*]";
                            item.Description += $" {src.SourceDescription.ToLower()} from siren setting ID ~b~{src.SourceId}~s~";
                        }
                    }
                    else
                    {
                        item.LeftBadge = UIMenuItem.BadgeStyle.Lock;
                        item.Description = $"~y~Built-in~w~ siren setting entry, siren setting ID ~b~{els.SirenSettingID()}~s~";
                        
                        if (!isCheckbox) item.RightLabel = $"~c~[{els.SirenSettingID()}]";

                        if (AlwaysReturnEditableSetting)
                        {
                            item.Description += ". An ~g~editable~w~ copy will be created if you select this setting.";
                        }
                    }
                }
            }

            Menu.RefreshIndex();
        }

        protected virtual void RemoveEntry(EmergencyLighting els)
        {
            if (elsEntries.ContainsKey(els))
            {
                Menu.RemoveItemAt(Menu.MenuItems.IndexOf(elsEntries[els]));
            }
            elsEntries.Remove(els);
            Game.LogTrivialDebug("Removed EmergencyLighting entry " + (els.IsValid() ? " of undesired type" : "for being invalid"));
        }
    }

    internal class SirenSettingsSelectionMenu : BaseSirenSettingsSelectionMenu<SirenSettingMenuItem>
    {
        public delegate void SirenSettingSelectedEvent(SirenSettingsSelectionMenu sender, UIMenu menu, SirenSettingMenuItem item, EmergencyLighting setting);
        public event SirenSettingSelectedEvent OnSirenSettingSelected;

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
                if (value != null)
                {
                    elsEntries.TryGetValue(value, out item);
                }
                SetSelectedSetting(item);
            }
        }

        public bool CloseOnSelection { get; set; }

        public SirenSettingsSelectionMenu(EmergencyLighting initialSelected, bool closeOnSelect = true, bool builtIn = true, bool custom = true, bool returnEditable = true, IEnumerable<EmergencyLighting> customList = null) : base(builtIn, custom, returnEditable, customList)
        {
            this.CloseOnSelection = closeOnSelect;
            SelectedEmergencyLighting = initialSelected;
            Menu.OnItemSelect += OnMenuItemSelected;
        }

        private void SetSelectedSetting(SirenSettingMenuItem item)
        {
            if (item != selectedSetting?.Item2)
            {
                if (selectedSetting?.Item2 != null)
                {
                    selectedSetting.Item2.RightBadge = UIMenuItem.BadgeStyle.Blank;
                    selectedSetting.Item2.BackColor = Color.Empty;
                }

                if (item == null)
                {
                    selectedSetting = null;
                }
                else
                {
                    EmergencyLighting selectedEls = item.ELS;
                    if (AlwaysReturnEditableSetting && !selectedEls.IsCustomSetting())
                    {
                        selectedEls = selectedEls.CloneWithID();
                        RefreshSirenSettingList();
                        item = elsEntries[selectedEls];
                    }
                    selectedSetting = Tuple.Create(item.ELS, item);
                    item.RightBadge = UIMenuItem.BadgeStyle.Tick;
                    item.BackColor = Color.DarkGray;
                }
                OnSirenSettingSelected?.Invoke(this, Menu, item, item?.ELS);
                SelectSelectedItem();
                UpdateBoundMenuLabel(Menu.ParentItem);
            }
        }

        private void SelectSelectedItem()
        {
            Menu.RefreshIndex();
            if (selectedSetting != null)
            {
                Menu.CurrentSelection = Menu.MenuItems.IndexOf(selectedSetting.Item2);
            }
        }

        public override void RefreshSirenSettingList(bool forceUpdateAll = false)
        {
            base.RefreshSirenSettingList(forceUpdateAll);
            SelectSelectedItem();
        }

        public override void UpdateBoundMenuLabel(UIMenuItem item)
        {
            if (item == null) return;

            if (selectedSetting?.Item1?.Exists() == true)
            {
                item.RightLabel = selectedSetting.Item1.Name + " →";
            }
            else
            {
                item.RightLabel = "~c~none~w~";
            }
        }

        private void OnMenuItemSelected(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            SirenSettingMenuItem selectedSetting = selectedItem as SirenSettingMenuItem;
            if (selectedSetting != null)
            {
                SetSelectedSetting(selectedSetting);
                if (CloseOnSelection)
                {
                    Menu.GoBack();
                }
            }
        }

        protected override void RemoveEntry(EmergencyLighting els)
        {
            base.RemoveEntry(els);
            if (SelectedEmergencyLighting == els)
            {
                SetSelectedSetting(null);
            }
        }

        protected override SirenSettingMenuItem CreateNewSirenSettingMenuItem(EmergencyLighting els) => new SirenSettingMenuItem(els);
    }

    
    internal class SirenSettingsSelectionMenuMulti : BaseSirenSettingsSelectionMenu<MultiSirenSettingMenuItem>
    {
        public UIMenuItem AcceptMenuItem { get; } 
        public bool ShowAcceptButton { get; set; }

        public EmergencyLighting[] SelectedItems => elsEntries.Where(e => e.Value.Checked).Select(e => e.Key).ToArray();

        public void SelectItems(bool clearOthers, params EmergencyLighting[] items)
        {
            foreach (var entry in elsEntries)
            {
                entry.Value.Checked = items.Contains(entry.Key) || (entry.Value.Checked && !clearOthers);
            }
            UpdateBoundMenuLabel(Menu.ParentItem);
        }

        public void SelectItems(bool clearOthers, IEnumerable<EmergencyLighting> items) => SelectItems(clearOthers, items.ToArray());
        public void SelectItems(IEnumerable<EmergencyLighting> items) => SelectItems(true, items);
        public void SelectItems(params EmergencyLighting[] items) => SelectItems(true, items);

        public SirenSettingsSelectionMenuMulti(bool showAcceptButton = false, bool builtIn = true, bool custom = true, bool returnEditable = true, IEnumerable<EmergencyLighting> customList = null, IEnumerable<EmergencyLighting> initialSelected = null) : base(builtIn, custom, returnEditable, customList)
        {
            this.ShowAcceptButton = showAcceptButton;
            AcceptMenuItem = new UIMenuItem("Accept Selection");
            AcceptMenuItem.HighlightedForeColor = Color.Green;
            AcceptMenuItem.RightLabel = "→";

            if (initialSelected != null)
            {
                SelectItems(initialSelected);
            }
        }

        protected override MultiSirenSettingMenuItem CreateNewSirenSettingMenuItem(EmergencyLighting els) => new MultiSirenSettingMenuItem(els);

        public override void UpdateBoundMenuLabel(UIMenuItem item)
        {
            if (item == null) return;

            int numSelected = SelectedItems.Length;

            if (numSelected == 0)
            {
                item.RightLabel = "~c~None selected~s~ →";
            } else if (numSelected == 1)
            {
                item.RightLabel = SelectedItems[0].Name;
            }
            else
            {
                item.RightLabel = $"{numSelected} selected →";
            }
        }

        public override void RefreshSirenSettingList(bool forceUpdateAll = false)
        {
            base.RefreshSirenSettingList(forceUpdateAll);
            Menu.MenuItems.Remove(AcceptMenuItem);
            if (ShowAcceptButton) Menu.AddItem(AcceptMenuItem);
            Menu.RefreshIndex();
            Menu.OnCheckboxChange += OnCheckboxItemChanged;
        }

        private void OnCheckboxItemChanged(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked)
        {
            UpdateBoundMenuLabel(Menu.ParentItem);
        }
    }
    
}

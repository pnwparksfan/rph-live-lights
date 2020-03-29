using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RAGENativeUI.Elements
{
    using System.Drawing;
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using LiveLights.Utils;

    // This needs to be a separate non-generic interface so we can have a List<IRefreshableItemWrapper>
    // which calls RefreshFromData() without knowing what the data types are
    internal interface IRefreshableItemWrapper
    {
        void RefreshFromData();
        UIMenuItem MenuItem { get; }
    }

    internal interface IRefreshableBindingWrapper<T> : IRefreshableItemWrapper
    {
        Action<T> MenuUpdateBinding { get; set; }
        Func<T> DataUpdateBinding { get; set; }
        void SetBindings(Action<T> menuBinding, Func<T> dataBinding);
    }

    internal class UIMenuValueEntrySelector<T> : IRefreshableBindingWrapper<T> // where T : IEquatable<T>
    {
        public static implicit operator UIMenuItem(UIMenuValueEntrySelector<T> i) => i.MenuItem;

        public UIMenuValueEntrySelector(UIMenuItem menuItem, T value)
        {
            // Must set MenuItem before setting ItemValue, 
            // because ItemValue setter uses MenuItem getter
            this.MenuItem = menuItem;
            this.ItemValue = value;
            this.MenuItem.Activated += ActivatedHandler;
            this.MenuItem.Description += "\n Press ~b~ENTER~w~ to set custom value";
        }

        public virtual UIMenuItem MenuItem { get; }

        public Action<T> MenuUpdateBinding { get; set; } = null;
        public Func<T> DataUpdateBinding { get; set; } = null;

        public void SetBindings(Action<T> menuBinding, Func<T> dataBinding)
        {
            this.MenuUpdateBinding = menuBinding;
            this.DataUpdateBinding = dataBinding;
        }

        protected virtual T itemValue { get; set; }
        public T ItemValue
        {
            get => itemValue;

            set
            {
                try
                {
                    MenuUpdateBinding?.Invoke(value);
                    itemValue = value;
                } catch (Exception e)
                {
                    Game.DisplaySubtitle($"Unable to set ~b~{this.MenuItem.Text}~w~ to \"{value}\":~n~~r~{e.Message}", 20000);
                    return;
                }

                UpdateMenuDisplay();
                OnValueEntryChanged?.Invoke(this.MenuItem.Parent, this.MenuItem, this, value);
                OnValueChanged?.Invoke(value);
            }
        }

        public void RefreshFromData()
        {
            // Use the itemValue FIELD not the PROPERTY here, because we don't want to 
            // trigger the data update binding or property changed handlers if the 
            // value was changed externally rather than through the UI
            T value = DataUpdateBinding();
            if(!EqualityComparer<T>.Default.Equals(value, itemValue))
            {
                itemValue = value;
                UpdateMenuDisplay();
            }
        }

        public delegate void ValueEntryChangedEvent(UIMenu sender, UIMenuItem menuItem, UIMenuValueEntrySelector<T> selector, T value);
        public event ValueEntryChangedEvent OnValueEntryChanged;

        public delegate void ValueChangedEvent(T value);
        public event ValueChangedEvent OnValueChanged;

        protected virtual void UpdateMenuDisplay()
        {
            try
            {
                this.MenuItem.SetRightLabel(DisplayMenu);
            } catch (Exception) { }
        }

        protected virtual int MaxInputLength { get; } = 1000;
        protected virtual string DisplayMenu => ItemValue?.ToString() ?? "(empty)";
        protected virtual string DisplayInputBox => ItemValue.ToString();

        protected virtual void ActivatedHandler(UIMenu sender, UIMenuItem selectedItem)
        {
            string input = GetUserInput(this.MenuItem.Text, DisplayInputBox, this.MaxInputLength);
            if(input != null && ValidateInput(input, out T parsedValue))
            {
                ItemValue = parsedValue;
            } else if(input != null)
            {
                Game.DisplaySubtitle($"The value ~b~{input}~w~ is ~r~invalid~w~ for property ~b~{MenuItem.Text}", 6000);
            }
        }

        private static string GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);

            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, windowTitle, 0, defaultText, 0, 0, 0, maxLength);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
            {
                GameFiber.Yield();
            }

            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }

        private TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

        protected virtual bool ValidateInput(string input, out T value)
        {
            try
            {
                if (converter != null)
                {
                    value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
                    return true;
                }
            }
            catch (Exception) { }

            value = default(T);
            return false;
        }
    }

    // STRING
    internal class UIMenuStringSelector : UIMenuValueEntrySelector<string>
    {
        public UIMenuStringSelector(UIMenuItem menuItem, string value) : base(menuItem, value) { }
        public UIMenuStringSelector(string text, string value) : base(new UIMenuItem(text), value) { }
        public UIMenuStringSelector(string text, string value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out string value)
        {
            value = input;
            return true;
        }

        protected override int MaxInputLength => MaxLength;

        public int MaxLength { get; set; } = 1000;
    }

    // UINT 
    internal class UIMenuUIntSelector : UIMenuValueEntrySelector<uint>
    {
        public UIMenuUIntSelector(UIMenuItem menuItem, uint value) : base(menuItem, value) { }
        public UIMenuUIntSelector(string text, uint value) : base(new UIMenuItem(text), value) { }
        public UIMenuUIntSelector(string text, uint value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out uint value) => uint.TryParse(input, out value);

        protected override int MaxInputLength => uint.MaxValue.ToString().Length;
    }

    // INT
    internal class UIMenuIntSelector : UIMenuValueEntrySelector<int>
    {
        public UIMenuIntSelector(UIMenuItem menuItem, int value) : base(menuItem, value) { }
        public UIMenuIntSelector(string text, int value) : base(new UIMenuItem(text), value) { }
        public UIMenuIntSelector(string text, int value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out int value) => int.TryParse(input, out value);

        protected override int MaxInputLength => Math.Max(int.MaxValue.ToString().Length, int.MinValue.ToString().Length);
    }

    // FLOAT
    internal class UIMenuFloatSelector : UIMenuValueEntrySelector<float>
    {
        public UIMenuFloatSelector(UIMenuItem menuItem, float value) : base(menuItem, value) { }
        public UIMenuFloatSelector(string text, float value) : base(new UIMenuItem(text), value) { }
        public UIMenuFloatSelector(string text, float value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out float value) => float.TryParse(input, out value);
    }

    // VECTOR3
    internal class UIMenuVector3Selector : UIMenuValueEntrySelector<Vector3>
    {
        public UIMenuVector3Selector(UIMenuItem menuItem, Vector3 value) : base(menuItem, value) { }
        public UIMenuVector3Selector(string text, Vector3 value) : base(new UIMenuItem(text), value) { }
        public UIMenuVector3Selector(string text, Vector3 value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override string DisplayInputBox => string.Format("{0},{1},{2}", ItemValue.X, ItemValue.Y, ItemValue.Z);

        protected override bool ValidateInput(string input, out Vector3 value)
        {
            value = Vector3.Zero;
            string[] inputs = input.Split(',');
            if (inputs.Length != 3) return false;

            float[] outputs = new float[3];
            bool success = true;
            for (int i = 0; i < inputs.Length; i++)
            {
                success = success && float.TryParse(inputs[i], out outputs[i]);
            }

            if(success)
            {
                value = new Vector3(outputs[0], outputs[1], outputs[2]);
            }

            return success;
        }
    }

    // CUSTOM-EDIT LIST SELECTOR
    internal class UIMenuListItemSelector<T> : UIMenuValueEntrySelector<T> // where T : IEquatable<T>
    {
        // public UIMenuListItem ListMenuItem => MenuItem as UIMenuListItem;
        // public override UIMenuItem MenuItem => ListMenuItem;
        public UIMenuCustomListItem<T> ListMenuItem => MenuItem as UIMenuCustomListItem<T>;

        public UIMenuListItemSelector(string text, string description, T value, IEnumerable<IDisplayItem> items) : this(new UIMenuCustomListItem<T>(text, description, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, params IDisplayItem[] items) : this(new UIMenuCustomListItem<T>(text, description, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, IEnumerable<T> items) : this(new UIMenuCustomListItem<T>(text, description, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, params T[] items) : this(new UIMenuCustomListItem<T>(text, description, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, Func<T, string> labelGenerator, IEnumerable<IDisplayItem> items) : this(new UIMenuCustomListItem<T>(text, description, labelGenerator, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, Func<T, string> labelGenerator, params IDisplayItem[] items) : this(new UIMenuCustomListItem<T>(text, description, labelGenerator, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, Func<T, string> labelGenerator, IEnumerable<T> items) : this(new UIMenuCustomListItem<T>(text, description, labelGenerator, items), value) { }

        public UIMenuListItemSelector(string text, string description, T value, Func<T, string> labelGenerator, params T[] items) : this(new UIMenuCustomListItem<T>(text, description, labelGenerator, items), value) { }

        public UIMenuListItemSelector(UIMenuCustomListItem<T> menuItem, T value) : base(menuItem, value)
        {
            ListMenuItem.OnListChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(UIMenuItem sender, int newIndex)
        {
            this.ItemValue = ListMenuItem.Value;
        }

        /*
        protected override void UpdateMenuDisplay()
        {
            if (!EqualityComparer<T>.Default.Equals(ListMenuItem.Value, ItemValue))
            {
                ListMenuItem.Value = ItemValue;
            }
        }
        */
        protected override void UpdateMenuDisplay() { }

        protected override T itemValue
        {
            get => ListMenuItem.Value;
            set => ListMenuItem.Value = value;
        }
    }

    // Color Selector
    internal class UIMenuColorSelector : UIMenuListItemSelector<Color>
    {
        public class ColorDisplayItem : IDisplayItem
        {
            public ColorDisplayItem(Color color)
            {
                this.Color = color;
            }

            public static implicit operator Color(ColorDisplayItem item) => item.Color;

            public static implicit operator ColorDisplayItem(Color color) => new ColorDisplayItem(color);

            public Color Color { get; set; }

            public object Value => Color;

            public string DisplayText => Color.DisplayText();

            public bool Equals(IDisplayItem other)
            {
                return (
                    other.Value != null 
                    && other.Value.GetType() == typeof(Color) 
                    && ((Color)other.Value).ToArgb() == Color.ToArgb()
                );
            }

            // public override int GetHashCode() => this.Color.ToArgb();
        }

        public UIMenuColorSelector(string text, string description, Color value, params Color[] items) : base(text, description, value, (c) => c.DisplayText(), items.Select(c => new ColorDisplayItem(c))) { }
        public UIMenuColorSelector(string text, string description, Color value, IEnumerable<Color> items) : base(text, description, value, (c) => c.DisplayText(), items.Select(c => new ColorDisplayItem(c))) { }
        public UIMenuColorSelector(string text, string description, Color value, params KnownColor[] items) : this(text, description, value, items.Select(Color.FromKnownColor)) { }
        public UIMenuColorSelector(string text, string description, Color value, IEnumerable<KnownColor> items) : this(text, description, value, items.Select(Color.FromKnownColor)) { }

        protected override string DisplayInputBox => ItemValue.DisplayText();

        protected override bool ValidateInput(string input, out Color value)
        {
            value = Color.FromName(input);
            if(value.IsValid()) 
            {
                return true;
            }

            try
            {
                string tempInput = input.ToLower().Replace("0x", "").Replace("#", "").Replace("x","").Trim();
                value = ColorTranslator.FromHtml("#" + tempInput);
                if(value.IsValid())
                {
                    return true;
                }
            } catch (Exception) { }

            // If no matches, return empty color
            value = Color.Empty;
            return false;
        }

        protected override Color itemValue 
        { 
            get => base.itemValue.ToNamedColor(false); 
            set => base.itemValue = value.ToNamedColor(false); 
        }
    }

    internal class UIMenuSwitchSelectable : UIMenuValueEntrySelector<int>
    {
        public UIMenuSwitchSelectable(string text, string description, IEnumerable<UIMenu> menus) : base(new UIMenuSwitchMenusItem(text, description, menus), 1) { }

        public UIMenuSwitchSelectable(string text, string description, IEnumerable<IDisplayItem> menus) : base(new UIMenuSwitchMenusItem(text, description, menus), 1) { }

        public UIMenuSwitchMenusItem SwitchMenuItem => (UIMenuSwitchMenusItem)MenuItem;

        protected override void UpdateMenuDisplay() { }
        protected override int MaxInputLength => SwitchMenuItem.Collection.Count.ToString().Length;
        protected override bool ValidateInput(string input, out int value)
        {
            return int.TryParse(input, out value) && value >= 1 && value <= SwitchMenuItem.Collection.Count;
        }

        protected override int itemValue 
        { 
            get => SwitchMenuItem.Index + 1; 
            set => SwitchMenuItem.Index = (value - 1); 
        }
    }

    internal class UIMenuCustomListItem<T> : UIMenuListItem // where T : class
    {
        public T Value
        {
            get => (T)SelectedItem.Value;
            set
            {
                // Only remove custom item if it's not the default value, in case default value was in the original items list
                if(!EqualityComparer<T>.Default.Equals(customItemValue, default(T)) && this.Collection.Contains(customItemValue))
                {
                    this.Collection.Remove(customItemValue);
                }

                if (Collection.Contains(value))
                {
                    this.Index = this.Collection.IndexOf(value);
                    
                } else
                {
                    string label = CustomLabelGenerator?.Invoke(value) ?? value.ToString();
                    if (!AddNewItems)
                    {
                        customItemValue = value;
                        label = "Custom: " + label;
                    }
                    this.Collection.Add(value, label);
                    this.Index = (this.Collection.Count - 1);
                }
            }
        }

        public bool AddNewItems { get; set; }

        public Func<T, string> CustomLabelGenerator { get; set; } = null;

        public void SetAddNewItems(Func<T, string> generator)
        {
            AddNewItems = true;
            this.CustomLabelGenerator = generator;
        }

        private T customItemValue;

        public UIMenuCustomListItem(string text, string description) : base(text, description) { }
        public UIMenuCustomListItem(string text, string description, IEnumerable<IDisplayItem> items) : base(text, description, items) { }
        public UIMenuCustomListItem(string text, string description, IEnumerable<T> items) : base(text, description, items.Select(x => (object)x).ToArray())  { }

        public UIMenuCustomListItem(string text, string description, Func<T, string> labelGenerator, IEnumerable<IDisplayItem> items) : base(text, description, items) 
        {
            this.CustomLabelGenerator = labelGenerator;
        }
        public UIMenuCustomListItem(string text, string description, Func<T, string> labelGenerator, IEnumerable<T> items) : base(text, description, items.Select(x => (object)x).ToArray()) 
        {
            this.CustomLabelGenerator = labelGenerator;
        }
    }

    internal class UIMenuRefreshableCheckboxItem : UIMenuCheckboxItem, IRefreshableBindingWrapper<bool>
    {
        public UIMenuRefreshableCheckboxItem(string text, bool check, string description) : base(text, check, description)
        {
            this.CheckboxEvent += onValueChanged;
        }

        // Whenever the user changes the checkbox, trigger the menu update binding with the new value
        private void onValueChanged(UIMenuCheckboxItem sender, bool Checked)
        {
            MenuUpdateBinding(Checked);
        }

        public Action<bool> MenuUpdateBinding { get; set; }
        public Func<bool> DataUpdateBinding { get; set; }

        public UIMenuItem MenuItem => this;

        public void RefreshFromData()
        {
            this.Checked = DataUpdateBinding();
        }

        public void SetBindings(Action<bool> menuBinding, Func<bool> dataBinding)
        {
            MenuUpdateBinding = menuBinding;
            DataUpdateBinding = dataBinding;
        }
    }

    internal class UIMenuRefreshable : UIMenu
    {
        public UIMenuRefreshable(string title, string subtitle) : base(title, subtitle) { }

        public UIMenuRefreshable(string title, string subtitle, Point offset) : base(title, subtitle, offset) { }

        public UIMenuRefreshable(string title, string subtitle, Point offset, Texture customBanner) : base(title, subtitle, offset, customBanner) { }

        public UIMenuRefreshable(string title, string subtitle, Point offset, string spriteLibrary, string spriteName) : base(title, subtitle, offset, spriteLibrary, spriteName) { }

        private List<IRefreshableItemWrapper> bindings = new List<IRefreshableItemWrapper>();
        private List<UIMenuRefreshable> menuBindings = new List<UIMenuRefreshable>();

        public void AddMenuDataBinding<TMenuItem, TData>(TMenuItem menuItem, Action<TData> menuBinding, Func<TData> dataBinding) where TMenuItem : IRefreshableBindingWrapper<TData> // where TData : IEquatable<TData>
        {
            menuItem.SetBindings(menuBinding, dataBinding);
            this.AddItem(menuItem.MenuItem);
            bindings.Add(menuItem);
        }

        public void AddMenuDataBinding<TMenuItem, TData>(TMenuItem menuItem, Action<TData> menuBinding, Func<TData> dataBinding, Func<IRefreshableItemWrapper> relatedItem) where TMenuItem : IRefreshableBindingWrapper<TData> where TData : IEquatable<TData>
        {
            menuItem.SetBindings(
                (x) => {
                    menuBinding(x);                     // Update this item's data from the menu
                    relatedItem()?.RefreshFromData();   // Update the related item's data from the source
                }, () => dataBinding());

            this.AddItem(menuItem.MenuItem);
            bindings.Add(menuItem);
        }

        // This is used when a submenu isn't directly registered (specifically for 
        // the case of a submenu which uses a menu selector) and is manually configured
        // but still needs to be updated when the parent menu is updated
        public void AddSubMenuBinding(UIMenuRefreshable subMenu) 
        {
            if(subMenu.ParentMenu == this)
            {
                menuBindings.Add(subMenu);
            }
        }

        public void RefreshData(bool refreshSubMenus = true)
        {
            foreach (var binding in bindings)
            {
                // Check if the bound item is still in the menu, in case it was removed
                if(MenuItems.Contains(binding.MenuItem))
                {
                    binding.RefreshFromData();
                }
            }

            if(refreshSubMenus)
            {
                foreach (var subMenu in Children)
                {
                    if (subMenu.Value is UIMenuRefreshable)
                    {
                        ((UIMenuRefreshable)subMenu.Value).RefreshData(true);
                    }
                }
                foreach (var subMenu in menuBindings)
                {
                    subMenu.RefreshData();
                }
            }
        }

        public void AddRefreshItem(UIMenuItem item)
        {
            this.AddItem(item);
            item.Activated += onRefreshActivated;
        }

        private void onRefreshActivated(UIMenu sender, UIMenuItem selectedItem) => this.RefreshData(true);
    }

    internal static class MenuExtensions
    {
        public static void BindMenuAndCopyProperties(this UIMenu parentMenu, UIMenu menuToBind, UIMenuItem itemToBindTo, bool recursiveCopyProperties = true)
        {
            parentMenu.BindMenuToItem(menuToBind, itemToBindTo);
            parentMenu.CopyMenuProperties(menuToBind);
        }

        public static void CopyMenuProperties(this UIMenu parentMenu, UIMenu newMenu, bool recursive = true)
        {
            newMenu.SetMenuWidthOffset(parentMenu.WidthOffset);
            newMenu.ControlDisablingEnabled = parentMenu.ControlDisablingEnabled;
            newMenu.MouseControlsEnabled = parentMenu.MouseControlsEnabled;
            newMenu.MouseEdgeEnabled = parentMenu.MouseEdgeEnabled;
            newMenu.AllowCameraMovement = parentMenu.AllowCameraMovement;

            if(recursive)
            {
                foreach (UIMenu subMenu in newMenu.Children.Values)
                {
                    newMenu.CopyMenuProperties(subMenu, true);
                }
            }
        }

        public static void AddMenuAndSubMenusToPool(this MenuPool pool, UIMenu menu)
        {
            if(!pool.Contains(menu))
            {
                pool.Add(menu);
            }

            foreach (UIMenu subMenu in menu.Children.Values)
            {
                // Don't need to call pool.Add here because it'll 
                // get called above when the function recurses
                pool.AddMenuAndSubMenusToPool(subMenu);
            }
        }
    }
}

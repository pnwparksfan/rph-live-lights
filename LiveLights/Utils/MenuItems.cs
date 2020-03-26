using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RAGENativeUI.Elements
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;

    internal interface IRefreshableItemWrapper
    {
        void RefreshFromData();
        UIMenuItem MenuItem { get; }
    }

    internal class UIMenuValueEntrySelector<T> : IRefreshableItemWrapper where T : IEquatable<T>
    {
        public static implicit operator UIMenuItem(UIMenuValueEntrySelector<T> i) => i.MenuItem;

        public UIMenuValueEntrySelector(UIMenuItem menuItem, T value)
        {
            // Must set MenuItem before setting ItemValue, 
            // because ItemValue setter uses MenuItem getter
            this.MenuItem = menuItem;
            this.ItemValue = value;
            this.MenuItem.Activated += ActivatedHandler;
        }

        public virtual UIMenuItem MenuItem { get; }

        public Action<T> MenuUpdateBinding { get; set; }
        public Func<T> DataUpdateBinding { get; set; }

        public void SetBindings(Action<T> menuBinding, Func<T> dataBinding)
        {
            this.MenuUpdateBinding = menuBinding;
            this.DataUpdateBinding = dataBinding;
        }

        private T itemValue;
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
            } else
            {
                Game.DisplaySubtitle($"The value ~b~{input}~w~ is ~r~invalid~w~ for property ~b~{MenuItem.Text}", 6000);
            }
        }

        private static string GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);

            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, windowTitle, 0, defaultText, 0, 0, 0, maxLength + 1);

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
            catch (NotSupportedException) { }

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

    internal class UIMenuListItemSelector<T> : UIMenuValueEntrySelector<T> where T : IEquatable<T>
    {
        public UIMenuListItem ListMenuItem => MenuItem as UIMenuListItem;
        // public override UIMenuItem MenuItem => ListMenuItem;

        public UIMenuListItemSelector(UIMenuListItem menuItem, T value) : base(menuItem, value) { }

        protected override void UpdateMenuDisplay()
        {
            if(ListMenuItem.SelectedItem.DisplayText != DisplayMenu)
            {
                // find if any match
                // if none match, add one
            }
        }
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RAGENativeUI.Elements
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;


    internal class UIMenuValueEntrySelector<T, TMenuItem> /* : IMenuValueItem<T> */ where TMenuItem : UIMenuItem
    {
        public static implicit operator UIMenuItem(UIMenuValueEntrySelector<T, TMenuItem> i) => i.MenuItem;

        public UIMenuItem MenuItem { get; }

        public UIMenuValueEntrySelector(UIMenuItem menuItem, T value) // : base(text)
        {
            this.MenuItem = menuItem;
            this.ItemValue = value;
            this.MenuItem.Activated += ActivatedHandler;
        }

        /*
        public UIMenuValueEntrySelector(string text, T value, string description) // : base(text, description)
        {
            this.ItemValue = value;
            this.Activated += ActivatedHandler;
        }
        */

        private T itemValue;
        public T ItemValue
        {
            get => itemValue;

            set
            {
                itemValue = value;
                UpdateMenuDisplay();
            }
        }

        protected virtual void UpdateMenuDisplay() => this.MenuItem.SetRightLabel(DisplayMenu);

        protected virtual int MaxInputLength { get; } = 1000;
        protected virtual string DisplayMenu => ItemValue?.ToString() ?? "(empty)";
        protected virtual string DisplayInputBox => ItemValue.ToString();
        // public override string RightLabel => DisplayMenu;

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
    internal class UIMenuStringSelector : UIMenuValueEntrySelector<string, UIMenuItem>
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
    internal class UIMenuUIntSelector : UIMenuValueEntrySelector<uint, UIMenuItem>
    {
        public UIMenuUIntSelector(UIMenuItem menuItem, uint value) : base(menuItem, value) { }
        public UIMenuUIntSelector(string text, uint value) : base(new UIMenuItem(text), value) { }
        public UIMenuUIntSelector(string text, uint value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out uint value) => uint.TryParse(input, out value);

        protected override int MaxInputLength => uint.MaxValue.ToString().Length;
    }

    // INT
    internal class UIMenuIntSelector : UIMenuValueEntrySelector<int, UIMenuItem>
    {
        public UIMenuIntSelector(UIMenuItem menuItem, int value) : base(menuItem, value) { }
        public UIMenuIntSelector(string text, int value) : base(new UIMenuItem(text), value) { }
        public UIMenuIntSelector(string text, int value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out int value) => int.TryParse(input, out value);

        protected override int MaxInputLength => Math.Max(int.MaxValue.ToString().Length, int.MinValue.ToString().Length);
    }

    // FLOAT
    internal class UIMenuFloatSelector : UIMenuValueEntrySelector<float, UIMenuItem>
    {
        public UIMenuFloatSelector(UIMenuItem menuItem, float value) : base(menuItem, value) { }
        public UIMenuFloatSelector(string text, float value) : base(new UIMenuItem(text), value) { }
        public UIMenuFloatSelector(string text, float value, string description) : base(new UIMenuItem(text, description), value) { }

        protected override bool ValidateInput(string input, out float value) => float.TryParse(input, out value);
    }

    // VECTOR3
    internal class UIMenuVector3Selector : UIMenuValueEntrySelector<Vector3, UIMenuItem>
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

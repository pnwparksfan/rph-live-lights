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

    internal interface IMenuValueItem<T>
    {
        T ItemValue { get; }
    }

    internal class UIMenuValueEntrySelector<T> : UIMenuItem, IMenuValueItem<T>
    {
        public UIMenuValueEntrySelector(string text, T value) : base(text)
        {
            this.ItemValue = value;
            this.Activated += ActivatedHandler;
        }

        public UIMenuValueEntrySelector(string text, T value, string description) : base(text, description)
        {
            this.ItemValue = value;
            this.Activated += ActivatedHandler;
        }

        public T ItemValue { get; set; }
        
        // public object ItemValue => Value;
        protected virtual int MaxInputLength { get; } = 1000;
        // protected abstract bool ValidateInput(string input, out T value);
        protected virtual string DisplayMenu => ItemValue.ToString();
        protected virtual string DisplayInputBox => ItemValue.ToString();
        public override string RightLabel => DisplayMenu;

        protected virtual void ActivatedHandler(UIMenu sender, UIMenuItem selectedItem)
        {
            string input = GetUserInput(this.Text, DisplayInputBox, this.MaxInputLength);
            if(input != null && ValidateInput(input, out T parsedValue))
            {
                ItemValue = parsedValue;
            } else
            {
                Game.DisplaySubtitle($"The value ~b~{input}~w~ is ~r~invalid~w~ for property ~b~{Text}", 6000);
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

    /*
    internal class UIMenuGenericSelector<T> : UIMenuValueEntrySelector<T>
    {
      
        public UIMenuGenericSelector(string text, T value) : base(text, value) { }
        public UIMenuGenericSelector(string text, T value, string description) : base(text, value, description) { }

        private TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

        protected override bool ValidateInput(string input, out T value)
        {
            try
            {
                if (converter != null)
                {
                    value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
                    return true;
                }
            } catch(NotSupportedException) { }

            value = default(T);
            return false;
        }
    }
    */

    internal class UIMenuStringSelector : UIMenuValueEntrySelector<string>
    {
        public UIMenuStringSelector(string text, string value) : base(text, value) { }
        public UIMenuStringSelector(string text, string value, string description) : base(text, value, description) { }

        protected override bool ValidateInput(string input, out string value)
        {
            value = input;
            return true;
        }
    }

    internal class UIMenuUIntSelector : UIMenuValueEntrySelector<uint>
    {
        public UIMenuUIntSelector(string text, uint value) : base(text, value) { }
        public UIMenuUIntSelector(string text, uint value, string description) : base(text, value, description) { }

        protected override bool ValidateInput(string input, out uint value) => uint.TryParse(input, out value);
    }

    internal class UIMenuIntSelector : UIMenuValueEntrySelector<int>
    {
        public UIMenuIntSelector(string text, int value) : base(text, value) { }
        public UIMenuIntSelector(string text, int value, string description) : base(text, value, description) { }

        protected override bool ValidateInput(string input, out int value) => int.TryParse(input, out value);
    }

    internal class UIMenuFloatSelector : UIMenuValueEntrySelector<float>
    {
        public UIMenuFloatSelector(string text, float value) : base(text, value) { }
        public UIMenuFloatSelector(string text, float value, string description) : base(text, value, description) { }

        protected override bool ValidateInput(string input, out float value) => float.TryParse(input, out value);
    }

    internal class UIMenuVector3Selector : UIMenuValueEntrySelector<Vector3>
    {
        public UIMenuVector3Selector(string text, Vector3 value) : base(text, value) { }
        public UIMenuVector3Selector(string text, Vector3 value, string description) : base(text, value, description) { }

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

    internal class UIMenuCheckBoxValueItem : UIMenuCheckboxItem, IMenuValueItem<bool>
    {
        public bool ItemValue => this.Checked;

        public UIMenuCheckBoxValueItem(string text, bool check) : base(text, check) {}
        public UIMenuCheckBoxValueItem(string text, bool check, string description) : base(text, check, description) { }
    }
}

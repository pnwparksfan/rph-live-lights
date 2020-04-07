using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Menu
{
    using RAGENativeUI.Elements;

    internal class UIMenuSequenceItemSelector : UIMenuStringSelector
    {
        public UIMenuSequenceItemSelector(string text, string value, string description) : base(new UIMenuMonoSpaceItem(text, description), value)
        {
        }


        public static string FormatSequence(string sequence) => sequence.Replace("0", "~c~0~w~").Replace("1", "~r~1~w~");
        protected override int MaxInputLength => 32;
        protected override string DisplayMenu => FormatSequence(base.DisplayMenu);
    }
}

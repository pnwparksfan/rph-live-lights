using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LiveLights.Menu
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal class SequenceQuickEditMenu
    {
        public SequenceQuickEditMenu(EmergencyLighting els, EmergencyLightingMenu parent)
        {
            this.ELS = els;
            this.Parent = parent;

            Menu = new UIMenuRefreshable("Edit Sequences", parent.Menu.Subtitle.Caption + " > Sequence Quick Edit");

            for (int i = 0; i < ELS.Lights.Length; i++)
            {
                EmergencyLight siren = ELS.Lights[i];
                string sirenId = $"Siren {i + 1}";
                UIMenuSequenceItemSelector item = new UIMenuSequenceItemSelector($"{sirenId} Sequence", siren.FlashinessSequence, $"Edit 32-bit sequence for {sirenId}");
                Menu.AddMenuDataBinding(item, (x) => siren.FlashinessSequence = x, () => siren.FlashinessSequence);
                sirenSequenceItems.Add(item);
            }

            Menu.AddItem(Parent.LeftHeadlightSequenceItem);
            sirenSequenceItems.Add(Parent.LeftHeadlightSequenceItem);
            Menu.AddItem(Parent.RightHeadlightSequenceItem);
            sirenSequenceItems.Add(Parent.RightHeadlightSequenceItem);
            Menu.AddItem(Parent.LeftTaillightSequenceItem);
            sirenSequenceItems.Add(Parent.LeftTaillightSequenceItem);
            Menu.AddItem(Parent.RightTaillightSequenceItem);
            sirenSequenceItems.Add(Parent.RightTaillightSequenceItem);


            buttons.Add(new InstructionalButton("V", "Paste sequence"));
            buttons.Add(new InstructionalButton("C", "Copy sequence"));
            buttons.Add(new InstructionalButton("dn", "Move siren down"));
            buttons.Add(new InstructionalButton("up", "Move siren up"));
            buttons.Add(new InstructionalButton("R", "Invert sequence"));
            buttons.Add(new InstructionalButton("E", "Extend sequence"));
            buttons.Add(new InstructionalButton(GameControl.FrontendRight, "Shift sequence"));
            buttons.Add(new InstructionalButton(GameControl.FrontendLeft, "Shift sequence"));
            buttons.Add(new InstructionalButton(GameControl.Duck, "Shift x4"));

            foreach (InstructionalButton button in buttons)
            {
                Menu.AddInstructionalButton(button);
            }

            Menu.OnMenuClose += OnMenuClose;
        }

        private void OnMenuClose(UIMenu sender)
        {
            // This menu changes sequences, so we need to refresh all other 
            // sub-menus where those sequences may appear
            Parent.Menu.RefreshData();
        }

        internal void Process()
        {
            if(Menu.Visible)
            {
                
                if(Game.IsKeyDown(Keys.C))
                {
                    copiedSequence = getSelectedSequence()?.ItemValue;
                    Common.PlaySound(Menu.AUDIO_SELECT, Menu.AUDIO_LIBRARY);
                } else if(Game.IsKeyDown(Keys.V) && copiedSequence != null)
                {
                    var s = getSelectedSequence();
                    if (s != null)
                    {
                        s.ItemValue = copiedSequence;
                        Common.PlaySound(Menu.AUDIO_SELECT, Menu.AUDIO_LIBRARY);
                    }
                } else if(Game.IsControlJustPressed(13, GameControl.FrontendLeft))
                {
                    shiftSelectedSequence(-1);
                } else if(Game.IsControlJustPressed(13, GameControl.FrontendRight))
                {
                    shiftSelectedSequence(1);
                } else if(Game.IsKeyDown(Keys.PageDown))
                {
                    shiftSelectedItem(1);
                } else if(Game.IsKeyDown(Keys.PageUp))
                {
                    shiftSelectedItem(-1);
                } else if(Game.IsKeyDown(Keys.R))
                {
                    invertSelectedSequence();
                } else if(Game.IsKeyDown(Keys.E))
                {
                    extendSelectedSequence();
                }

                if(!string.IsNullOrEmpty(copiedSequence))
                {
                    string seqDisp = "Copied sequence " + UIMenuSequenceItemSelector.FormatSequence(copiedSequence);
                    Point p = new Point(Menu.WidthOffset + 500, 30);
                    ResText.Draw(seqDisp, p, 0.4f, Color.White, Common.EFont.Monospace, false);
                    int rectW = (int)ResText.MeasureStringWidth(seqDisp, Common.EFont.Monospace, 0.4f);
                    ResRectangle.Draw(new Point(p.X - 10, p.Y - 1), new Size(rectW + 20, 31), Color.FromArgb(180, Color.Black));
                }
            }
        }

        private string copiedSequence = "";

        private void shiftSelectedItem(int shift)
        {
            var item = getSelectedSequence()?.MenuItem;
            if(item != null)
            {
                int currentIndex = Menu.MenuItems.IndexOf(item);
                int newIndex = (currentIndex + shift);
                // if(newIndex > currentIndex) newIndex--;

                Menu.MenuItems.RemoveAt(currentIndex);
                Menu.MenuItems.Insert(newIndex, item);
                Menu.RefreshIndex();
                Menu.CurrentSelection = newIndex;
                Common.PlaySound(Menu.AUDIO_LEFTRIGHT, Menu.AUDIO_LIBRARY);
            }
        }

        private void extendSelectedSequence()
        {
            var s = getSelectedSequence();
            string seq = s.ItemValue;
            char[] newSeq = new char[32];

            int iOld = 0;
            for (int iNew = 0; iNew < newSeq.Length; iNew++)
            {
                if(iNew > seq.Length - 1)
                {
                    newSeq[iNew] = seq[iOld % seq.Length];
                    iOld++;
                } else
                {
                    newSeq[iNew] = seq[iNew];
                }
            }

            s.ItemValue = string.Join("", newSeq);
        }

        private void invertSelectedSequence()
        {
            var s = getSelectedSequence();
            string seq = s.ItemValue;
            seq = string.Join("", seq.Reverse().ToArray());
            s.ItemValue = seq;
        }

        private void shiftSelectedSequence(int shift)
        {
            shift *= -1;
            if(NativeFunction.Natives.IS_DISABLED_CONTROL_PRESSED<bool>(0, (int)GameControl.Duck))
            {
                shift *= 4;
            }
            var s = getSelectedSequence();
            if(s != null)
            {
                string seq = s.ItemValue;
                if (shift < 0)
                {
                    shift = seq.Length + shift;
                }
                s.ItemValue = seq.Substring(shift) + seq.Substring(0, shift);
                Common.PlaySound(Menu.AUDIO_LEFTRIGHT, Menu.AUDIO_LIBRARY);
            }
        }

        private UIMenuSequenceItemSelector getSelectedSequence()
        {
            UIMenuItem selectedItem = Menu.MenuItems[Menu.CurrentSelection];
            return SirenSequenceItems.FirstOrDefault(x => x.MenuItem == selectedItem);
        }

        public EmergencyLightingMenu Parent { get; }
        public EmergencyLighting ELS { get; }
        public UIMenuRefreshable Menu { get; }

        private List<UIMenuSequenceItemSelector> sirenSequenceItems = new List<UIMenuSequenceItemSelector>();

        public UIMenuSequenceItemSelector[] SirenSequenceItems => sirenSequenceItems.ToArray();

        private List<InstructionalButton> buttons = new List<InstructionalButton>();
    }
}

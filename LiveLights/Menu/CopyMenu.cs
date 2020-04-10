using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;

namespace LiveLights.Menu
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;

    internal class CopyMenu
    {
        public CopyMenu(EmergencyLightingMenu parent)
        {
            this.ParentMenu = parent;
            Menu = new UIMenu("Edit Siren", parent.Menu.Subtitle.Caption + " > Copy Siren Properties");
            DestinationSirenSelectorMenu.Menu.Subtitle.Caption = "~b~Copy Siren Properties > Select Destination Sirens";
            TargetMenu = new SirenSettingsSelectionMenu(ParentELS);

            allCopyCheckboxes.Add(SequencesCheckbox);
            allCopyCheckboxes.Add(DeltasCheckbox);
            allCopyCheckboxes.Add(FlashinessCheckbox);
            allCopyCheckboxes.Add(RotationCheckbox);
            allCopyCheckboxes.Add(EnvLightingCheckbox);
            allCopyCheckboxes.Add(CoronaCheckbox);
            allCopyCheckboxes.Add(ColorCheckbox);
            allCopyCheckboxes.Add(SettingEnvCheckbox);

            Menu.AddItem(AllPropertiesCheckbox);
            foreach (UIMenuCheckboxItem checkbox in allCopyCheckboxes)
            {
                checkbox.BackColor = Color.FromArgb(100, Color.DarkGray);
                checkbox.ForeColor = Color.WhiteSmoke;
                checkbox.Text = "    " + checkbox.Text;
                Menu.AddItem(checkbox);
            }

            AllPropertiesCheckbox.CheckboxEvent += OnAllPropertiesChecked;


            Menu.AddItem(CopyModeItem);
            TargetMenuItem = TargetMenu.CreateAndBindToSubmenuItem(this.Menu, "Target", "Select target siren setting to copy to/from", true);
            Menu.AddItem(SourceSirenSelector);
            Menu.AddItem(DestinationSirenMenuItem);
            Menu.BindMenuAndCopyProperties(DestinationSirenSelectorMenu.Menu, DestinationSirenMenuItem);
            DestinationSirenMenuItem.SetRightLabel("None →");
            Menu.AddItem(CopyItem);

            CopyModeItem.OnListChanged += OnCopyModeChanged;
            CopyItem.Activated += OnCopyActivated;
            CopyItem.BackColor = Color.DarkGreen;
            CopyItem.ForeColor = Color.White;
            CopyItem.HighlightedBackColor = Color.PaleGreen;
            CopyItem.HighlightedForeColor = Color.DarkGreen;

            OnCopyModeChanged(CopyModeItem, 0);

            Menu.RefreshIndex();

        }

        private void OnAllPropertiesChecked(UIMenuCheckboxItem sender, bool Checked)
        {
            foreach (UIMenuCheckboxItem checkbox in allCopyCheckboxes)
            {
                checkbox.Enabled = !Checked;
                checkbox.SetLeftBadge(Checked ? UIMenuItem.BadgeStyle.Lock : UIMenuItem.BadgeStyle.None);
            }
        }

        private void OnCopyActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            (EmergencyLighting source, EmergencyLighting destination) = GetCopySourceAndDestination();
            if(source.Exists() && destination.Exists() && destination.IsCustomSetting())
            {
                EmergencyLight sourceSiren = source.Lights[SourceSiren - 1];
                bool copyAll = AllPropertiesCheckbox.Checked;

                foreach (int sirenId in DestinationSirens)
                {
                    EmergencyLight destinationSiren = destination.Lights[sirenId - 1];

                    if(copyAll)
                    {

                    } else
                    {
                        if (SequencesCheckbox.Checked)
                        {
                            destinationSiren.FlashinessSequence = sourceSiren.FlashinessSequence;
                            destinationSiren.RotationSequence = sourceSiren.RotationSequence;
                        }
                    }
                }
            } else
            {
                Game.DisplayNotification("~y~Unable to copy settings~w~, check that target is selected and destination is editable");
                Game.LogTrivial($"Source valid: {source.Exists()}, Destination valid: {destination.Exists()}, Destination editable: {destination.IsCustomSetting()}");
            }
        }

        private void OnCopyModeChanged(UIMenuItem sender, int newIndex)
        {
            if(CopyMode == COPY_MODE_SELF)
            {
                TargetMenu.SelectedEmergencyLighting = ParentELS;
                TargetMenuItem.Enabled = false;
                TargetMenuItem.Description = "Selection mode set to \"self\"";
                CopyModeItem.Description = "Copy properties from one siren to others on the current siren setting";
            } else
            {
                TargetMenuItem.Enabled = true;
                TargetMenu.AlwaysReturnEditableSetting = (CopyMode == COPY_MODE_TO_TARGET); // Only need an editable target if copying to target
                TargetMenuItem.Description = "Select siren setting to copy properties " + (CopyMode == COPY_MODE_TO_TARGET ? "TO" : "FROM");
                if(CopyMode == COPY_MODE_TO_TARGET)
                {
                    CopyModeItem.Description = "Copy properties from current siren setting to selected target";
                } else
                {
                    CopyModeItem.Description = "Copy properties from selected target to current siren setting";
                }
            }
        }

        private string CopyMode => (string)(CopyModeItem.SelectedValue);

        public EmergencyLightingMenu ParentMenu { get; }
        public EmergencyLighting ParentELS => ParentMenu.ELS;
        public UIMenu Menu { get; }

        // Properties selection checkboxes
        public UIMenuCheckboxItem AllPropertiesCheckbox { get; } = new UIMenuCheckboxItem("All Siren Properties", false, "Copy all properties for the selected sirens. Use individual checkboxes below to select specific properties to copy.");
        public UIMenuCheckboxItem SequencesCheckbox { get; } = new UIMenuCheckboxItem("Sequences", false, "Copy sequences and multiples properties on both flash and rotation");
        public UIMenuCheckboxItem DeltasCheckbox { get; } = new UIMenuCheckboxItem("Deltas", false, "Copy Delta properties on flash and rotation");
        public UIMenuCheckboxItem FlashinessCheckbox { get; } = new UIMenuCheckboxItem("Flashiness", false, "Copy flashiness properties ~y~except~w~ for sequences, multiples, and deltas");
        public UIMenuCheckboxItem RotationCheckbox { get; } = new UIMenuCheckboxItem("Rotation", false, "Copy rotation properties ~y~except~w~ for sequences, multiples, and deltas");
        public UIMenuCheckboxItem EnvLightingCheckbox { get; } = new UIMenuCheckboxItem("Siren Env Lighting", false, "Copy siren-specific environmental lighting properties ~y~except~w~ color");
        public UIMenuCheckboxItem CoronaCheckbox { get; } = new UIMenuCheckboxItem("Corona", false, "Copy all corona settings including intensity and size");
        public UIMenuCheckboxItem ColorCheckbox { get; } = new UIMenuCheckboxItem("Color", false, "Copy color setting for env lighting and corona");
        public UIMenuCheckboxItem SettingEnvCheckbox { get; } = new UIMenuCheckboxItem("Overall Env Lighting", false, "Copy overall environmental lighting settings ~y~for the entire SirenSetting~w~ (not siren-specific settings), including falloff and cone angle");
        private List<UIMenuCheckboxItem> allCopyCheckboxes = new List<UIMenuCheckboxItem>();


        // Copy details
        public UIMenuListItem CopyModeItem { get; } = new UIMenuListItem("Copy Mode", "Which siren setting to copy FROM and TO", new string[] { COPY_MODE_SELF, COPY_MODE_TO_TARGET, COPY_MODE_FROM_TARGET });
        public SirenSettingsSelectionMenu TargetMenu { get; }
        public UIMenuItem TargetMenuItem { get; }
        public UIMenuCustomListItem<int> SourceSirenSelector { get; } = new UIMenuCustomListItem<int>("Source Siren ID", "Select siren ID to copy from the source", Enumerable.Range(1, 20));
        public SirenIdMultiselectMenu DestinationSirenSelectorMenu { get; } = new SirenIdMultiselectMenu("Copy settings to {siren}. Multiselect enabled.");
        public UIMenuItem DestinationSirenMenuItem { get; } = new UIMenuItem("Destination Siren IDs", "Select siren IDs to copy to the destination");
        public UIMenuItem CopyItem { get; } = new UIMenuItem("~h~Copy~h~", "Copy the selected properties");

        /// <summary>
        /// Determines which EmergencyLighting to copy to and from
        /// </summary>
        /// <returns>Tuple(Source EmergencyLighting, Destination EmergencyLighting)</returns>
        public ValueTuple<EmergencyLighting, EmergencyLighting> GetCopySourceAndDestination()
        {
            switch(CopyMode)
            {
                case COPY_MODE_TO_TARGET:
                    return (this.ParentELS, this.TargetMenu.SelectedEmergencyLighting.ELS);
                case COPY_MODE_FROM_TARGET:
                    return (this.TargetMenu.SelectedEmergencyLighting.ELS, this.ParentELS);
                case COPY_MODE_SELF:
                default:
                    return (this.ParentELS, this.ParentELS);
            }
        }

        public int SourceSiren => SourceSirenSelector.Value;
        public int[] DestinationSirens => DestinationSirenSelectorMenu.SelectedSirenIDs.ToArray();

        private const string COPY_MODE_SELF = "Self";
        private const string COPY_MODE_TO_TARGET = "Copy to Target";
        private const string COPY_MODE_FROM_TARGET = "Copy from Target";
    }
}

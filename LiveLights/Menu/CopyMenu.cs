using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Menu
{
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Utils;
    using System.Drawing;

    internal class CopyMenu
    {
        public CopyMenu(EmergencyLightingMenu parent)
        {
            this.ParentMenu = parent;
            Menu = new UIMenu("Edit Siren", parent.Menu.Subtitle.Caption + " > Copy Siren Properties");
            DestinationSirenSelectorMenu.Menu.Subtitle.Caption = "~b~Copy Siren Properties > Select Destination Sirens";
            TargetMenu = new SirenSettingsSelectionMenu(ParentELS);

            Menu.AddItem(AllPropertiesCheckbox);
            Menu.AddItem(SequencesCheckbox);
            Menu.AddItem(CopyModeItem);
            TargetMenuItem = TargetMenu.CreateAndBindToSubmenuItem(this.Menu, "Target", "Select target siren setting to copy to/from", true);
            Menu.AddItem(SourceSirenSelector);
            Menu.AddItem(DestinationSirenMenuItem);
            Menu.BindMenuAndCopyProperties(DestinationSirenSelectorMenu.Menu, DestinationSirenMenuItem);
            DestinationSirenMenuItem.SetRightLabel("None →");
            Menu.AddItem(CopyItem);

            CopyModeItem.OnListChanged += OnCopyModeChanged;
            CopyItem.Activated += OnCopyActivated;

            OnCopyModeChanged(CopyModeItem, 0);
        }

        private void OnCopyActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            (EmergencyLighting source, EmergencyLighting destination) = GetCopySourceAndDestination();
            if(source.Exists() && destination.Exists() && destination.IsCustomSetting())
            {
                EmergencyLight sourceSiren = source.Lights[SourceSiren - 1];
                foreach (int sirenId in DestinationSirens)
                {
                    EmergencyLight destinationSiren = destination.Lights[sirenId - 1];

                    if (SequencesCheckbox.Checked)
                    {
                        destinationSiren.FlashinessSequence = sourceSiren.FlashinessSequence;
                        destinationSiren.RotationSequence = sourceSiren.RotationSequence;
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

        public UIMenuCheckboxItem AllPropertiesCheckbox { get; } = new UIMenuCheckboxItem("All Properties", false, "Copy all properties below");
        public UIMenuCheckboxItem SequencesCheckbox { get; } = new UIMenuCheckboxItem("Sequences", false, "Copy flash and rotation sequences");
        public UIMenuListItem CopyModeItem { get; } = new UIMenuListItem("Copy Mode", "Which siren setting to copy FROM and TO", new string[] { COPY_MODE_SELF, COPY_MODE_TO_TARGET, COPY_MODE_FROM_TARGET });
        public SirenSettingsSelectionMenu TargetMenu { get; }
        public UIMenuItem TargetMenuItem { get; }
        public UIMenuCustomListItem<int> SourceSirenSelector { get; } = new UIMenuCustomListItem<int>("Source Siren ID", "Select siren ID to copy from the source", Enumerable.Range(1, 20));
        // public UIMenuCustomListItem<int> DestinationSirenSelector { get; } = new UIMenuCustomListItem<int>("Destination Siren ID", "Select siren ID to copy to the destination", Enumerable.Range(1, 20));
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

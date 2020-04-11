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

            allSirenCopyCheckboxes.Add(SequencesCheckbox);
            allSirenCopyCheckboxes.Add(DeltasCheckbox);
            allSirenCopyCheckboxes.Add(FlashinessCheckbox);
            allSirenCopyCheckboxes.Add(RotationCheckbox);
            allSirenCopyCheckboxes.Add(EnvLightingCheckbox);
            allSirenCopyCheckboxes.Add(CoronaCheckbox);
            allSirenCopyCheckboxes.Add(ColorCheckbox);
            
            allGeneralCopyCheckboxes.Add(SettingEnvCheckbox);
            allGeneralCopyCheckboxes.Add(SettingSpeedCheckbox);
            allGeneralCopyCheckboxes.Add(HeadTailCheckbox);

            Menu.AddItem(AllSirenPropertiesCheckbox);
            foreach (UIMenuCheckboxItem checkbox in allSirenCopyCheckboxes)
            {
                checkbox.BackColor = Color.FromArgb(100, Color.DarkGray);
                checkbox.ForeColor = Color.WhiteSmoke;
                checkbox.Text = "    " + checkbox.Text;
                Menu.AddItem(checkbox);
            }

            Menu.AddItem(AllGeneralPropertiesCheckbox);
            foreach (UIMenuCheckboxItem checkbox in allGeneralCopyCheckboxes)
            {
                checkbox.BackColor = Color.FromArgb(100, Color.DarkGray);
                checkbox.ForeColor = Color.WhiteSmoke;
                checkbox.Text = "    " + checkbox.Text;
                Menu.AddItem(checkbox);
            }

            AllSirenPropertiesCheckbox.CheckboxEvent += OnAllPropertiesChecked;
            AllGeneralPropertiesCheckbox.CheckboxEvent += OnAllPropertiesChecked;


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
            List<UIMenuCheckboxItem> checkboxes = new List<UIMenuCheckboxItem>();
            if(sender == AllGeneralPropertiesCheckbox)
            {
                checkboxes = allGeneralCopyCheckboxes;
            } else if(sender == AllSirenPropertiesCheckbox)
            {
                checkboxes = allSirenCopyCheckboxes;
            }

            foreach (UIMenuCheckboxItem checkbox in checkboxes)
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
                
                bool sirenCopyAll = AllSirenPropertiesCheckbox.Checked;
                bool settingCopyAll = AllGeneralPropertiesCheckbox.Checked;

                Game.LogTrivial($"Initiating siren setting property copy from \"{source.Name}\" to \"{destination.Name}\"");

                foreach (int sirenId in DestinationSirens)
                {
                    // if SourceSiren is -1, signifies 1-to-1 copy with selected target sirens
                    // otherwise, use single selected source siren ID
                    EmergencyLight sourceSiren = source.Lights[((SourceSiren == -1) ? sirenId : SourceSiren) - 1];
                    EmergencyLight destinationSiren = destination.Lights[sirenId - 1];
                    Game.LogTrivial($"  SIREN {sirenId}:");

                    if(sirenCopyAll)
                    {
                        // In copy all mode, all siren-specific properties are copied for the selected sirens, no overall settings are copied 
                        Game.LogTrivial("    Copying all siren properties");
                        foreach (PropertyInfo property in sourceSiren.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                        {
                            try
                            {
                                property.SetValue(destinationSiren, property.GetValue(sourceSiren));
                                Game.LogTrivialDebug($"      {property.Name}");
                            } catch (TargetInvocationException e)
                            {
                                Game.LogTrivial($"      Unable to copy {property.Name}: {e.InnerException?.Message}");
                            }
                        } 
                    } else
                    {
                        if (SequencesCheckbox.Checked)
                        {
                            Game.LogTrivial("    Copying flashiness and rotation sequences and multiples");
                            destinationSiren.FlashinessSequence = sourceSiren.FlashinessSequence;
                            destinationSiren.RotationSequence = sourceSiren.RotationSequence;
                            destinationSiren.FlashinessMultiples = sourceSiren.FlashinessMultiples;
                            destinationSiren.RotationMultiples = sourceSiren.RotationMultiples;
                        }

                        if(DeltasCheckbox.Checked) 
                        {
                            Game.LogTrivial("    Copying flashiness and rotation deltas");
                            destinationSiren.FlashinessDelta = sourceSiren.FlashinessDelta;
                            destinationSiren.RotationDelta = sourceSiren.RotationDelta;
                        }

                        if(FlashinessCheckbox.Checked) 
                        {
                            // All flashiness settings except for sequences, multiples, deltas
                            Game.LogTrivial("    Copying flash toggle and flashiness direction, speed, start, sync to bpm, scale, and scale factor");
                            destinationSiren.Flash = sourceSiren.Flash;
                            destinationSiren.FlashinessDirection = sourceSiren.FlashinessDirection;
                            destinationSiren.FlashinessSpeed = sourceSiren.FlashinessSpeed;
                            destinationSiren.FlashinessStart = sourceSiren.FlashinessStart;
                            destinationSiren.FlashinessSynchronizeToBpm = sourceSiren.FlashinessSynchronizeToBpm;
                            destinationSiren.Scale = sourceSiren.Scale;
                            destinationSiren.ScaleFactor = sourceSiren.ScaleFactor;
                        }

                        if (RotationCheckbox.Checked)
                        {
                            // All rotation settings except for sequences, multiples, deltas
                            Game.LogTrivial("    Copying rotation toggle and rotation direction, speed, start, and sync to bpm");
                            destinationSiren.Rotate = sourceSiren.Rotate;
                            destinationSiren.RotationDirection = sourceSiren.RotationDirection;
                            destinationSiren.RotationSpeed = sourceSiren.RotationSpeed;
                            destinationSiren.RotationStart = sourceSiren.RotationStart;
                            destinationSiren.RotationSynchronizeToBpm = sourceSiren.RotationSynchronizeToBpm;
                        }

                        if(EnvLightingCheckbox.Checked) 
                        {
                            // All env lighting except color/corona
                            Game.LogTrivial("    Copying siren-specific environmental lighting");
                            destinationSiren.Light = sourceSiren.Light;
                            destinationSiren.Intensity = sourceSiren.Intensity;
                            destinationSiren.SpotLight = sourceSiren.SpotLight;
                            destinationSiren.CastShadows = sourceSiren.CastShadows;
                        }

                        if(CoronaCheckbox.Checked) 
                        {
                            // All corona settings except color
                            Game.LogTrivial("    Copying corona settings");
                            destinationSiren.CoronaFaceCamera = sourceSiren.CoronaFaceCamera;
                            destinationSiren.CoronaIntensity = sourceSiren.CoronaIntensity;
                            destinationSiren.CoronaPull = sourceSiren.CoronaPull;
                            destinationSiren.CoronaSize = sourceSiren.CoronaSize;
                        }

                        if(ColorCheckbox.Checked) 
                        {
                            Game.LogTrivial("    Copying siren color");
                            destinationSiren.Color = sourceSiren.Color;
                        }
                    }
                }

                Game.LogTrivial("  Copying general siren settings");
                if (settingCopyAll)
                {
                    Game.LogTrivial("    Copying all non-siren-specific settings");
                    foreach (PropertyInfo property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                    {
                        property.SetValue(destination, property.GetValue(source));
                        Game.LogTrivialDebug($"      {property.Name}");
                    }
                }
                else
                {
                    if (SettingEnvCheckbox.Checked)
                    {
                        // General env settings (not siren specific)
                        Game.LogTrivial("    Copying general environmental lighting settings including texture hash, falloff, cone angle, and real lights");
                        destination.TextureHash = source.TextureHash;
                        destination.LightFalloffMax = source.LightFalloffMax;
                        destination.LightFalloffExponent = source.LightFalloffExponent;
                        destination.LightInnerConeAngle = source.LightInnerConeAngle;
                        destination.LightOuterConeAngle = source.LightOuterConeAngle;
                        destination.UseRealLights = source.UseRealLights;
                    }
                    
                    if (SettingSpeedCheckbox.Checked)
                    {
                        // General bpm/multiplier 
                        Game.LogTrivial("    Copying BPM and time multiplier");
                        destination.SequencerBpm = source.SequencerBpm;
                        destination.TimeMultiplier = source.TimeMultiplier;
                    }

                    if (HeadTailCheckbox.Checked)
                    {
                        Game.LogTrivial("    Copying headlight and taillight sequences and multiples");
                        destination.LeftHeadLightMultiples = source.LeftHeadLightMultiples;
                        destination.RightHeadLightMultiples = source.RightHeadLightMultiples;
                        destination.LeftTailLightMultiples = source.LeftTailLightMultiples;
                        destination.RightTailLightMultiples = source.RightTailLightMultiples;

                        destination.LeftHeadLightSequence = source.LeftHeadLightSequence;
                        destination.RightHeadLightSequence = source.RightHeadLightSequence;
                        destination.LeftTailLightSequence = source.LeftTailLightSequence;
                        destination.RightTailLightSequence = source.RightTailLightSequence;
                    }
                }

                // After copying, refresh menus to reflect potentially updated data
                ParentMenu.Menu.RefreshData();
                string info = $"FROM: ~g~{source.Name}~w~";
                if(SourceSiren != -1) info += $", Siren ~b~{SourceSiren}";
                info += $"~w~\n\nTO: ~g~{destination.Name}~w~, Sirens ~b~";
                info += string.Join(", ", DestinationSirens);
                info += "~w~.\n\nCheck log/console for additional details.";
                Game.DisplayNotification("desktop_pc", "folder", "Copied Siren Settings", "", info);
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

        internal void ProcessShowSirens(Vehicle v)
        {
            if(Menu.Visible && SourceSirenSelector.Selected)
            {
                v.ShowSirenMarker(SourceSirenSelector.Value);
            } else if(DestinationSirenSelectorMenu.Menu.Visible)
            {
                foreach (int sirenId in DestinationSirenSelectorMenu.SelectedSirenIDs)
                {
                    v.ShowSirenMarker(sirenId, 0.12f);
                }
                v.ShowSirenMarker(DestinationSirenSelectorMenu.GetHighlightedSirenId(), new Vector3(0.05f, 0.05f, 0.3f), MarkerStyle.MarkerTypeVerticalCylinder, 0f);
            }
        }

        private string CopyMode => (string)(CopyModeItem.SelectedValue);

        public EmergencyLightingMenu ParentMenu { get; }
        public EmergencyLighting ParentELS => ParentMenu.ELS;
        public UIMenu Menu { get; }

        // Overall properties selection checkboxes
        public UIMenuCheckboxItem AllGeneralPropertiesCheckbox { get; } = new UIMenuCheckboxItem("All General Properties", false, "Copy all overall properties from the source to the destination siren setting. Does not copy any individual siren properties, only overall properties.");
        public UIMenuCheckboxItem SettingEnvCheckbox { get; } = new UIMenuCheckboxItem("Overall Env Lighting", false, "Copy overall environmental lighting settings ~y~for the entire SirenSetting~w~ (not siren-specific settings), including falloff and cone angle");
        public UIMenuCheckboxItem SettingSpeedCheckbox { get; } = new UIMenuCheckboxItem("BPM and Multiplier", false, "Copy BPM and Multiplier for the entire siren setting");
        public UIMenuCheckboxItem HeadTailCheckbox { get; } = new UIMenuCheckboxItem("Head/Tail Light Settings", false, "Copy sequences and multipliers for headlights and taillights");

        // Siren-specific properties selection checkboxes
        public UIMenuCheckboxItem AllSirenPropertiesCheckbox { get; } = new UIMenuCheckboxItem("All Siren Properties", false, "Copy all properties for the selected sirens. Use individual checkboxes below to select specific properties to copy.");
        public UIMenuCheckboxItem SequencesCheckbox { get; } = new UIMenuCheckboxItem("Sequences", false, "Copy sequences and multiples properties on both flash and rotation");
        public UIMenuCheckboxItem DeltasCheckbox { get; } = new UIMenuCheckboxItem("Deltas", false, "Copy Delta properties on flash and rotation");
        public UIMenuCheckboxItem FlashinessCheckbox { get; } = new UIMenuCheckboxItem("Flashiness", false, "Copy flashiness properties ~y~except~w~ for sequences, multiples, and deltas");
        public UIMenuCheckboxItem RotationCheckbox { get; } = new UIMenuCheckboxItem("Rotation", false, "Copy rotation properties ~y~except~w~ for sequences, multiples, and deltas");
        public UIMenuCheckboxItem EnvLightingCheckbox { get; } = new UIMenuCheckboxItem("Siren Env Lighting", false, "Copy siren-specific environmental lighting properties ~y~except~w~ color");
        public UIMenuCheckboxItem CoronaCheckbox { get; } = new UIMenuCheckboxItem("Corona", false, "Copy all corona settings including intensity and size");
        public UIMenuCheckboxItem ColorCheckbox { get; } = new UIMenuCheckboxItem("Color", false, "Copy color setting for env lighting and corona");
        
        private List<UIMenuCheckboxItem> allSirenCopyCheckboxes = new List<UIMenuCheckboxItem>();
        private List<UIMenuCheckboxItem> allGeneralCopyCheckboxes = new List<UIMenuCheckboxItem>();


        // Copy details
        public UIMenuListItem CopyModeItem { get; } = new UIMenuListItem("Copy Mode", "Which siren setting to copy FROM and TO", new string[] { COPY_MODE_SELF, COPY_MODE_TO_TARGET, COPY_MODE_FROM_TARGET });
        public SirenSettingsSelectionMenu TargetMenu { get; }
        public UIMenuItem TargetMenuItem { get; }
        public UIMenuCustomListItem<int> SourceSirenSelector { get; } = new UIMenuCustomListItem<int>("Source Siren ID", "Select siren ID to copy from the source. Choose ~b~1-to-1~w~ to copy settings from multiple source sirens to the matching destination siren IDs selected below. For example, if you select destination sirens 6 & 9 with \"1-to-1\" mode, properties from source siren 6 will be copied to destination siren 6, and source siren 9 to destination siren 9.", CommonSelectionItems.SirensOrAll);
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
                    return (this.ParentELS, this.TargetMenu.SelectedEmergencyLighting);
                case COPY_MODE_FROM_TARGET:
                    return (this.TargetMenu.SelectedEmergencyLighting, this.ParentELS);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System.Drawing;

namespace LiveLights
{
    using Rage;
    using Utils;

    internal static class SirenApply
    {
        public static void ApplySirenSettingsToEmergencyLighting(this SirenSetting setting, EmergencyLighting els, bool clearExcessSirens = true)
        {
            string name = setting.Name;
            for (int iName = 1; EmergencyLighting.GetByName(name).Exists(); iName++)
            {
                name = $"{setting.Name} ({iName})";
            }
            

            els.Name = name;
            els.TimeMultiplier = setting.TimeMultiplier;
            els.LightFalloffMax = setting.LightFalloffMax;
            els.LightFalloffExponent = setting.LightFalloffExponent;
            els.LightInnerConeAngle = setting.LightInnerConeAngle;
            els.LightOuterConeAngle = setting.LightOuterConeAngle;
            els.LightOffset = setting.LightOffset;
            els.TextureHash = setting.TextureHash;
            els.SequencerBpm = setting.SequencerBPM;
            els.UseRealLights = setting.UseRealLights;
            els.LeftHeadLightSequenceRaw = setting.LeftHeadLightSequencer;
            els.LeftHeadLightMultiples = setting.LeftHeadLightMultiples;
            els.RightHeadLightSequenceRaw = setting.RightHeadLightSequencer;
            els.RightHeadLightMultiples = setting.RightHeadLightMultiples;
            els.LeftTailLightSequenceRaw = setting.LeftTailLightSequencer;
            els.LeftTailLightMultiples = setting.LeftTailLightMultiples;
            els.RightTailLightSequenceRaw = setting.RightTailLightSequencer;
            els.RightTailLightMultiples = setting.RightTailLightMultiples;

            for (int i = 0; i < els.Lights.Length; i++)
            {
                EmergencyLight light = els.Lights[i];

                if (i < setting.Sirens.Length)
                {
                    SirenEntry entry = setting.Sirens[i];    

                    // Main light settings
                    light.Color = entry.LightColor;
                    light.Intensity = entry.Intensity;
                    light.LightGroup = entry.LightGroup;
                    light.Rotate = entry.Rotate;
                    light.Scale = entry.Scale;
                    light.ScaleFactor = entry.ScaleFactor;
                    light.Flash = entry.Flash;
                    light.SpotLight = entry.SpotLight;
                    light.CastShadows = entry.CastShadows;
                    light.Light = entry.Light;

                    // Corona settings
                    light.CoronaIntensity = entry.Corona.CoronaIntensity;
                    light.CoronaSize = entry.Corona.CoronaSize;
                    light.CoronaPull = entry.Corona.CoronaPull;
                    light.CoronaFaceCamera = entry.Corona.CoronaFaceCamera;

                    // Rotation settings
                    light.RotationDelta = entry.Rotation.DeltaDeg;
                    light.RotationStart = entry.Rotation.StartDeg;
                    light.RotationSpeed = entry.Rotation.Speed;
                    light.RotationSequenceRaw = entry.Rotation.Sequence;
                    light.RotationMultiples = entry.Rotation.Multiples;
                    light.RotationDirection = entry.Rotation.Direction;
                    light.RotationSynchronizeToBpm = entry.Rotation.SyncToBPM;

                    // Flash settings
                    light.FlashinessDelta = entry.Flashiness.DeltaDeg;
                    light.FlashinessStart = entry.Flashiness.StartDeg;
                    light.FlashinessSpeed = entry.Flashiness.Speed;
                    light.FlashinessSequenceRaw = entry.Flashiness.Sequence;
                    light.FlashinessMultiples = entry.Flashiness.Multiples;
                    light.FlashinessDirection = entry.Flashiness.Direction;
                    light.FlashinessSynchronizeToBpm = entry.Flashiness.SyncToBPM;
                } else if (clearExcessSirens)
                {
                    // Main light settings
                    light.Color = Color.Black;
                    light.Intensity = 0;
                    light.LightGroup = 0;
                    light.Rotate = false;
                    light.Scale = false;
                    light.ScaleFactor = 0;
                    light.Flash = false;
                    light.SpotLight = false;
                    light.CastShadows = false;
                    light.Light = false;

                    // Corona settings
                    light.CoronaIntensity = 0;
                    light.CoronaSize = 0;
                    light.CoronaPull = 0;
                    light.CoronaFaceCamera = false;

                    // Rotation settings
                    light.RotationDelta = 0;
                    light.RotationStart = 0;
                    light.RotationSpeed = 0;
                    light.RotationSequenceRaw = 0;
                    light.RotationMultiples = 0;
                    light.RotationDirection = false;
                    light.RotationSynchronizeToBpm = false;

                    // Flash settings
                    light.FlashinessDelta = 0;
                    light.FlashinessStart = 0;
                    light.FlashinessSpeed = 0;
                    light.FlashinessSequenceRaw = 0;
                    light.FlashinessMultiples = 0;
                    light.FlashinessDirection = false;
                    light.FlashinessSynchronizeToBpm = false;
                }
            }
        }

        public static void ExportEmergencyLightingToSirenSettings(this EmergencyLighting els, ref SirenSetting setting, int? maxToExport = null)
        {
            setting.Name = els.Name;
            setting.TimeMultiplier = els.TimeMultiplier;
            setting.LightFalloffMax = els.LightFalloffMax;
            setting.LightFalloffExponent = els.LightFalloffExponent;
            setting.LightInnerConeAngle = els.LightInnerConeAngle;
            setting.LightOuterConeAngle = els.LightOuterConeAngle;
            setting.LightOffset = els.LightOffset;
            setting.TextureHash = els.TextureHash;
            setting.SequencerBPM = els.SequencerBpm;
            setting.UseRealLights = els.UseRealLights;
            setting.LeftHeadLightSequencer = els.LeftHeadLightSequenceRaw;
            setting.LeftHeadLightMultiples = els.LeftHeadLightMultiples;
            setting.RightHeadLightSequencer = els.RightHeadLightSequenceRaw;
            setting.RightHeadLightMultiples = els.RightHeadLightMultiples;
            setting.LeftTailLightSequencer = els.LeftTailLightSequenceRaw;
            setting.LeftTailLightMultiples = els.LeftTailLightMultiples;
            setting.RightTailLightSequencer = els.RightTailLightSequenceRaw;
            setting.RightTailLightMultiples = els.RightTailLightMultiples;

            // if a max is defined, export up to the max or the total available lights
            // if a max is not defined, export all available lights
            maxToExport = Math.Min(maxToExport ?? els.Lights.Length, els.Lights.Length);

            for (int i = 0; i < maxToExport; i++)
            {
                if (maxToExport.HasValue && i >= maxToExport.Value) break;

                SirenEntry entry = new SirenEntry();
                EmergencyLight light = els.Lights[i];

                // Main light settings
                entry.LightColor = light.Color;
                entry.Intensity = light.Intensity;
                entry.LightGroup = light.LightGroup;
                entry.Rotate = light.Rotate;
                entry.Scale = light.Scale;
                entry.ScaleFactor = light.ScaleFactor;
                entry.Flash = light.Flash;
                entry.SpotLight = light.SpotLight;
                entry.CastShadows = light.CastShadows;
                entry.Light = light.Light;

                // Corona settings
                entry.Corona.CoronaIntensity = light.CoronaIntensity;
                entry.Corona.CoronaSize = light.CoronaSize;
                entry.Corona.CoronaPull = light.CoronaPull;
                entry.Corona.CoronaFaceCamera = light.CoronaFaceCamera;

                // Rotation settings
                entry.Rotation.DeltaDeg = light.RotationDelta;
                entry.Rotation.StartDeg = light.RotationStart;
                entry.Rotation.Speed = light.RotationSpeed;
                entry.Rotation.Sequence = light.RotationSequenceRaw;
                entry.Rotation.Multiples = light.RotationMultiples;
                entry.Rotation.Direction = light.RotationDirection;
                entry.Rotation.SyncToBPM = light.RotationSynchronizeToBpm;

                // Flash settings
                entry.Flashiness.DeltaDeg = light.FlashinessDelta;
                entry.Flashiness.StartDeg = light.FlashinessStart;
                entry.Flashiness.Speed = light.FlashinessSpeed;
                entry.Flashiness.Sequence = light.FlashinessSequenceRaw;
                entry.Flashiness.Multiples = light.FlashinessMultiples;
                entry.Flashiness.Direction = light.FlashinessDirection;
                entry.Flashiness.SyncToBPM = light.FlashinessSynchronizeToBpm;

                setting.AddSiren(entry);
            }
        }

        public static SirenSetting ExportEmergencyLightingToSirenSettings(this EmergencyLighting els, int? maxToExport = null)
        {
            SirenSetting s = new SirenSetting();
            els.ExportEmergencyLightingToSirenSettings(ref s, maxToExport);
            return s;
        }

        public static EmergencyLighting GetELSForVehicle(this Vehicle v)
        {
            if (!v) return null;

            EmergencyLighting els = v.EmergencyLightingOverride;
            if (!els.Exists())
            {
                v.EmergencyLightingOverride = v.DefaultEmergencyLighting.CloneWithID();
                els = v.EmergencyLightingOverride;
                Game.LogTrivial("Cloned default ELS");
            }

            return els;
        }
    }
}

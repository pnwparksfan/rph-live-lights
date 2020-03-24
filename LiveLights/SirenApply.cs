using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace LiveLights
{
    using Rage;

    internal static class SirenApply
    {
        public static void ApplySirenSettingsToEmergencyLighting(this SirenSetting setting, EmergencyLighting els)
        {
            els.Name = setting.Name;
            els.TimeMultiplier = setting.TimeMultiplier;
            els.LightFalloffMax = setting.LightFalloffMax;
            els.LightFalloffExponent = setting.LightFalloffExponent;
            els.LightInnerConeAngle = setting.LightInnerConeAngle;
            els.LightOuterConeAngle = setting.LightOuterConeAngle;
            els.LightOffset = setting.LightOffset;
            els.TextureHash = setting.TextureHash;
            els.SequencerBpm = setting.SequencerBPM;
            els.UseRealLights = setting.UseRealLights;
            els.LeftHeadLightSequence = setting.LeftHeadLightSequencer;
            els.LeftHeadLightMultiples = setting.LeftHeadLightMultiples;
            els.RightHeadLightSequence = setting.RightHeadLightSequencer;
            els.RightHeadLightMultiples = setting.RightHeadLightMultiples;
            els.LeftTailLightSequence = setting.LeftTailLightSequencer;
            els.LeftTailLightMultiples = setting.LeftTailLightMultiples;
            els.RightTailLightSequence = setting.RightTailLightSequencer;
            els.RightTailLightMultiples = setting.RightTailLightMultiples;

            for (int i = 0; i < setting.Sirens.Length; i++)
            {
                SirenEntry entry = setting.Sirens[i];
                EmergencyLight light = els.Lights[i];

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

                // Corona settings
                light.CoronaIntensity = entry.Corona.CoronaIntensity;
                light.CoronaSize = entry.Corona.CoronaSize;
                light.CoronaPull = entry.Corona.CoronaPull;
                light.CoronaFaceCamera = entry.Corona.CoronaFaceCamera;

                // Rotation settings
                light.RotationDelta = entry.Rotation.DeltaRad;
                light.RotationStart = entry.Rotation.DeltaRad;
                light.RotationSpeed = entry.Rotation.Speed;
                light.RotationSequence = entry.Rotation.Sequence;
                light.RotationMultiples = entry.Rotation.Multiples;
                light.RotationDirection = entry.Rotation.Direction;
                light.RotationSynchronizeToBpm = entry.Rotation.SyncToBPM;

                // Flash settings
                light.FlashinessDelta = entry.Flashiness.DeltaDeg;
                light.FlashinessStart = entry.Flashiness.StartDeg;
                light.FlashinessSpeed = entry.Flashiness.Speed;
                light.FlashinessSequence = entry.Flashiness.Sequence;
                light.FlashinessMultiples = entry.Flashiness.Multiples;
                light.FlashinessDirection = entry.Flashiness.Direction;
                light.FlashinessSynchronizeToBpm = entry.Flashiness.SyncToBPM;
            }
        }

        public static void ExportEmergencyLightingToSirenSettings(this EmergencyLighting els, ref SirenSetting setting)
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
            setting.LeftHeadLightSequencer = els.LeftHeadLightSequence;
            setting.LeftHeadLightMultiples = els.LeftHeadLightMultiples;
            setting.RightHeadLightSequencer = els.RightHeadLightSequence;
            setting.RightHeadLightMultiples = els.RightHeadLightMultiples;
            setting.LeftTailLightSequencer = els.LeftTailLightSequence;
            setting.LeftTailLightMultiples = els.LeftTailLightMultiples;
            setting.RightTailLightSequencer = els.RightTailLightSequence;
            setting.RightTailLightMultiples = els.RightTailLightMultiples;

            for (int i = 0; i < els.Lights.Length; i++)
            {
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

                // Corona settings
                entry.Corona.CoronaIntensity = light.CoronaIntensity;
                entry.Corona.CoronaSize = light.CoronaSize;
                entry.Corona.CoronaPull = light.CoronaPull;
                entry.Corona.CoronaFaceCamera = light.CoronaFaceCamera;

                // Rotation settings
                entry.Rotation.DeltaRad = light.RotationDelta;
                entry.Rotation.DeltaRad = light.RotationStart;
                entry.Rotation.Speed = light.RotationSpeed;
                entry.Rotation.Sequence = light.RotationSequence;
                entry.Rotation.Multiples = light.RotationMultiples;
                entry.Rotation.Direction = light.RotationDirection;
                entry.Rotation.SyncToBPM = light.RotationSynchronizeToBpm;

                // Flash settings
                entry.Flashiness.DeltaDeg = light.FlashinessDelta;
                entry.Flashiness.StartDeg = light.FlashinessStart;
                entry.Flashiness.Speed = light.FlashinessSpeed;
                entry.Flashiness.Sequence = light.FlashinessSequence;
                entry.Flashiness.Multiples = light.FlashinessMultiples;
                entry.Flashiness.Direction = light.FlashinessDirection;
                entry.Flashiness.SyncToBPM = light.FlashinessSynchronizeToBpm;

                setting.AddSiren(entry);
            }
        }

        public static SirenSetting ExportEmergencyLightingToSirenSettings(this EmergencyLighting els)
        {
            SirenSetting s = new SirenSetting();
            els.ExportEmergencyLightingToSirenSettings(ref s);
            return s;
        }

        public static EmergencyLighting GetELSForVehicle(this Vehicle v)
        {
            if (!v) return null;

            EmergencyLighting els = v.EmergencyLightingOverride;
            if (!els.Exists())
            {
                v.EmergencyLightingOverride = v.DefaultEmergencyLighting.Clone();
                els = v.EmergencyLightingOverride;
                Game.LogTrivial("Cloned default ELS");
            }

            return els;
        }
    }
}

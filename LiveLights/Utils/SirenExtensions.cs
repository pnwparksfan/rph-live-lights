﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LiveLights.Utils
{
    using Rage;
    using Rage.Native;
    
    internal static class SirenExtensions
    {
        public static void ShowSirenMarker(this Vehicle vehicle, int siren, float size = 0.2f, MarkerStyle style = MarkerStyle.MarkerTypeUpsideDownCone)
        {
            ShowSirenMarker(vehicle, siren, new Vector3(size, size, size), style);
        }

        public static void ShowSirenMarker(this Vehicle vehicle, int siren, Vector3 scale, MarkerStyle style = MarkerStyle.MarkerTypeUpsideDownCone)
        {
            string boneName = $"siren{siren}";
            if (vehicle && vehicle.HasBone(boneName) && vehicle.EmergencyLighting.Exists())
            {
                EmergencyLight light = vehicle.EmergencyLighting.Lights[siren - 1];
                Vector3 bonePosition = vehicle.GetBonePosition(boneName);
                Vector3 markerPosition = vehicle.GetOffsetPosition(vehicle.GetPositionOffset(bonePosition) + 0.6f * scale.Z * Vector3.WorldUp);
                Vector3 direction = vehicle.GetBoneOrientation(boneName).ToVector();
                Marker.DrawMarker(style, markerPosition, vehicle.UpVector, Rotator.Zero, scale, light.Color);
                // Debug.DrawLine(bonePosition, bonePosition + direction * 0.3f, light.Color);
            }
        }

        public static bool IsCustomSetting(this EmergencyLighting els)
        {
            // return EmergencyLighting.Get(false, true).Contains(els);
            return EmergencyLighting.Get(false, true).Any(l => l.Name == els.Name);
        }

        public static EmergencyLighting GetCustomOrClone(this EmergencyLighting els)
        {
            if(els.IsCustomSetting())
            {
                return els;
            } else
            {
                return els.Clone();
            }
        }

        public static EmergencyLighting GetOrCreateOverrideEmergencyLighting(this Vehicle vehicle)
        {
            if (!vehicle.EmergencyLightingOverride.Exists())
            {
                vehicle.EmergencyLightingOverride = vehicle.DefaultEmergencyLighting.Clone();
            }

            return vehicle.EmergencyLightingOverride;
        }
    }
}
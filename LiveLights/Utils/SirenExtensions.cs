using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

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

        public static void ShowSirenMarker(this Vehicle vehicle, int siren, Vector3 scale, MarkerStyle style = MarkerStyle.MarkerTypeUpsideDownCone, float verticalOffset = 0.6f)
        {
            string boneName = $"siren{siren}";
            if (vehicle && vehicle.HasBone(boneName) && vehicle.EmergencyLighting.Exists() && siren <= vehicle.EmergencyLighting.Lights.Length)
            {
                EmergencyLight light = vehicle.EmergencyLighting.Lights[siren - 1];
                Vector3 bonePosition = vehicle.GetBonePosition(boneName);
                Vector3 markerPosition = vehicle.GetOffsetPosition(vehicle.GetPositionOffset(bonePosition) + verticalOffset * scale.Z * Vector3.WorldUp);
                Vector3 direction = vehicle.GetBoneOrientation(boneName).ToVector();
                Marker.DrawMarker(style, markerPosition, vehicle.UpVector, Rotator.Zero, scale, light.Color);
                // Debug.DrawLine(bonePosition, bonePosition + direction * 0.3f, light.Color);
            }
        }

        public static bool HasSiren(this Vehicle vehicle, int sirenNum) => vehicle.HasBone($"siren{sirenNum}");

        public static bool IsCustomSetting(this EmergencyLighting els)
        {
            return els.Id == uint.MaxValue;
        }

        public static EmergencyLighting GetCustomOrClone(this EmergencyLighting els)
        {
            if(els.IsCustomSetting())
            {
                return els;
            } else
            {
                return els.CloneWithID();
            }
        }

        public static EmergencyLighting GetOrCreateOverrideEmergencyLighting(this Vehicle vehicle)
        {
            if (!vehicle.EmergencyLightingOverride.Exists())
            {
                vehicle.EmergencyLightingOverride = vehicle.DefaultEmergencyLighting.CloneWithID();
            }

            return vehicle.EmergencyLightingOverride;
        }

        public static void RefreshSiren(this Vehicle vehicle)
        {
            if (vehicle)
            {
                if (vehicle.IsSirenOn)
                {
                    GameFiber.StartNew(() =>
                    {
                        if (!vehicle) return;
                        vehicle.IsSirenOn = false;
                        GameFiber.Sleep(5);
                        if (!vehicle) return;
                        vehicle.IsSirenOn = true;
                    });
                }
            }
        }
    }
}

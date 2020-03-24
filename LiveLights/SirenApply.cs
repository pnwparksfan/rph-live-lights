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

#if false
    internal class PropertyMapping<Ta, Tb>
    {
        // public Func<T, T> AtoB;
        // public Func<T, T> BtoA;
        private Action<Ta, object> a2b;
        private Action<Tb, object> b2a;

        public void AtoB(Ta A, Tb B)
        {

        }

        /*
        public PropertyMapping(Func<Ta, object> lambdaA, Func<Tb, object> lambdaB) 
        {
            this.lambdaA = lambdaA;
            this.lambdaB = lambdaB;
        }
        */

        public PropertyMapping(Action<Ta, Tb> a2b, Action<Tb, Ta> b2a)
        {
            this.a2b = a2b;
            this.b2a = b2a;
        }

        private static void test()
        {
            PropertyMapping<EmergencyLighting, SirenSetting> foo = 
                new PropertyMapping<EmergencyLighting, SirenSetting>((A, B) => { A.Name = B.Name; }, (B, A) => { B.Name = A.Name; });

            List<PropertyMapping<EmergencyLighting, SirenSetting>> stuff = new List<PropertyMapping<EmergencyLighting, SirenSetting>>()
            {
                new PropertyMapping<EmergencyLighting, SirenSetting>(x => x.Name, y => y.Name),
                new PropertyMapping<EmergencyLighting, SirenSetting>(x => x.SequencerBpm, y => y.SequencerBPM)
            };

            SirenSetting s = new SirenSetting();
            EmergencyLighting e = Game.LocalPlayer.Character.CurrentVehicle.GetELSForVehicle();
            foo.a2b(e, s);

        }
    }
#endif 

    internal static class SirenApply
    {
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> lambda)
        {
            var member = lambda.Body as MemberExpression;
            return member.Member as PropertyInfo;
        }

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
                light.RotationDelta = entry.Rotation.DeltaDeg;
                light.RotationStart = entry.Rotation.StartDeg;
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

            /*
            var sirenSettingProperties = typeof(SirenSetting).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            // var emergencyLightingProperties = typeof(EmergencyLighting).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var settingProp in sirenSettingProperties)
            {
                Game.LogTrivial($"{settingProp.Name}<{settingProp.PropertyType.Name}>");
                PropertyInfo elpProp = typeof(EmergencyLighting).GetProperty(settingProp.Name);
                if (elpProp == null)
                {
                    Game.LogTrivial("  No matching property");
                }
                else if (elpProp.PropertyType.IsAssignableFrom(settingProp.PropertyType))
                {
                    Game.LogTrivial($"  Matching assignable property: {elpProp.Name}<{elpProp.PropertyType.Name}>");
                } else
                {
                    object x = settingProp.GetValue(settings);

                    Type t = settingProp.PropertyType;
                    // var q = x as ValueItem<typeof(t) >;
                    Type t2 = typeof(ValueItem<>).MakeGenericType(t);
                    var q = x as typeof(t2);

                    Game.LogTrivial($"  Non-assignable property: {elpProp.Name}<{elpProp.PropertyType.Name}>");
                    elpProp.SetValue(els, settingProp.GetValue(settings));
                }  
            }
            */
        }

        public static void DoThingToStuff<T>(Action<T> foo)
        {

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

        [Rage.Attributes.ConsoleCommand]
        public static void TestPropertyAccess(string newName)
        {
            var info = GetPropertyInfo<EmergencyLighting>(l => l.LightFalloffExponent);

            Vehicle v = Game.LocalPlayer.Character.LastVehicle;
            /*
            if (!v) return;

            EmergencyLighting els = v.EmergencyLightingOverride;
            if(!els.Exists())
            {
                v.EmergencyLightingOverride = v.DefaultEmergencyLighting.Clone();
                els = v.EmergencyLightingOverride;
                Game.LogTrivial("Cloned default ELS");
            }
            */

            EmergencyLighting els = v.GetELSForVehicle();
            if (!v.Exists()) return;

            Game.LogTrivial("Got ELS: " + els.Name);

            typeof(EmergencyLighting).GetProperty("Name").SetValue(els, newName);

            Game.LogTrivial("ELS updated name: " + els.Name);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SirenPropertyAttribute : Attribute
    {
        public string PropertyName { get; }
        public bool PrefixOnly { get; }

        public SirenPropertyAttribute(Type T, string propertyName)
        {
            this.PropertyName = propertyName;
            this.PrefixOnly = false;
        }

        public SirenPropertyAttribute(Type T, string propertyName, bool prefix)
        {
            this.PropertyName = propertyName;
            this.PrefixOnly = prefix;
        }

    }
}

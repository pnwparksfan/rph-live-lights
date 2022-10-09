using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace LiveLights.Utils
{
    internal enum EmergencyLightingSource
    {
        BuiltIn,
        Cloned,
        Imported,
        Manual
    }

    internal class SirenSource
    {
        public uint SourceId { get; set; }
        public EmergencyLightingSource Source { get; set; }

        public string SourceDescription { 
            get
            {
                switch (Source)
                {
                    case EmergencyLightingSource.BuiltIn:
                        return "Built In";
                    case EmergencyLightingSource.Cloned:
                        return "Cloned";
                    case EmergencyLightingSource.Imported:
                        return "Imported";
                    case EmergencyLightingSource.Manual:
                        return "Manaully Set";
                    default:
                        return "Copied";
                }
            } 
        }

        private SirenSource(uint sourceId, EmergencyLightingSource source)
        {
            SourceId = sourceId;
            Source = source;
        }

        private static Dictionary<EmergencyLighting, SirenSource> sources = new Dictionary<EmergencyLighting, SirenSource>();

        internal static void SetSource(EmergencyLighting els, uint srcId, EmergencyLightingSource src)
        {
            if (srcId == uint.MaxValue) return;

            if (!sources.TryGetValue(els, out SirenSource source))
            {
                source = new SirenSource(srcId, src);
                sources.Add(els, source);
            }
            else
            {
                source.SourceId = srcId;
                source.Source = src;
            }
        }

        internal static SirenSource GetSource(EmergencyLighting els)
        {
            if (sources.TryGetValue(els, out SirenSource source))
            {
                return source;
            }
            else if (!els.IsCustomSetting())
            {
                return new SirenSource(els.SirenSettingID(), EmergencyLightingSource.BuiltIn);
            }

            return null;
        }
    }

    internal static class SirenSourceExtensions
    {

        // this abomination is required because the first instance returned has a different hashcode from 
        // any subsequent instances returned by Get[ByName], Vehicle.EmergencyLighting, etc. 
        // hopefully will be fixed soon
        public static EmergencyLighting GetSafeInstance(this EmergencyLighting els) => EmergencyLighting.GetByName(els.Name);

        public static EmergencyLighting CloneWithID(this EmergencyLighting source)
        {
            uint srcId = source.SirenSettingID();
            var clone = source.Clone().GetSafeInstance();
            SirenSource.SetSource(clone, srcId, EmergencyLightingSource.Cloned);
            return clone;
        }

        public static void SetSource(this EmergencyLighting els, uint srcId, EmergencyLightingSource src) => SirenSource.SetSource(els, srcId, src);

        public static SirenSource GetSource(this EmergencyLighting els) => SirenSource.GetSource(els);
    }
}

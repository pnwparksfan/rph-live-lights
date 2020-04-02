using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    using Rage;

    internal class EmergencyLightingWrapper
    {
        public static implicit operator EmergencyLighting(EmergencyLightingWrapper w) => w.ELS;
        public static implicit operator EmergencyLightingWrapper(EmergencyLighting e) => new EmergencyLightingWrapper(e);

        public EmergencyLighting ELS { get; }

        public EmergencyLightingWrapper(EmergencyLighting els)
        {
            this.ELS = els;
        }

        public override int GetHashCode()
        {
            return this.ELS?.Name.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType()) 
            {
                return false;
            }

            EmergencyLightingWrapper objW = ((EmergencyLightingWrapper)obj);
            return objW.ELS.Exists() && this.ELS.Exists() && objW.ELS.Name == this.ELS.Name;
        }
    }
}

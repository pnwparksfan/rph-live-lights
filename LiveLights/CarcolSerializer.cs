using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights
{
    using Rage;

    public static class CarcolSerializer
    {
        public static void ToCarcolEntry(this EmergencyLighting els, uint id = 0)
        {
            SirensItem sirensetting = new SirensItem();
            sirensetting.id.value = id;
            sirensetting.name = els.Name;
        }
    }
}

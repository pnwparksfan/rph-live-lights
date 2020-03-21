using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using Rage.Attributes;


[assembly: Rage.Attributes.Plugin("Live Carcols and Siren Editor", Description = "Enable/disable emergency lights, configure siren settings, and create custom carcols entries", Author = "PNWParksFan")]

namespace LiveLights
{
    public class EntryPoint
    {
        private static void Main()
        {
            GameFiber.Hibernate();
        }
    }
}

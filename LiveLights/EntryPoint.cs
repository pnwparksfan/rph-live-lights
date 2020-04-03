using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
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
            AssemblyName pluginInfo = Assembly.GetExecutingAssembly().GetName();
            Game.LogTrivial($"Loaded {pluginInfo.Name} {pluginInfo.Version}");
            if(Settings.MenuKey != Keys.None)
            {
                Game.LogTrivial("Press " + (Settings.MenuModifier == System.Windows.Forms.Keys.None ? "" : (Settings.MenuModifier.ToString() + " + ")) + Settings.MenuKey.ToString() + " to open the menu");
            } else
            {
                Game.LogTrivial("Use the OpenLiveLightsMenu console command to open the menu");
            }
            
            GameFiber.ExecuteWhile(Menu.MenuController.Process, () => true);
            GameFiber.Hibernate();
        }
    }
}

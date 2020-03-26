using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Menu
{
    using Rage;
    using Rage.Attributes;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    using Menu;

    internal static class MenuController
    {
        internal static MenuPool Pool = new MenuPool();

        internal static void Process()
        {
            Pool.ProcessMenus();
        }

        [ConsoleCommand]
        private static void TestMenu()
        {
            Vehicle v = Game.LocalPlayer.Character.CurrentVehicle;
            EmergencyLighting els = v.GetELSForVehicle();
            EmergencyLightingMenu menu = new EmergencyLightingMenu(els);
            menu.Menu.Visible = true;

            GameFiber.ExecuteNewWhile(Process, () => menu.Menu.Visible);
        }
    }
}

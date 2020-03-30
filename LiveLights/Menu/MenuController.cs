using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            if(Game.IsKeyDown(Keys.Multiply))
            {
                if(!menu.Menu.Visible)
                {
                    menu.SirenConfigMenu?.Menu.RefreshData();
                }
                menu.Menu.Visible = !menu.Menu.Visible;
            }

            menu.SirenConfigMenu?.ShowSirenPositions(Game.LocalPlayer.Character.CurrentVehicle, true);

            Pool.ProcessMenus();
        }

        // private static EmergencyLightingMenu menu;
        private static VehicleMenu menu;

        [ConsoleCommand]
        private static void StartMenu()
        {
            Vehicle v = Game.LocalPlayer.Character.CurrentVehicle;
            EmergencyLighting els = v.GetELSForVehicle();
            // menu = new EmergencyLightingMenu(els);
            menu = new VehicleMenu(v);
            // menu.Menu.Visible = true;

            GameFiber.ExecuteNewWhile(Process, () => v);
        }
    }
}

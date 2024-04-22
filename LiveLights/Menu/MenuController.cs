﻿using System;
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
    using Utils;

    internal static class MenuController
    {
        internal static MenuPool Pool = new MenuPool();

        internal static void Process()
        {
            if(KeyChecker.AreKeysDownExclusively(Settings.MenuModifier, Settings.MenuKey))
            {
                if(!VehicleMenu.Menu.Visible)
                {
                    VehicleMenu.Refresh();
                }
                if (Pool.IsAnyMenuOpen())
                {
                    Pool.CloseAllMenus();
                } else
                {
                    VehicleMenu.Menu.Visible = true;
                }
            }

            VehicleMenu.SirenConfigMenu?.ShowSirenInfo(VehicleMenu.Vehicle);
            VehicleMenu.SirenConfigMenu?.SequenceQuickEdit?.Process();


            Pool.ProcessMenus();
        }

        [ConsoleCommand("Open the Live Lights menu (use this if you don't have a keybinding for it)")]
        private static void OpenLiveLightsMenu()
        {
            VehicleMenu.Menu.Visible = true;
        }
    }
}

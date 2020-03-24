using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights
{
    using Rage;
    using Rage.Attributes;

    internal static class XMLTesting
    {
        [ConsoleCommand]
        private static void ExportTestXML()
        {
            CarcolsFile f = new CarcolsFile();
            SirenSetting s = new SirenSetting();

            /*
            for (int i = 0; i < s.Count; i++)
            {
                s[i].Intensity = i * 0.05f;
            }
            */

            /*
            for (int i = 0; i < s.Sirens.Length; i++)
            {
                s.Sirens[i].Intensity = i * 0.5f;
            }
            */

            /*
            for (int i = 0; i < 20; i++)
            {
                SirenEntry e = new SirenEntry();
                e.Intensity = i * 0.5f;
                s.Sirens.Add(e);
            }
            */

            for (int i = 0; i < 20; i++)
            {
                SirenEntry e = new SirenEntry();
                e.Intensity = i * 0.5f;
                s.AddSiren(e);
            }

            f.SirenSettings.Add(s);
            Serializer.SaveItemToXML(f, @"Plugins\LiveLights\text.xml");
        }

        [ConsoleCommand]
        private static void ImportTestXML(string path)
        {
            CarcolsFile c = Serializer.LoadItemFromXML<CarcolsFile>(path); // Serializer.LoadFromXML<CarcolsFile>(path);
            Game.LogTrivial("Loaded " + c.SirenSettings.Count + " siren settings");
            foreach (SirenSetting siren in c.SirenSettings)
            {
                Game.LogTrivial("  " + siren.Name);
            }
        }

        [ConsoleCommand]
        private static void ImportAndApplyXML(string path)
        {
            CarcolsFile c = Serializer.LoadItemFromXML<CarcolsFile>(path); // Serializer.LoadFromXML<CarcolsFile>(path);
            Game.LogTrivial("Loaded " + c.SirenSettings.Count + " siren settings");
            foreach (SirenSetting siren in c.SirenSettings)
            {
                Game.LogTrivial("  " + siren.Name);
            }

            SirenSetting setting = c.SirenSettings[0];
            EmergencyLighting els = Game.LocalPlayer.Character.LastVehicle.GetELSForVehicle();
            if (!els.Exists()) return;

            Game.LogTrivial("Got ELS instance: " + els.Name);

            setting.ApplySirenSettingsToEmergencyLighting(els);
        }

        [ConsoleCommand]
        private static void ExportToXML(string path)
        {
            EmergencyLighting els = Game.LocalPlayer.Character.LastVehicle.GetELSForVehicle();
            if (!els.Exists()) return;

            Game.LogTrivial("Got ELS instance: " + els.Name);

            SirenSetting s = els.ExportEmergencyLightingToSirenSettings();
            CarcolsFile c = new CarcolsFile();
            c.SirenSettings.Add(s);
            Serializer.SaveItemToXML(c, path);
        }
    }
}

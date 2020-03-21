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
                s.Add(e);
            }

            f.SirenSettings.Add(s);
            Serializer.SaveItemToXML(f, @"Plugins\LiveLights\text.xml");
        }
    }
}

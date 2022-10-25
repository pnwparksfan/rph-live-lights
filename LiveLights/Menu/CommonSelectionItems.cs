using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LiveLights.Menu
{
    using RAGENativeUI;
    using Rage;

    internal static class CommonSelectionItems
    {
        public static IEnumerable<byte> MultiplesBytes => Enumerable.Range(1, 4).Select(x => (byte)x);
        public static float[] MultiplierFloat => new float[] { 0.1f, 0.25f, 0.5f, 0.75f, 0.9f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 10f };
        public static float[] UnitCircleDegrees => new float[] { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        public static float[] IntensityFloat => new float[] { 0.5f, 1.0f, 2.0f, 4.0f };
        public static IEnumerable<byte> LightGroupByte => Enumerable.Range(0, 4).Select(x => (byte)x);
        public static byte[] ScaleFactorByte => new byte[] { 0, 2, 4, 10, 20 };
        public static IEnumerable<IDisplayItem> SirensOrAll => Enumerable.Range(1, EmergencyLighting.MaxLights).Select(s => new DisplayItem(s, $"Siren {s}")).Concat(new IDisplayItem[] { new DisplayItem(-1, "1-to-1") });
    }
}

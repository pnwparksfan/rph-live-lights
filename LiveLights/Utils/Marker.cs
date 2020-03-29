using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    /* Thanks to LtFlash for sharing his original Marker class code */
    /* Refactored by PNWParksFan */

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using System.Drawing;

    internal enum MarkerStyle
    {
        MarkerTypeUpsideDownCone = 0,
        MarkerTypeVerticalCylinder = 1,
        MarkerTypeThickChevronUp = 2,
        MarkerTypeThinChevronUp = 3,
        MarkerTypeCheckeredFlagRect = 4,
        MarkerTypeCheckeredFlagCircle = 5,
        MarkerTypeVerticleCircle = 6,
        MarkerTypePlaneModel = 7,
        MarkerTypeLostMCDark = 8,
        MarkerTypeLostMCLight = 9,
        MarkerTypeNumber0 = 10,
        MarkerTypeNumber1 = 11,
        MarkerTypeNumber2 = 12,
        MarkerTypeNumber3 = 13,
        MarkerTypeNumber4 = 14,
        MarkerTypeNumber5 = 15,
        MarkerTypeNumber6 = 16,
        MarkerTypeNumber7 = 17,
        MarkerTypeNumber8 = 18,
        MarkerTypeNumber9 = 19,
        MarkerTypeChevronUpx1 = 20,
        MarkerTypeChevronUpx2 = 21,
        MarkerTypeChevronUpx3 = 22,
        MarkerTypeHorizontalCircleFat = 23,
        MarkerTypeReplayIcon = 24,
        MarkerTypeHorizontalCircleSkinny = 25,
        MarkerTypeHorizontalCircleSkinny_Arrow = 26,
        MarkerTypeHorizontalSplitArrowCircle = 27,
        MarkerTypeDebugSphere = 28
    };

    internal class Marker
    {
        public Color Color { get; set; } = Color.Blue;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        public Rotator Rotation { get; set; } = Rotator.Zero;
        public MarkerStyle Style { get; set; } = MarkerStyle.MarkerTypeUpsideDownCone;

        public Marker(Vector3 pos)
        {
            Position = pos;
            Direction = pos;
        }

        public Marker(Vector3 pos, Color color, MarkerStyle type = MarkerStyle.MarkerTypeUpsideDownCone)
        {
            Style = type;
            Position = pos;
            Direction = pos;
            Color = color;
        }

        public Marker(Vector3 position, Vector3 direction, Vector3 scale, Rotator rotation, Color color, MarkerStyle style)
        {
            Position = position;
            Direction = direction;
            Scale = scale;
            Rotation = rotation;
            Color = Color;
            Style = style;
        }

        /// <summary>
        /// Must be called on each tick, e.g. in a GameFiber or FrameRender
        /// </summary>
        public void Draw() => DrawMarker(this.Style, this.Position, this.Direction, this.Rotation, this.Scale, this.Color);

        public static void DrawMarker(MarkerStyle type, Vector3 position, Vector3 scale, Color color) => DrawMarker(type, position, Vector3.Zero, Rotator.Zero, scale, color);

        public static void DrawMarker(MarkerStyle type, Vector3 position, Color color) => DrawMarker(type, position, Vector3.Zero, Rotator.Zero, new Vector3(1, 1, 1), color);

        public static void DrawMarker(Entity entity, MarkerStyle type, Color color, Vector3 scale, float extraZ)
        {
            if (!entity) return;

            DrawMarker(type, entity.AbovePosition + Vector3.WorldUp * ((scale.Z * 0.5f) - extraZ), scale, color);
        }

        public static void DrawMarker(MarkerStyle type, Vector3 position, Vector3 direction, Rotator rotation, Vector3 scale, Color color)
        {

            if (Game.IsPaused || Game.Console.IsOpen || Game.IsScreenFadedOut)
            {
                return;
            }

            NativeFunction.Natives.DRAW_MARKER(
                    (int)type,
                    position,
                    direction,
                    rotation,
                    scale,
                    (int)color.R,
                    (int)color.G,
                    (int)color.B,
                    (int)color.A,
                    false, true,
                    0, 0,
                    false);
        }
    }
}

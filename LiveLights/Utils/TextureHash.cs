using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    using Rage;

    public static class TextureHash
    {
        static TextureHash()
        {
            string[] textureNames = new string[]
            {
                "VehicleLight_car_oldsquare",
                "VehicleLight_car_standardmodern",
                "VehicleLight_car_standard70s",
                "VehicleLight_misc_searchlight",
                "VehicleLight_misc_squarelight",
                "VehicleLight_bicycle",
                "VehicleLight_car_LED1",
                "VehicleLight_car_LED2",
                "VehicleLight_bike_sport",
                "VehicleLight_bike_round",
                "VehicleLight_car_utility",
                "VehicleLight_car_antique",
                "VehicleLight_sirenlight"
            };

            foreach (string name in textureNames)
            {
                lightTextureHashes[Game.GetHashKey(name)] = name;
            }
        }

        public const string defaultLightTexture = "VehicleLight_sirenlight";
        public static Dictionary<uint, string> lightTextureHashes { get; } = new Dictionary<uint, string> { };

        public static uint StringToHash(string textureName)
        {
            try
            {
                // If case the name is actually already hash (in format 0xABC123), 
                // try converting from hex to uint and return that directly
                return Convert.ToUInt32(textureName, 16);
            }
            catch (FormatException)
            {
                // If the name is not a hash and is actually a real string, then 
                // save the hash to the lookup dictionary and return the hash
                uint hash = Game.GetHashKey(textureName);
                if(!lightTextureHashes.ContainsKey(hash))
                {
                    lightTextureHashes[hash] = textureName;
                }
                return hash;
            }
        }

        public static string HashToString(uint textureHash)
        {
            if (lightTextureHashes.TryGetValue(textureHash, out string textureName))
            {
                return textureName;
            }
            else
            {
                return string.Format("0x{0:X8}", textureHash);
            }
        }
    }
}

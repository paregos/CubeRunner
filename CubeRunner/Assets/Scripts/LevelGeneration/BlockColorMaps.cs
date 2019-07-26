using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scriptss
{
    public static class BlockColorMaps
    {
        private static Random rnd = new Random();

        public static HashSet<ColorMap> ColorMappings = new HashSet<ColorMap>
        {
            new ColorMap(new Color32(163, 194, 161, 225), new Color32(236, 183, 50, 225)),
           // new ColorMap(new Color32(53,121,130, 214), new Color32(255,166,158, 225)),
            new ColorMap(new Color32(140, 134, 170, 214), new Color32(126, 160, 183, 225)),
//            new ColorMap(new Color32(255, 211, 173, 214), new Color32(235, 85, 77, 225)),
//            new ColorMap(new Color32(213, 209, 242, 214), new Color32(235, 85, 77, 225))
        };

        public static ColorMap SelectRandomColorMapping()
        {
            return ColorMappings.ToArray()[rnd.Next(ColorMappings.Count)];
        }
    }

    public class ColorMap
    {
        public Color FloorMainColor;
        public Color PlayerMainColor;

        public ColorMap(Color floorMainColor, Color playerMainColor)
        {
            FloorMainColor = floorMainColor;
            PlayerMainColor = playerMainColor;
        }
    }
}
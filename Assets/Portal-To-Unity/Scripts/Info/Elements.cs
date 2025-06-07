using System.Collections.Generic;
using UnityEngine;

namespace PortalToUnity
{
    public enum Element
    {
        None = 0,
        Earth = 1,
        Water = 2,
        Air = 3,
        Fire = 4,
        Life = 5,
        Undead = 6,
        Magic = 7,
        Tech = 8,
        Light = 9,
        Dark = 10,
        Kaos = 11
    }

    public static class Elements
    {
        public static Dictionary<Element, Color32[]> Colors = new Dictionary<Element, Color32[]>();

        static Elements()
        {
            /* 
             * Declares Portal colors for each element
             * The first set are the regular Portal element colors, using the color ripped from either Giants or Imaginators (TFB and (mostly) VV respectively)
             * Imaginators was used to rip the VV colors as it's Portal color system is similar to SuperChargers, just with some fixes
             * The second and third sets are the two different colors the sides of the Traptanium Portal oscillate between
             * The last remaining value is a unique color for 3DS (blue only) Portals. At the moment, it just mirrors the B value from the first set of colors
             * maybe scriptable-object-ify this implementation
             */
            Colors.Add(Element.None, Construct
            (
                0x00, 0x00, 0x00,   // The original behavior would have the Portal freeze on the last color, not actually change to a black color
                0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x00
            ));

            Colors.Add(Element.Earth, Construct
            (
                0xFF, 0x7F, 0x00,   // Uses the VV color, TFB uses 0x7F, 0x19, 0x00
                0xB4, 0x78, 0x00,
                0x5A, 0x2D, 0x00,
                0x00
            ));

            Colors.Add(Element.Water, Construct
            (
                0x00, 0x44, 0xFF,   // Uses the TFB color, VV uses 0x00, 0x19, 0xFA
                0x00, 0x00, 0xFF,
                0x00, 0xA0, 0x00,
                0xFF
            ));

            Colors.Add(Element.Air, Construct
            (
                0x7F, 0xFF, 0xD4,   // Uses the VV color, TFB uses 0x58, 0xC2, 0xDC
                0x00, 0xA0, 0xFF,
                0x78, 0xA0, 0x78,
                0xD4
            ));

            Colors.Add(Element.Fire, Construct
            (
                0xFF, 0x00, 0x00,   // Both engines use the same color for Fire
                0xFF, 0x00, 0x00,
                0x78, 0x78, 0x00,
                0x00
            ));

            Colors.Add(Element.Life, Construct
            (
                0x00, 0xFF, 0x00,   // Uses the TFB color, VV uses 0x0A, 0xEB, 0x0A
                0x00, 0xFF, 0x00,
                0x1E, 0X78, 0X1E,
                0x00
            ));

            Colors.Add(Element.Undead, Construct
            (
                0xA0, 0x20, 0xF0,   // Uses the VV color, TFB uses 0x15, 0x12, 0x32
                0x64, 0x3C, 0x60,
                0x14, 0x28, 0x14,
                0xF0
            ));

            Colors.Add(Element.Magic, Construct
            (
                0xC7, 0x15, 0x85,   // Uses the VV color, TFB uses 0x74, 0x00, 0x9B
                0xFF, 0x00, 0xFF,
                0x14, 0x3C, 0x14,
                0x85
            ));

            Colors.Add(Element.Tech, Construct
            (
                0xB8, 0x86, 0x0B,   // Uses the VV color, TFB uses 0xAC, 0x6A, 0x00
                0xFF, 0xC8, 0x00,
                0x50, 0x28, 0x00,
                0x0B
            ));

            // Because TFB colors were ripped from Giants, there are no respective Portal colors for the Light, Dark, or Kaos elements (who normally mimics the Dark color)

            Colors.Add(Element.Light, Construct
            (
                0xFF, 0xFC, 0x9D,   
                0xFF, 0xFF, 0x00,
                0x80, 0x80, 0x28,
                0x9D
            ));

            Colors.Add(Element.Dark, Construct
            (
                0x02, 0x23, 0xB3,
                0x00, 0x00, 0x14,
                0x0F, 0x0F, 0x3C,
                0xB3
            ));

            Colors.Add(Element.Kaos, Construct
            (
                0x02, 0x23, 0xB3,
                0x00, 0x00, 0x14,
                0x0F, 0x0F, 0x3C,
                0xB3
            ));   
        }

        private static Color32[] Construct(byte defaultR, byte defaultG, byte defaultB, byte traptaniumR0, byte traptaniumG0, byte traptaniumB0, byte traptaniumR1, byte traptaniumG1, byte traptaniumB1, byte n3ds)
        {
            return new Color32[4]
            {
                new Color32(defaultR, defaultG, defaultB, 255),
                new Color32(traptaniumR0, traptaniumG0, traptaniumB0, 255),
                new Color32(traptaniumR1, traptaniumG1, traptaniumB1, 255),
                new Color32(0, 0, n3ds, 255)
            };
        }
    }
}
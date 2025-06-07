using System.Collections.Generic;
using UnityEngine;

namespace PortalToUnity
{
    public static class SkylanderDatabase
    {
        private static Dictionary<ushort, Skylander> Skylanders = new Dictionary<ushort, Skylander>();

        public static void Initialize()
        {
            Skylanders.Clear();
            Skylander[] skylanders = Resources.LoadAll<Skylander>("Portal-To-Unity/Figures");
            foreach (Skylander skylander in skylanders)
                AddSkylander(skylander);
        }

        public static void AddSkylander(Skylander skylander)
        {
            if (!Skylanders.ContainsKey(skylander.CharacterID))
                Skylanders.Add(skylander.CharacterID, skylander);
            else
                PTUManager.LogWarning($"Skylander with ToyCode {skylander.CharacterID} already exists in the database.", LogPriority.Normal);
        }

        public static bool GetSkylander(ushort characterID, out Skylander skylander)
        {
            if (Skylanders.TryGetValue(characterID, out skylander))
                return true;

            return false;
        }

        public static IEnumerable<Skylander> GetAllSkylanders() => Skylanders.Values;
    }

    // Common subtext names used by Skylanders
    public static class Tags
    {
        public const string S = "Special";                        // For registered chase variants
        public const string R = "Rare";                           // For certain registered chase variants
        public const string V = "Variant";                        // Normally this would be "Special" in VV engine and blank in TFB
        public const string S1 = "Series 1";
        public const string S2 = "Series 2";
        public const string S3 = "Series 3";
        public const string S4 = "Series 4";                      // Used exclusively by Tidal Wave Gill Grunt
        public const string LC = "LightCore";
        public const string EE = "Eon's Elite";
        public const string EV = "Exclusive Event Edition";       // Used exclusively by E3 Hot Streak
        public const string VV = "Vicarious Visions";             // Used exclusively by (Gear Head) VVind-Up

        public static bool IsReposeTag(string tag) => tag == S2 || tag == S3 || tag == S4;
        public static bool IsSpecialTag(string tag) => tag == S || tag == R;
    }
}
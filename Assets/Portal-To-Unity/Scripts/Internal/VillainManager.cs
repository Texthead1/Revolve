using System.Collections.Generic;
using UnityEngine;

namespace PortalToUnity
{
    public static class VillainDatabase
    {
        private static Dictionary<byte, Villain> Villains = new Dictionary<byte, Villain>();

        public static void Initialize()
        {
            Villains.Clear();
            Villain[] villains = Resources.LoadAll<Villain>("Portal-To-Unity/Villains");
            foreach (Villain villain in villains)
                AddVillain(villain);
        }

        public static void AddVillain(Villain villain)
        {
            if (!Villains.ContainsKey(villain.VillainID))
                Villains.Add(villain.VillainID, villain);
            else
                PTUManager.LogWarning($"Villain with ID {villain.VillainID} already exists in the database.", LogPriority.Normal);
        }

        public static bool GetVillain(byte villainID, out Villain villain)
        {
            if (Villains.TryGetValue(villainID, out villain))
                return true;

            PTUManager.LogWarning($"Villain with ID {villainID} not found in the database.", LogPriority.Normal);
            return false;
        }

        public static IEnumerable<Villain> GetAllVillains() => Villains.Values;
    }
}
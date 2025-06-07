using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortalToUnity
{
    public static class PortalDatabase
    {
        private static List<PortalInfo> Portals = new List<PortalInfo>();

        public static void Initialize()
        {
            Portals.Clear();
            Portals = Resources.LoadAll<PortalInfo>("Portal-To-Unity/Portals").ToList();
        }

        public static PortalInfo PortalFromID(byte[] id)
        {
            try
            {
                return Portals.First(x => x.ID.SequenceEqual(id));
            }
            catch (Exception) { return null; }
        }

        public static string NameFromID(byte[] id)
        {
            PortalInfo info = PortalFromID(id);
            return info != null ? info.Name : "Portal of Power";
        }
    }
}
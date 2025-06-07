using System.Collections.Generic;
using UnityEngine;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewSkylander", menuName = "Portal-To-Unity/Skylander", order = 0)]
#endif
    public class Skylander : ScriptableObject
    {
        public string Name;
        public ushort CharacterID;
        public SkyType Type;
        public Element Element;
        public string Prefix;
        public List<SkylanderVariant> Variants = new List<SkylanderVariant>();
        public string ToyCodeEnumType = "ToyCode";
        public string DecoIDEnumType = "DecoID";

        public string GetLongName() => Prefix == string.Empty ? Name : $"{Prefix} {Name}";
    }
}
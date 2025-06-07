using UnityEngine;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewVillain", menuName = "Portal-To-Unity/Villain", order = 20)]
#endif
    public class Villain : ScriptableObject
    {
        public string Name;
        public byte VillainID;
        public Element Element;
        public VillainVariant Variant;
        public string VillainIDEnumType = typeof(VillainID).Name;
    }
}
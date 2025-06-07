using UnityEngine;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewTrinket", menuName = "Portal-To-Unity/Trinket", order = 42)]
#endif
    public class Trinket : ScriptableObject
    {
        public string Name;
        public byte ID;
        public string TrinketIDEnumType = typeof(TrinketID).Name;
    }
}
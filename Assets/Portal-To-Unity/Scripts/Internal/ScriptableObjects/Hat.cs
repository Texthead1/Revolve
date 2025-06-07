using UnityEngine;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewHat", menuName = "Portal-To-Unity/Hat", order = 40)]
#endif
    public class Hat : ScriptableObject
    {
        public string Name;
        public ushort ID;
        public string HatIDEnumType = typeof(HatID).Name;
    }
}
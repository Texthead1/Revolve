using UnityEngine;

namespace PortalToUnity
{
    public enum RewardType
    {
        CriticalHit,
        Armor,
        Speed,
        ElementalPower
    }

#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewHeroic", menuName = "Portal-To-Unity/Heroic Challenge", order = 41)]
#endif
    public class HeroicChallenge : ScriptableObject
    {
        public string Name;
        public ushort ID;
        public RewardType RewardType;
        public string HeroicIDEnumType = typeof(HeroicChallengeID).Name;
    }
}
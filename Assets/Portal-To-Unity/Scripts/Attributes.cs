using System;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsToyCodeAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsDecoIDAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsHatIDAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsHeroicChallengeIDAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsTrinketIDAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsVillainIDAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSBackAttachmentAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSHeadgearAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSLegGuardsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSArmGuardsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSShoulderGuardsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSEarsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSHeadAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSTorsoAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSArmsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSLegsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSTailAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSAuraAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSSoundFXAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSEyesAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSCatchphrase1Attribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSCatchphrase2Attribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSMusicAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSVoiceAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum)]
    public class MarkAsCYOSVoiceFXAttribute : Attribute {}
#endif
}
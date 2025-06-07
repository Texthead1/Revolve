using UnityEditor;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewSkylanderVariant", menuName = "Portal-To-Unity/Skylander Variant/Default", order = 1)]
#endif
    public class SkylanderVariant : ScriptableObject
    {
        public string Name;
        public string Tag;
        public VariantID VariantID;
        public string NameOverride;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Series 2 (Giants)")]
        public static void CreateS2SGVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Giants2012, true, false, false, false, 1), Tags.S2);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Alt Deco (Giants)")]
        public static void CreateAltSGVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Giants2012, false, true, false, false, 2), Tags.V);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/LightCore (Giants)")]
        public static void CreateLCSGVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Giants2012, false, false, true, false, 6), Tags.LC);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Legendary (Giants)")]
        public static void CreateLGSGVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Giants2012, false, true, false, false, 3), Tags.V, "Legendary");
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Repose (SWAP Force)")]
        public static void CreateS2SSFVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SwapForce2013, true, false, false, false, 5), Tags.S2);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Alt Deco (SWAP Force)")]
        public static void CreateAltSSFVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SwapForce2013, false, true, false, false, 2), Tags.V);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/LightCore (SWAP Force)")]
        public static void CreateLCSSFVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SwapForce2013, false, false, true, false, 6), Tags.LC);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Legendary (SWAP Force)")]
        public static void CreateLGSSFVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SwapForce2013, false, true, false, false, 3), Tags.V, "Legendary");
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Repose (Trap Team)")]
        public static void CreateS2STTVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.TrapTeam2014, true, false, false, false, 5), Tags.S2);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Alt Deco (Trap Team)")]
        public static void CreateAltSTTVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.TrapTeam2014, false, true, false, false, 2), Tags.V);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Legendary (Trap Team)")]
        public static void CreateLGSTTVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.TrapTeam2014, false, true, false, false, 3), Tags.V, "Legendary");
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Eon's Elite (Trap Team)")]
        public static void CreateEESTTVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.TrapTeam2014, true, false, false, false, 16), Tags.EE);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/SuperCharger (SuperChargers)")]
        public static void CreateSSCVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SuperChargers2015, false, false, false, true, 0));
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Alt Deco (SuperChargers)")]
        public static void CreateAltSSCVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SuperChargers2015, false, true, false, false, 2), Tags.V);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Legendary (SuperChargers)")]
        public static void CreateLGSSCVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SuperChargers2015, false, true, false, false, 3), Tags.V, "Legendary");
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Eon's Elite (SuperChargers)")]
        public static void CreateEESSCVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.SuperChargers2015, true, false, false, false, 16), Tags.EE);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Alt Deco (Imaginators)")]
        public static void CreateAltSIVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Imaginators2016, false, true, false, false, 2), Tags.V);
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Legendary (Imaginators)")]
        public static void CreateLGSIVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Imaginators2016, false, true, false, false, 3), Tags.V, "Legendary");
        }

        [MenuItem("Assets/Create/Portal-To-Unity/Skylander Variant/Creation Crystal (Imaginators)")]
        public static void CreateCYOSSIVariant()
        {
            CreateSkylanderVariant(new VariantID(SkylandersGame.Imaginators2016, false, false, true, false, 0));
        }

        private static void CreateSkylanderVariant(VariantID variantID, string tag = null, string name = null)
        {
            SkylanderVariant variant = CreateInstance<SkylanderVariant>();
            variant.Name ??= name;
            variant.Tag ??= tag;
            variant.VariantID = variantID;
            variant.NameOverride = null;

            string selectedPath = GetSelectedPathOrFallback();
            ProjectWindowUtil.CreateAsset(variant, AssetDatabase.GenerateUniqueAssetPath(selectedPath + "/NewSkylanderVariant.asset"));
        }
#endif
    }
}
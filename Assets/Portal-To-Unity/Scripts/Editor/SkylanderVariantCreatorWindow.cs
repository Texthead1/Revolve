using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class SkylanderVariantCreatorWindow : EditorWindow
    {
        public static Action<SkylanderVariant> OnVariantCreated;

        private static readonly List<Type> enums = new List<Type>();

        private SkylanderVariant workingVariant = null;
        private Skylander parent;
        private string presetButtonName = "Default";
        private string presetDecoIDName = "";
        private string desiredDecoIDEnumType = "";

        private struct VariantPresetInfo
        {
            public string Label;
            public VariantID VariantID;
            public string Tag;
            public string Name;
            public string DecoIDEnumType;

            public VariantPresetInfo(string label, VariantID variantID, string tag, string name, string decoIDEnumType)
            {
                Label = label;
                VariantID = variantID;
                Tag = tag;
                Name = name;
                DecoIDEnumType = decoIDEnumType;
            }
        }

        private static readonly List<VariantPresetInfo> presets = new List<VariantPresetInfo>
        {
            new VariantPresetInfo("Default", new VariantID(), "", "", "DecoID"),
            new VariantPresetInfo("Trap", new VariantID(SkylandersGame.TrapTeam2014, false, false, false, false, -1), "", "", "TrapDecoID"),
            new VariantPresetInfo("Creation Crystal", new VariantID(SkylandersGame.Imaginators2016, false, false, true, false, -1), "", "", "CrystalDecoID"),
            new VariantPresetInfo("Create Your Own Skylander", new VariantID(SkylandersGame.Imaginators2016, false, false, false, false, -1), "", "", "CYOSDecoID"),
            new VariantPresetInfo("Imaginite Chest", new VariantID(SkylandersGame.Imaginators2016, false, false, false, false, -1), "", "", "ChestDecoID"),
            new VariantPresetInfo("Series 2 (Giants)", new VariantID(SkylandersGame.Giants2012, true, false, false, false, 1), Tags.S2, "", "DecoID"),
            new VariantPresetInfo("Alt Deco (Giants)", new VariantID(SkylandersGame.Giants2012, false, true, false, false, 2), Tags.V, "", "DecoID"),
            new VariantPresetInfo("LightCore (Giants)", new VariantID(SkylandersGame.Giants2012, false, false, true, false, 6), Tags.LC, "", "DecoID"),
            new VariantPresetInfo("Legendary (Giants)", new VariantID(SkylandersGame.Giants2012, false, true, false, false, 3), Tags.V, "Legendary", "DecoID"),
            new VariantPresetInfo("Repose (SWAP Force)", new VariantID(SkylandersGame.SwapForce2013, true, false, false, false, 5), Tags.S2, "", "DecoID"),
            new VariantPresetInfo("Alt Deco (SWAP Force)", new VariantID(SkylandersGame.SwapForce2013, false, true, false, false, 2), Tags.V, "", "DecoID"),
            new VariantPresetInfo("LightCore (SWAP Force)", new VariantID(SkylandersGame.SwapForce2013, false, false, true, false, 6), Tags.LC, "", "DecoID"),
            new VariantPresetInfo("Legendary (SWAP Force)", new VariantID(SkylandersGame.SwapForce2013, false, true, false, false, 3), Tags.V, "Legendary", "DecoID"),
            new VariantPresetInfo("Repose (Trap Team)", new VariantID(SkylandersGame.TrapTeam2014, true, false, false, false, 5), Tags.S2, "", "DecoID"),
            new VariantPresetInfo("Alt Deco (Trap Team)", new VariantID(SkylandersGame.TrapTeam2014, false, true, false, false, 2), Tags.V, "", "DecoID"),
            new VariantPresetInfo("Legendary (Trap Team)", new VariantID(SkylandersGame.TrapTeam2014, false, true, false, false, 3), Tags.V, "Legendary", "DecoID"),
            new VariantPresetInfo("Eon's Elite (Trap Team)", new VariantID(SkylandersGame.TrapTeam2014, true, false, false, false, 16), Tags.EE, "", "DecoID"),
            new VariantPresetInfo("SuperCharger (SuperChargers)", new VariantID(SkylandersGame.SuperChargers2015, false, false, false, true, 0), "", "", "DecoID"),
            new VariantPresetInfo("Alt Deco (SuperChargers)", new VariantID(SkylandersGame.SuperChargers2015, false, true, false, false, 2), Tags.V, "", "DecoID"),
            new VariantPresetInfo("Legendary (SuperChargers)", new VariantID(SkylandersGame.SuperChargers2015, false, true, false, false, 3), Tags.V, "Legendary", "DecoID"),
            new VariantPresetInfo("Eon's Elite (SuperChargers)", new VariantID(SkylandersGame.SuperChargers2015, true, false, false, false, 16), Tags.EE, "", "DecoID"),
            new VariantPresetInfo("Alt Deco (Imaginators)", new VariantID(SkylandersGame.Imaginators2016, false, true, false, false, 2), Tags.V, "", "DecoID"),
            new VariantPresetInfo("Legendary (Imaginators)", new VariantID(SkylandersGame.Imaginators2016, false, true, false, false, 3), Tags.V, "Legendary", "DecoID"),
        };

        public static void ShowWindow(Skylander parent)
        {
            SkylanderVariantCreatorWindow window = GetWindow<SkylanderVariantCreatorWindow>("Create Skylander Variant");
            window.parent = parent;
            window.desiredDecoIDEnumType = parent.DecoIDEnumType;
            window.workingVariant = CreateInstance<SkylanderVariant>();

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            var findTarget = enumTypes.Where(x => x.Name == window.desiredDecoIDEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(window.workingVariant.VariantID.DecoID))
                    window.presetDecoIDName = Enum.GetName(findTarget, window.workingVariant.VariantID.DecoID);
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == window.desiredDecoIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(window.workingVariant.VariantID.DecoID))
                            window.presetDecoIDName = Enum.GetName(type, window.workingVariant.VariantID.DecoID);
                    }
                }
            }
            else
            {
                foreach (Type type in enumTypes)
                {
                    if (type.Name == window.desiredDecoIDEnumType) continue;

                    if (Enum.GetValues(type).Cast<int>().Contains(window.workingVariant.VariantID.DecoID))
                    {
                        window.desiredDecoIDEnumType = type.Name;
                        window.presetDecoIDName = Enum.GetName(type, window.workingVariant.VariantID.DecoID);
                    }
                }
            }
            window.Show();
        }

        private void OnGUI()
        {
            if (workingVariant == null)
                workingVariant = CreateInstance<SkylanderVariant>();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            foreach (Type type in enumTypes)
                enums.Add(Enum.GetValues(type).GetValue(0).GetType());

            GUILayout.Label($"New {parent.Name} Variant", boldStyle);
            workingVariant.name = EditorGUILayout.TextField("File Name", workingVariant.name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Preset", GUILayout.Width(EditorGUIUtility.labelWidth));

            if (GUILayout.Button(presetButtonName, EditorStyles.popup))
                ShowPresetMenu();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            workingVariant.Name = EditorGUILayout.TextField("Name", workingVariant.Name);
            workingVariant.Tag = EditorGUILayout.TextField("Tag", workingVariant.Tag);
            workingVariant.VariantID.YearCode = (SkylandersGame)EditorGUILayout.EnumPopup("Year Code", workingVariant.VariantID.YearCode);
            workingVariant.VariantID.IsRepose = EditorGUILayout.Toggle("Is Repose", workingVariant.VariantID.IsRepose);
            workingVariant.VariantID.IsAltDeco = EditorGUILayout.Toggle("Is Alt Deco", workingVariant.VariantID.IsAltDeco);
            workingVariant.VariantID.IsLightCore = EditorGUILayout.Toggle("Is LightCore", workingVariant.VariantID.IsLightCore);
            workingVariant.VariantID.IsSuperCharger = EditorGUILayout.Toggle("Is SuperCharger", workingVariant.VariantID.IsSuperCharger);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset Deco ID", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string discoveredEnum = typeof(DecoID).Name;

            var findTarget = enumTypes.Where(x => x.Name == desiredDecoIDEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(workingVariant.VariantID.DecoID))
                {
                    discoveredEnum = findTarget.Name;
                    presetDecoIDName = Enum.GetName(findTarget, workingVariant.VariantID.DecoID);
                }
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == desiredDecoIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingVariant.VariantID.DecoID))
                        {
                            discoveredEnum = type.Name;
                            presetDecoIDName = Enum.GetName(type, workingVariant.VariantID.DecoID);
                        }
                    }
                }
            }
            else
            {
                foreach (Type type in enumTypes)
                {
                    if (type.Name == desiredDecoIDEnumType) continue;

                    if (Enum.GetValues(type).Cast<int>().Contains(workingVariant.VariantID.DecoID))
                    {
                        desiredDecoIDEnumType = type.Name;
                        presetDecoIDName = Enum.GetName(type, workingVariant.VariantID.DecoID);
                    }
                }
            }

            if (GUILayout.Button(presetDecoIDName, EditorStyles.popup))
            {
                DecoIDPicker picker = CreateInstance<DecoIDPicker>();
                picker.onSelection = (x, y) =>
                {
                    workingVariant.VariantID.DecoID = (byte)x;
                    desiredDecoIDEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingVariant.VariantID.DecoID = (byte)Mathf.Clamp(EditorGUILayout.IntField("Deco ID", workingVariant.VariantID.DecoID), 0, byte.MaxValue);
            EditorGUILayout.Space();
            workingVariant.NameOverride = EditorGUILayout.TextField("Name Override", workingVariant.NameOverride);
            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Variant")) return;

            desiredDecoIDEnumType = discoveredEnum;

            string newFileName = workingVariant.name.Trim();

            if (newFileName == string.Empty)
                newFileName = parent.Name + "Variant";

            string path = $"Assets/Resources/Portal-To-Unity/Figures/Variants/{(int)parent.CharacterID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Variant with file name {newFileName} already exists. Please use a different name instead");
                return;
            }

            AssetDatabase.CreateAsset(workingVariant, path);
            parent.Variants ??= new List<SkylanderVariant>();
            parent.Variants.Add(workingVariant);
            parent.DecoIDEnumType = desiredDecoIDEnumType;
            EditorUtility.SetDirty(parent);
            AssetDatabase.SaveAssets();
            OnVariantCreated?.Invoke(workingVariant);
            Close();
        }

        private void ShowPresetMenu()
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < presets.Count; i++)
            {
                if (i == 5) menu.AddSeparator("");

                int index = i;
                menu.AddItem(new GUIContent(presets[i].Label), false, () => 
                {
                    OnPresetSelected(presets[index]);
                });
            }
            menu.ShowAsContext();
        }

        private void OnPresetSelected(VariantPresetInfo preset)
        {
            presetButtonName = preset.Label;
            workingVariant.Name = preset.Name;
            workingVariant.Tag = preset.Tag;
            desiredDecoIDEnumType = preset.DecoIDEnumType;

            if (preset.VariantID.DecoID == -1)
            {
                string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

                var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null);
                enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

                int prevDecoID = workingVariant.VariantID.DecoID;
                workingVariant.VariantID = preset.VariantID;
                int[] list = Enum.GetValues(enumTypes.Where(x => x.Name == desiredDecoIDEnumType).FirstOrDefault()).Cast<int>().ToArray();

                workingVariant.VariantID.DecoID = (prevDecoID == 0) ? list.Min() : prevDecoID;
                return;
            }
            workingVariant.VariantID = preset.VariantID;
        }
    }
}
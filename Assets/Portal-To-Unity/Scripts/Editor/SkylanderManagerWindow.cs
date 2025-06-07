using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using static PortalToUnity.Global;
using Unity.VisualScripting;
using System.IO;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace PortalToUnity
{
    public class SkylanderManagerWindow : EditorWindow
    {
        private List<Skylander> skylanders;
        private Skylander selectedSkylander;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private Skylander editingSkylander = null;
        private SkylanderVariant editingVariant = null;
        private Dictionary<SkylanderVariant, bool> foldoutStates = new Dictionary<SkylanderVariant, bool>();
        private Dictionary<string, bool> groupFoldoutStates = new Dictionary<string, bool>();
        private Skylander tempSkylander;
        private SkylanderVariant tempVariant;
        private int selectedTypeIndex = 0;
        private int selectedDecoID = 0;
        private bool refreshingSkylanders;
        private float leftPanelWidth = 0.3333f;
        private string searchQuery = string.Empty;
        private string presetToyCodeName = typeof(ToyCode).Name;
        private string presetDecoIDName = "";
        private string desiredDecoIDEnumType = "";

        private readonly List<Type> enums = new List<Type>();
        private readonly string[] groupLabelNames = new string[7] { "Spyro's Adventure", "Giants", "SWAP Force", "Trap Team", "SuperChargers", "Imaginators", "Other" };

        [MenuItem("Portal-To-Unity/Collections/Skylanders", false, 30)]
        public static void ShowWindow()
        {
            SkylanderManagerWindow window = GetWindow<SkylanderManagerWindow>("Skylanders");
            window.titleContent = new GUIContent("Skylanders", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Figure" : "Portal-To-Unity/Editor/Icons/Figure"));
        }

        public void OnGUI()
        {
            if (!refreshingSkylanders)
                LoadSkylanders();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (skylanders == null) return;

            skylanders = skylanders.OrderBy(x => x.CharacterID).ToList();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));
            IEnumerable<Skylander>[] groups = new IEnumerable<Skylander>[6];

            EditorGUILayout.BeginHorizontal();

            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                editingSkylander = null;
                editingVariant = null;
                SkylanderCreatorWindow.ShowWindow(searchQuery);
                SkylanderCreatorWindow.OnSkylanderCreated = (x) =>
                {
                    searchQuery = "";
                    selectedSkylander = x;
                };
            }
            EditorGUILayout.EndHorizontal();

            List<Skylander> skylandersFiltered = new List<Skylander>(skylanders);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("element:", StringComparison.OrdinalIgnoreCase))
                    skylandersFiltered.RemoveAll(x => !x.Element.ToString().Contains(search.Replace("element:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                    skylandersFiltered.RemoveAll(x => !((int)x.CharacterID).ToString().Contains(search.Replace("id:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("type:", StringComparison.OrdinalIgnoreCase))
                    skylandersFiltered.RemoveAll(x => !x.Type.ToString().Contains(search.Replace("type:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    skylandersFiltered.RemoveAll(x => !x.GetLongName().Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            groups[0] = skylandersFiltered.Where(x => ToyCodeExtensions.IsSSA(x.CharacterID) || ToyCodeExtensions.IsTFB_Item(x.CharacterID) && x.CharacterID < ToyCodeExtensions.TFB_BattlePieces_Low || x.CharacterID >= ToyCodeExtensions.TFB_Expansions_Low && x.CharacterID < 305 || x.CharacterID == (ushort)ToyCode.Mini_Terrabite || x.CharacterID == (ushort)ToyCode.Mini_GillRunt || x.CharacterID == (ushort)ToyCode.Mini_TriggerSnappy || x.CharacterID == (ushort)ToyCode.Mini_WhisperElf).ToList();
            skylandersFiltered.RemoveAll(x => groups[0].Contains(x));
            groups[1] = skylandersFiltered.Where(x => ToyCodeExtensions.IsGiants(x.CharacterID) || ToyCodeExtensions.IsTFB_BattlePiece(x.CharacterID) || ToyCodeExtensions.IsMini(x.CharacterID) && x.CharacterID > 539).ToList();
            skylandersFiltered.RemoveAll(x => groups[1].Contains(x));
            groups[2] = skylandersFiltered.Where(x => ToyCodeExtensions.IsSwapForce(x.CharacterID) || ToyCodeExtensions.IsSwapPart(x.CharacterID) || x.CharacterID >= ToyCodeExtensions.VV_Items_Low && x.CharacterID < ToyCodeExtensions.Vehicles_Low || ToyCodeExtensions.IsVV_Expansion(x.CharacterID)).ToList();
            skylandersFiltered.RemoveAll(x => groups[2].Contains(x));
            groups[3] = skylandersFiltered.Where(x => ToyCodeExtensions.IsTrapTeam(x.CharacterID) || ToyCodeExtensions.IsMini(x.CharacterID) || x.CharacterID != 235 && x.CharacterID >= ToyCodeExtensions.Traps_Low && x.CharacterID < 310 || ToyCodeExtensions.IsTrapTeam_Debug(x.CharacterID)).ToList();
            skylandersFiltered.RemoveAll(x => groups[3].Contains(x));
            groups[4] = skylandersFiltered.Where(x => ToyCodeExtensions.IsVehicle(x.CharacterID) || x.CharacterID >= ToyCodeExtensions.SuperChargers_Low && x.CharacterID <= ToyCodeExtensions.TemplateVehicle_High).ToList();
            skylandersFiltered.RemoveAll(x => groups[4].Contains(x));
            groups[5] = skylandersFiltered;

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i].Count() == 0) continue;
                IEnumerable<Skylander> group = groups[i];

                if (!groupFoldoutStates.ContainsKey(i.ToString()))
                    groupFoldoutStates[i.ToString()] = EditorPrefs.GetBool($"GroupFoldoutState_{i}", true);

                groupFoldoutStates[i.ToString()] = EditorGUILayout.Foldout(groupFoldoutStates[i.ToString()], groupLabelNames[i], true);
                EditorPrefs.SetBool($"GroupFoldoutState_{i}", groupFoldoutStates[i.ToString()]);

                if (groupFoldoutStates[i.ToString()])
                {
                    foreach (Skylander skylander in group)
                    {
                        bool button = GUILayout.Button(skylander.GetLongName());
                        e = Event.current;

                        if (!mouseOverWindow) continue;

                        if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                            ShowContextOption("Show Skylander in Explorer", "Assets/Resources/Portal-To-Unity/Figures/", skylander);
                        else if (button && e.button != 1)
                            selectedSkylander = skylander;
                    }
                }
            }

            if (selectedSkylander == null || editingVariant != null && !selectedSkylander.Variants.Contains(editingVariant))
            {
                editingVariant = null;
                tempVariant = null;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedSkylander == null)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Show in Explorer"), false, () => EditorUtility.RevealInFinder("Assets/Resources/Portal-To-Unity/Figures/"));
                    menu.ShowAsContext();
                }
                else
                {
                    ShowContextOption("Show Skylander in Explorer", "Assets/Resources/Portal-To-Unity/Figures/", selectedSkylander);
                }
            }

            EditorGUILayout.Space();
            if (skylanders.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Skylanders found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Figures/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Show in Explorer"), false, () => EditorUtility.RevealInFinder("Assets/Resources/Portal-To-Unity/Figures/"));
                    menu.ShowAsContext();
                }
                selectedSkylander = null;
            }
            else if (selectedSkylander != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (selectedSkylander != editingSkylander)
                {
                    EditorGUILayout.LabelField($"Skylander Info ({selectedSkylander.name})", boldStyle);
                    editingSkylander = null;

                    if (GUILayout.Button("Modify", GUILayout.Width(75)))
                    {
                        GenericMenu modifyMenu = new GenericMenu();
                        modifyMenu.AddItem(new GUIContent("Edit"), false, () =>
                        {
                            tempSkylander = CreateInstance<Skylander>();
                            tempSkylander.name = selectedSkylander.name;
                            tempSkylander.Name = selectedSkylander.Name;
                            tempSkylander.CharacterID = selectedSkylander.CharacterID;
                            tempSkylander.Type = selectedSkylander.Type;
                            tempSkylander.Element = selectedSkylander.Element;
                            tempSkylander.Prefix = selectedSkylander.Prefix;
                            tempSkylander.ToyCodeEnumType = selectedSkylander.ToyCodeEnumType;
                            editingVariant = null;
                            tempVariant = null;
                            GUI.FocusControl(null);
                            editingSkylander = selectedSkylander;
                        });

                        modifyMenu.AddItem(new GUIContent("Delete"), false, () =>
                        {
                            if (selectedSkylander.AskForDeletion())
                            {
                                foreach (SkylanderVariant variant in selectedSkylander.Variants)
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variant));

                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSkylander));
                                AssetDatabase.Refresh();
                            }
                        });
                        modifyMenu.ShowAsContext();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Prefix", selectedSkylander.Prefix);
                    EditorGUILayout.LabelField("Name", selectedSkylander.Name);
                    EditorGUILayout.LabelField("Character ID", GetFriendlyToyCode(selectedSkylander));
                    EditorGUILayout.LabelField("Type", selectedSkylander.Type.ToString());
                    EditorGUILayout.LabelField("Element", selectedSkylander.Element.ToString());
                }
                else
                {
                    GUIStyle altBoldStyle = new GUIStyle(GUI.skin.textField);
                    altBoldStyle.fontStyle = FontStyle.Bold;
                    altBoldStyle.fontSize = 14;
                    tempSkylander.name = GUI.TextField(new Rect(2, 7, 236, 20), tempSkylander.name, altBoldStyle);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Cancel", GUILayout.Width(75)))
                    {
                        GUI.FocusControl(null);
                        editingSkylander = null;
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(1);
                    tempSkylander.Prefix = EditorGUILayout.TextField("Prefix", tempSkylander.Prefix);
                    tempSkylander.Name = EditorGUILayout.TextField("Name", tempSkylander.Name);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preset ToyCode", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

                    string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

                    var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsToyCodeAttribute>() != null);
                    enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

                    var findTarget = enumTypes.Where(x => x.Name == tempSkylander.ToyCodeEnumType).FirstOrDefault();
                    if (findTarget != null && findTarget != default)
                    {
                        if (Enum.GetValues(findTarget).Cast<int>().Contains(tempSkylander.CharacterID))
                            presetToyCodeName = Enum.GetName(findTarget, (int)tempSkylander.CharacterID);
                        else
                        {
                            foreach (Type type in enumTypes)
                            {
                                if (type.Name == tempSkylander.ToyCodeEnumType) continue;

                                if (Enum.GetValues(type).Cast<int>().Contains(tempSkylander.CharacterID))
                                    presetToyCodeName = Enum.GetName(type, (int)tempSkylander.CharacterID);
                            }
                        }
                    }
                    else
                    {
                        foreach (Type type in enumTypes)
                        {
                            if (type.Name == tempSkylander.ToyCodeEnumType) continue;

                            if (Enum.GetValues(type).Cast<int>().Contains(tempSkylander.CharacterID))
                            {
                                tempSkylander.ToyCodeEnumType = type.Name;
                                presetToyCodeName = Enum.GetName(type, (int)tempSkylander.CharacterID);
                            }
                        }
                    }

                    if (GUILayout.Button(presetToyCodeName, EditorStyles.popup))
                    {
                        ToyCodePicker picker = CreateInstance<ToyCodePicker>();
                        picker.onSelection = (x, y) =>
                        { 
                            tempSkylander.CharacterID = (ushort)x;
                            tempSkylander.ToyCodeEnumType = y;
                        };
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
                    }

                    EditorGUILayout.EndHorizontal();
                    tempSkylander.CharacterID = (ushort)Mathf.Clamp(EditorGUILayout.IntField("Character ID", tempSkylander.CharacterID), 0, ushort.MaxValue);
                    EditorGUILayout.Space();
                    tempSkylander.Type = (SkyType)EditorGUILayout.EnumPopup("Type", tempSkylander.Type);
                    tempSkylander.Element = (Element)EditorGUILayout.EnumPopup("Element", tempSkylander.Element);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Save", GUILayout.Width(85)))
                    {
                        presetToyCodeName = Enum.GetNames(typeof(ToyCode)).FirstOrDefault();
                        string path = AssetDatabase.GetAssetPath(selectedSkylander);
                        AssetDatabase.RenameAsset(path, tempSkylander.name);
                        AssetDatabase.SaveAssets();

                        selectedSkylander.Name = tempSkylander.Name;
                        selectedSkylander.CharacterID = tempSkylander.CharacterID;
                        selectedSkylander.Type = tempSkylander.Type;
                        selectedSkylander.Element = tempSkylander.Element;
                        selectedSkylander.Prefix = tempSkylander.Prefix;

                        if (findTarget != null && findTarget != default)
                        {
                            if (Enum.GetValues(findTarget).Cast<int>().Contains(tempSkylander.CharacterID))
                                selectedSkylander.ToyCodeEnumType = tempSkylander.ToyCodeEnumType;
                            else
                            {
                                foreach (Type type in enumTypes)
                                {
                                    if (type.Name == tempSkylander.ToyCodeEnumType) continue;

                                    if (Enum.GetValues(type).Cast<int>().Contains(tempSkylander.CharacterID))
                                        selectedSkylander.ToyCodeEnumType = type.Name;
                                }
                            }
                        }
                        else
                        {
                            foreach (Type type in enumTypes)
                            {
                                if (type.Name == tempSkylander.ToyCodeEnumType) continue;

                                if (Enum.GetValues(type).Cast<int>().Contains(tempSkylander.CharacterID))
                                    selectedSkylander.ToyCodeEnumType = type.Name;
                            }
                        }
                        editingSkylander = null;
                        tempSkylander = null;
                        GUI.FocusControl(null);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Variants", boldStyle);

                try
                {
                    selectedSkylander.Variants.RemoveAll(x => x == null);
                    selectedSkylander.Variants = selectedSkylander.Variants.OrderBy(x => x.VariantID.Encode()).ToList();

                    foreach (SkylanderVariant variant in selectedSkylander.Variants)
                    {
                        if (!foldoutStates.ContainsKey(variant))
                            foldoutStates[variant] = EditorPrefs.GetBool($"FoldoutState_{variant.GetInstanceID()}", true);

                        if (variant != editingVariant)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(2);
                            foldoutStates[variant] = EditorGUILayout.Foldout(foldoutStates[variant], variant.name, true);
                            EditorGUILayout.EndVertical();
                            EditorPrefs.SetBool($"FoldoutState_{variant.GetInstanceID()}", foldoutStates[variant]);
                            if (foldoutStates[variant])
                            {
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("Modify", GUILayout.Width(60)))
                                {
                                    GenericMenu modifyMenu = new GenericMenu();
                                    modifyMenu.AddItem(new GUIContent("Edit"), false, () =>
                                    {
                                        editingSkylander = null;
                                        tempSkylander = null;
                                        tempVariant = CreateInstance<SkylanderVariant>();
                                        tempVariant.name = variant.name;
                                        tempVariant.Name = variant.Name;
                                        tempVariant.Tag = variant.Tag;
                                        tempVariant.VariantID = variant.VariantID;
                                        tempVariant.NameOverride = variant.NameOverride;
                                        desiredDecoIDEnumType = selectedSkylander.DecoIDEnumType;

                                        editingVariant = variant;
                                        GUI.FocusControl(null);

                                        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                        Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

                                        var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null);
                                        enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

                                        var findTarget = enumTypes.Where(x => x.Name == desiredDecoIDEnumType).FirstOrDefault();
                                        if (findTarget != null && findTarget != default)
                                        {
                                            if (Enum.GetValues(findTarget).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                                presetDecoIDName = Enum.GetName(findTarget, tempVariant.VariantID.DecoID);
                                            else
                                            {
                                                foreach (Type type in enumTypes)
                                                {
                                                    if (type.Name == desiredDecoIDEnumType) continue;

                                                    if (Enum.GetValues(type).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                                        presetDecoIDName = Enum.GetName(type, tempVariant.VariantID.DecoID);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (Type type in enumTypes)
                                            {
                                                if (type.Name == desiredDecoIDEnumType) continue;

                                                if (Enum.GetValues(type).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                                {
                                                    desiredDecoIDEnumType = type.Name;
                                                    presetDecoIDName = Enum.GetName(type, tempVariant.VariantID.DecoID);
                                                }
                                            }
                                        }
                                    });

                                    modifyMenu.AddItem(new GUIContent("Delete"), false, () =>
                                    {
                                        if (variant.AskForDeletion())
                                        {
                                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variant));
                                            AssetDatabase.Refresh();

                                            selectedSkylander.Variants.RemoveAll(x => x == variant);
                                        }
                                    });
                                    modifyMenu.ShowAsContext();
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.LabelField("Name", variant.Name);
                                EditorGUILayout.LabelField("Tag", variant.Tag);
                                EditorGUILayout.LabelField("Year Code", variant.VariantID.YearCode.ToString());
                                EditorGUILayout.LabelField("Is Repose", variant.VariantID.IsRepose.ToString());
                                EditorGUILayout.LabelField("Is Alt Deco", variant.VariantID.IsAltDeco.ToString());
                                EditorGUILayout.LabelField("Is LightCore", variant.VariantID.IsLightCore.ToString());
                                EditorGUILayout.LabelField("Is SuperCharger", variant.VariantID.IsSuperCharger.ToString());
                                EditorGUILayout.LabelField("DecoID", $"{variant.VariantID.DecoID} ({(DecoID)variant.VariantID.DecoID})");
                                EditorGUILayout.LabelField("Name Override", variant.NameOverride);
                                EditorGUILayout.LabelField("Encoded VariantID", variant.VariantID.Encode().ToString());
                                EditorGUILayout.EndVertical();

                                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1 && mouseOverWindow == this)
                                {
                                    GenericMenu menu = new GenericMenu();
                                    menu.AddItem(new GUIContent("Focus in Inspector"), false, () => Selection.activeObject = variant);
                                    menu.AddItem(new GUIContent("Show Variant in Explorer"), false, () => EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(variant)));
                                    menu.AddItem(new GUIContent("Delete"), false, () =>
                                    {
                                        if (variant.AskForDeletion())
                                        {
                                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variant));
                                            AssetDatabase.Refresh();

                                            selectedSkylander.Variants.RemoveAll(x => x == variant);
                                        }
                                    });
                                    menu.ShowAsContext();
                                }
                                GUILayout.Space(6);
                            }
                            else
                            {
                                EditorGUILayout.EndHorizontal();
                                GUILayout.Space(4);
                            }
                        }
                        else
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(11);
                            tempVariant.name = EditorGUILayout.TextField(tempVariant.name, GUILayout.Width(150));

                            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

                            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null);
                            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

                            foreach (Type type in enumTypes)
                                enums.Add(Enum.GetValues(type).GetValue(0).GetType());

                            GUILayout.FlexibleSpace();

                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(2);

                            bool cancelPress = GUILayout.Button("Cancel", GUILayout.Width(60));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                            if (cancelPress)
                            {
                                GUI.FocusControl(null);
                                editingVariant = null;
                                tempVariant = null;
                            }
                            else
                            {
                                if (foldoutStates[variant])
                                {
                                    EditorGUILayout.BeginVertical("box");
                                    tempVariant.Name = EditorGUILayout.TextField("Name", tempVariant.Name);
                                    tempVariant.Tag = EditorGUILayout.TextField("Tag", tempVariant.Tag);
                                    EditorGUILayout.Space();
                                    
                                    tempVariant.VariantID.YearCode = (SkylandersGame)EditorGUILayout.EnumPopup("Year Code", tempVariant.VariantID.YearCode);
                                    tempVariant.VariantID.IsRepose = EditorGUILayout.Toggle("Is Repose", tempVariant.VariantID.IsRepose);
                                    tempVariant.VariantID.IsAltDeco = EditorGUILayout.Toggle("Is Alt Deco", tempVariant.VariantID.IsAltDeco);
                                    tempVariant.VariantID.IsLightCore = EditorGUILayout.Toggle("Is LightCore", tempVariant.VariantID.IsLightCore);
                                    tempVariant.VariantID.IsSuperCharger = EditorGUILayout.Toggle("Is SuperCharger", tempVariant.VariantID.IsSuperCharger);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Preset Deco ID", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

                                    string discoveredEnum = typeof(DecoID).Name;

                                    var findTarget = enumTypes.Where(x => x.Name == desiredDecoIDEnumType).FirstOrDefault();
                                    if (findTarget != null && findTarget != default)
                                    {
                                        if (Enum.GetValues(findTarget).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                        {
                                            discoveredEnum = findTarget.Name;
                                            presetDecoIDName = Enum.GetName(findTarget, tempVariant.VariantID.DecoID);
                                        }
                                        else
                                        {
                                            foreach (Type type in enumTypes)
                                            {
                                                if (type.Name == desiredDecoIDEnumType) continue;

                                                if (Enum.GetValues(type).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                                {
                                                    discoveredEnum = type.Name;
                                                    presetDecoIDName = Enum.GetName(type, tempVariant.VariantID.DecoID);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (Type type in enumTypes)
                                        {
                                            if (type.Name == desiredDecoIDEnumType) continue;

                                            if (Enum.GetValues(type).Cast<int>().Contains(tempVariant.VariantID.DecoID))
                                            {
                                                desiredDecoIDEnumType = type.Name;
                                                presetDecoIDName = Enum.GetName(type, tempVariant.VariantID.DecoID);
                                            }
                                        }
                                    }

                                    if (GUILayout.Button(presetDecoIDName, EditorStyles.popup))
                                    {
                                        DecoIDPicker picker = CreateInstance<DecoIDPicker>();
                                        picker.onSelection = (x, y) =>
                                        {
                                            tempVariant.VariantID.DecoID = (byte)x;
                                            desiredDecoIDEnumType = y;
                                        };
                                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    tempVariant.VariantID.DecoID = (byte)Mathf.Clamp(EditorGUILayout.IntField("Deco ID", tempVariant.VariantID.DecoID), 0, byte.MaxValue);
                                    EditorGUILayout.Space();
                                    tempVariant.NameOverride = EditorGUILayout.TextField("Name Override", tempVariant.NameOverride);
                                    EditorGUILayout.LabelField("Encoded VariantID", tempVariant.VariantID.Encode().ToString());
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();

                                    if (GUILayout.Button("Save", GUILayout.Width(85)))
                                    {
                                        string path = AssetDatabase.GetAssetPath(variant);
                                        AssetDatabase.RenameAsset(path, tempVariant.name);
                                        AssetDatabase.SaveAssets();

                                        variant.Name = tempVariant.Name;
                                        variant.Tag = tempVariant.Tag;
                                        variant.VariantID = tempVariant.VariantID;
                                        variant.NameOverride = tempVariant.NameOverride;
                                        editingVariant = null;
                                        tempVariant = null;
                                        selectedSkylander.DecoIDEnumType = desiredDecoIDEnumType;
                                        GUI.FocusControl(null);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) {Debug.LogException(ex);}

                if (GUILayout.Button("New Variant...", GUILayout.Width(138)))
                {
                    SkylanderVariantCreatorWindow.ShowWindow(selectedSkylander);
                    SkylanderVariantCreatorWindow.OnVariantCreated = (x) =>
                    {
                        editingSkylander = null;
                        editingVariant = null;
                    };
                }
                GUILayout.Space(6);
            }
            else
                EditorGUILayout.LabelField("Select a Skylander", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Skylanders", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Figure" : "Portal-To-Unity/Editor/Icons/Figure"));
            LoadSkylanders();
            InspectSkylanders();
        }

        private void OnDisable()
        {
            refreshingSkylanders = false;           
        }

        private async void LoadSkylanders()
        {
            refreshingSkylanders = true;
            while (refreshingSkylanders)
            {
                skylanders = Resources.LoadAll<Skylander>("Portal-To-Unity/Figures/").ToList();
                await Task.Delay(750);
            }
            refreshingSkylanders = false;
        }

        private void ShowContextOption(string text, string path, Skylander file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Focus in Inspector"), false, () => Selection.activeObject = file);
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (file.AskForDeletion())
                {
                    foreach (SkylanderVariant variant in file.Variants)
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variant));

                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(file));
                    AssetDatabase.Refresh();
                }
            });
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path, Skylander file) => EditorUtility.RevealInFinder(path + file.name + ".asset");

        private void InspectSkylanders()
        {
            List<SkylanderVariant> skylanderVariants = new List<SkylanderVariant>(Resources.LoadAll<SkylanderVariant>("Portal-To-Unity/Figures/"));
            Debug.Log($"Loaded {skylanders.Count} Skylanders and {skylanderVariants.Count} variants (approx. {skylanders.Count + skylanderVariants.Count} unique figures)");

            foreach (Skylander skylander in skylanders)
            {
                foreach (SkylanderVariant variant in skylander.Variants)
                {
                    if (skylanderVariants.Contains(variant))
                        skylanderVariants.Remove(variant);
                }
            }

            if (skylanderVariants.Count > 0)
            {
                Debug.LogError("Unreferenced Skylander variants:");
                foreach (SkylanderVariant unreferencedVariant in skylanderVariants)
                    Debug.LogError(unreferencedVariant.name);
            }
        }
    }
}
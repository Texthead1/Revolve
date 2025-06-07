using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public class VillainManagerWindow : EditorWindow
    {
        private List<Villain> villains;
        private Villain selectedVillain;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private float leftPanelWidth = 0.3333f;
        private bool refreshingVillains;
        private string searchQuery = string.Empty;

        [MenuItem("Portal-To-Unity/Collections/Villains", false, 34)]
        public static void ShowWindow()
        {
            VillainManagerWindow window = GetWindow<VillainManagerWindow>("Villains");
            window.titleContent = new GUIContent("Villains", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Trap" : "Portal-To-Unity/Editor/Icons/Trap"));
        }

        public void OnGUI()
        {
            if (!refreshingVillains)
                LoadVillains();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (villains == null) return;

            villains = villains.OrderBy(x => x.VillainID).ToList();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));

            EditorGUILayout.BeginHorizontal();
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                VillainCreatorWindow.ShowWindow(searchQuery);
            }
            EditorGUILayout.EndHorizontal();

            List<Villain> villainsFiltered = new List<Villain>(villains);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("element:", StringComparison.OrdinalIgnoreCase))
                    villainsFiltered.RemoveAll(x => !x.Element.ToString().Contains(search.Replace("element:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                    villainsFiltered.RemoveAll(x => !((int)x.VillainID).ToString().Contains(search.Replace("id:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    villainsFiltered.RemoveAll(x => !x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (Villain villain in villainsFiltered)
            {
                bool button = GUILayout.Button(villain.Name);
                e = Event.current;

                if (!mouseOverWindow) continue;

                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                    ShowContextOption("Show Villain in Explorer", "Assets/Resources/Portal-To-Unity/Villains/", villain);
                else if (button && e.button != 1)
                    selectedVillain = villain;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedVillain == null)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Villains/");
                else
                    ShowContextOption("Show Villain in Explorer", "Assets/Resources/Portal-To-Unity/Villains/", selectedVillain);
            }

            EditorGUILayout.Space();
            if (villains.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Villains found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Villain/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Villains/");

                selectedVillain = null;
            }
            else if (selectedVillain != null)
            {
                EditorGUILayout.LabelField($"Villain Info ({selectedVillain.name})", boldStyle);
                EditorGUILayout.LabelField("Name", selectedVillain.Name);
                EditorGUILayout.LabelField("Villain ID", $"{selectedVillain.VillainID} ({(int)selectedVillain.VillainID})");
                EditorGUILayout.LabelField("Element", selectedVillain.Element.ToString());
                EditorGUILayout.Space();

                try
                {
                    VillainVariant variant = selectedVillain.Variant;

                    if (variant != null)
                    {
                        EditorGUILayout.LabelField($"Variant ({variant.name})", boldStyle);

                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Variant Name", variant.Name);
                        EditorGUILayout.LabelField("Name Override", variant.NameOverride);
                        string fullName = $"{variant.Name} {(variant.NameOverride != string.Empty && variant.NameOverride != null ? variant.NameOverride : selectedVillain.Name)}";
                        EditorGUILayout.LabelField("Full Name", fullName);
                        EditorGUILayout.EndVertical();

                        if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1 && mouseOverWindow == this)
                            ShowContextOption("Show Variant in Explorer", AssetDatabase.GetAssetPath(variant));
                    }
                    else
                        EditorGUILayout.LabelField("Variant", boldStyle);
                }
                catch (Exception) {}
            }
            else
                EditorGUILayout.LabelField("Select a Villain", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Villains", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Trap" : "Portal-To-Unity/Editor/Icons/Trap"));
            LoadVillains();
            InspectVillains();
        }

        private void OnDisable()
        {
            refreshingVillains = false;
        }

        private async void LoadVillains()
        {
            refreshingVillains = true;
            while (refreshingVillains)
            {
                villains = Resources.LoadAll<Villain>("Portal-To-Unity/Villains").ToList();
                await Task.Delay(750);
            }
            refreshingVillains = false;
        }

        private void ShowContextOption(string text, string path)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path));
            menu.ShowAsContext();
        }

        private void ShowContextOption(string text, string path, Villain file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path) => EditorUtility.RevealInFinder(path);

        private void OpenFileLocation(string path, Villain file) => OpenFileLocation(path + file.name + ".asset");

        private void InspectVillains()
        {
            HashSet<VillainVariant> villainVariants = new HashSet<VillainVariant>(Resources.LoadAll<VillainVariant>("Portal-To-Unity/Villains/"));
            Debug.Log($"Loaded {villains.Count} Villains and {villainVariants.Count} variants");

            foreach (Villain villain in villains)
            {
                if (villain.Variant != null)
                {
                    if (villainVariants.Contains(villain.Variant))
                        villainVariants.Remove(villain.Variant);
                }
            }

            if (villainVariants.Count > 0)
            {
                Debug.LogError("Unreferenced Villain variants:");
                foreach (VillainVariant unreferencedVariant in villainVariants)
                    Debug.LogError(unreferencedVariant.name);
            }
        }
    }
}
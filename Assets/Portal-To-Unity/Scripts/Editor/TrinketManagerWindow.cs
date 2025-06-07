using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public class TrinketManagerWindow : EditorWindow
    {
        private List<Trinket> trinkets;
        private Trinket selectedTrinket;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private float leftPanelWidth = 0.3333f;
        private bool refreshingTrinkets;
        private string searchQuery = string.Empty;

        [MenuItem("Portal-To-Unity/Collections/Trinkets", false, 32)]
        public static void ShowWindow()
        {
            TrinketManagerWindow window = GetWindow<TrinketManagerWindow>("Trinkets");
            window.titleContent = new GUIContent("Trinkets", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/d_placeholder"));
        }

        public void OnGUI()
        {
            if (!refreshingTrinkets)
                LoadTrinketInfos();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (trinkets == null) return;

            trinkets = trinkets.OrderBy(x => x.ID).ToList();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));

            EditorGUILayout.BeginHorizontal();

            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                TrinketCreatorWindow.ShowWindow(searchQuery);
            }
            EditorGUILayout.EndHorizontal();

            List<Trinket> trinketsFiltered = new List<Trinket>(trinkets);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                    trinketsFiltered.RemoveAll(x => !((int)x.ID).ToString().Contains(search.Replace("id:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    trinketsFiltered.RemoveAll(x => !x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (Trinket trinket in trinketsFiltered)
            {
                bool button = GUILayout.Button(trinket.Name);
                e = Event.current;

                if (!mouseOverWindow) continue;

                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                    ShowContextOption("Show Trinket in Explorer", "Assets/Resources/Portal-To-Unity/Trinkets/", trinket);
                else if (button && e.button != 1)
                    selectedTrinket = trinket;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedTrinket == null)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Trinkets/");
                else
                    ShowContextOption("Show Trinket in Explorer", "Assets/Resources/Portal-To-Unity/Trinkets/", selectedTrinket);
            }

            EditorGUILayout.Space();
            if (trinkets.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Trinkets found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Trinkets/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Trinkets/");

                selectedTrinket = null;
            }
            else if (selectedTrinket != null)
            {
                EditorGUILayout.LabelField($"Trinket Info ({selectedTrinket.name})", boldStyle);
                EditorGUILayout.LabelField("Name", selectedTrinket.Name);
                EditorGUILayout.LabelField("ID", ((int)selectedTrinket.ID).ToString());
            }
            else
                EditorGUILayout.LabelField("Select a Trinket", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Trinkets", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/placeholder"));
            LoadTrinketInfos();
            InspectTrinketInfos();
        }

        private void OnDisable()
        {
            refreshingTrinkets = false;
        }

        private async void LoadTrinketInfos()
        {
            refreshingTrinkets = true;
            while (refreshingTrinkets)
            {
                trinkets = Resources.LoadAll<Trinket>("Portal-To-Unity/Trinkets").ToList();
                await Task.Delay(750);
            }
            refreshingTrinkets = false;
        }

        private void ShowContextOption(string text, string path)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path));
            menu.ShowAsContext();
        }

        private void ShowContextOption(string text, string path, Trinket file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path) => EditorUtility.RevealInFinder(path);

        private void OpenFileLocation(string path, Trinket file) => OpenFileLocation(path + file.name + ".asset");

        private void InspectTrinketInfos()
        {
            Debug.Log($"Loaded {trinkets.Count} Trinkets");
        }
    }
}
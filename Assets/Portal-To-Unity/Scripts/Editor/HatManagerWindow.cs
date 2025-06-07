using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public class HatManagerWindow : EditorWindow
    {
        private List<Hat> hats;
        private Hat selectedHat;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private float leftPanelWidth = 0.3333f;
        private bool refreshingHats;
        private string searchQuery = string.Empty;
        private string presetHatEnumName = typeof(HatID).Name;

        [MenuItem("Portal-To-Unity/Collections/Hats", false, 31)]
        public static void ShowWindow()
        {
            HatManagerWindow window = GetWindow<HatManagerWindow>("Hats");
            window.titleContent = new GUIContent("Hats", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/d_placeholder"));
        }

        public void OnGUI()
        {
            if (!refreshingHats)
                LoadHatInfos();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (hats == null) return;

            hats = hats.OrderBy(x => x.ID).ToList();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));

            EditorGUILayout.BeginHorizontal();
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                HatCreatorWindow.ShowWindow(searchQuery);
            }
            EditorGUILayout.EndHorizontal();

            List<Hat> hatsFiltered = new List<Hat>(hats);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                    hatsFiltered.RemoveAll(x => !((int)x.ID).ToString().Contains(search.Replace("id:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    hatsFiltered.RemoveAll(x => !x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (Hat hat in hatsFiltered)
            {
                bool button = GUILayout.Button(hat.Name);
                e = Event.current;

                if (!mouseOverWindow) continue;

                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                {
                    ShowContextOption("Show Hat in Explorer", "Assets/Resources/Portal-To-Unity/Hats/", hat);
                }
                else if (button && e.button != 1)
                    selectedHat = hat;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedHat == null)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Hats/");
                else
                    ShowContextOption("Show Hat in Explorer", "Assets/Resources/Portal-To-Unity/Hats/", selectedHat);
            }

            EditorGUILayout.Space();
            if (hats.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Hats found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Hats/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Hats/");

                selectedHat = null;
                return;
            }
            else if (selectedHat != null)
            {
                EditorGUILayout.LabelField($"Hat Info ({selectedHat.name})", boldStyle);
                EditorGUILayout.LabelField("Name", selectedHat.Name);
                EditorGUILayout.LabelField("ID", ((int)selectedHat.ID).ToString());
            }
            else
                EditorGUILayout.LabelField("Select a Hat", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Hats", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/placeholder"));
            LoadHatInfos();
            InspectHatInfos();
        }

        private void OnDisable()
        {
            refreshingHats = false;
        }

        private async void LoadHatInfos()
        {
            refreshingHats = true;
            while (refreshingHats)
            {
                hats = Resources.LoadAll<Hat>("Portal-To-Unity/Hats").ToList();
                await Task.Delay(750);
            }
            refreshingHats = false;
        }

        private void ShowContextOption(string text, string path)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path));
            menu.ShowAsContext();
        }

        private void ShowContextOption(string text, string path, Hat file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path) => EditorUtility.RevealInFinder(path);

        private void OpenFileLocation(string path, Hat file) => OpenFileLocation(path + file.name + ".asset");

        private void InspectHatInfos()
        {
            Debug.Log($"Loaded {hats.Count} Hats");
        }
    }
}
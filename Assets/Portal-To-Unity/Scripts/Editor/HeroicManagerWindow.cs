using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public class HeroicManagerWindow : EditorWindow
    {
        private List<HeroicChallenge> heroics;
        private HeroicChallenge selectedHeroic;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private float leftPanelWidth = 0.3333f;
        private bool refreshingHeroics;
        private string searchQuery = string.Empty;

        [MenuItem("Portal-To-Unity/Collections/Heroic Challenges", false, 33)]
        public static void ShowWindow()
        {
            HeroicManagerWindow window = GetWindow<HeroicManagerWindow>("Heroic Challenges");
            window.titleContent = new GUIContent("Heroic Challenges", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/d_placeholder"));
        }

        public void OnGUI()
        {
            if (!refreshingHeroics)
                LoadHeroicsInfos();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (heroics == null) return;

            heroics = heroics.OrderBy(x => x.ID).ToList();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));

            EditorGUILayout.BeginHorizontal();
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                HeroicCreatorWindow.ShowWindow(searchQuery);
            }
            EditorGUILayout.EndHorizontal();

            List<HeroicChallenge> heroicsFiltered = new List<HeroicChallenge>(heroics);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                    heroicsFiltered.RemoveAll(x => !((int)x.ID).ToString().Contains(search.Replace("id:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("reward:", StringComparison.OrdinalIgnoreCase))
                    heroicsFiltered.RemoveAll(x => !x.RewardType.ToString().Contains(search.Replace("reward:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    heroicsFiltered.RemoveAll(x => !x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (HeroicChallenge heroic in heroicsFiltered)
            {
                bool button = GUILayout.Button(heroic.Name);
                e = Event.current;

                if (!mouseOverWindow) continue;

                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                {
                    ShowContextOption("Show Heroic Challenge in Explorer", "Assets/Resources/Portal-To-Unity/Heroics/", heroic);
                }
                else if (button && e.button != 1)
                    selectedHeroic = heroic;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedHeroic == null)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Heroics/");
                else
                    ShowContextOption("Show Heroic Challenge in Explorer", "Assets/Resources/Portal-To-Unity/Heroics/", selectedHeroic);
            }

            EditorGUILayout.Space();
            if (heroics.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Heroic Challenges found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Heroics/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Heroics/");

                selectedHeroic = null;
            }
            else if (selectedHeroic != null)
            {
                EditorGUILayout.LabelField($"Heroic Challenge Info ({selectedHeroic.name})", boldStyle);
                EditorGUILayout.LabelField("Name", selectedHeroic.Name);
                EditorGUILayout.LabelField("ID", ((int)selectedHeroic.ID).ToString());
                EditorGUILayout.LabelField("Reward Type", selectedHeroic.RewardType.ToString());
            }
            else
                EditorGUILayout.LabelField("Select a Heroic Challenge", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Heroic Challenges", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/placeholder"));
            LoadHeroicsInfos();
            InspectHeroicsInfos();
        }

        private void OnDisable()
        {
            refreshingHeroics = false;
        }

        private async void LoadHeroicsInfos()
        {
            refreshingHeroics = true;
            while (refreshingHeroics)
            {
                heroics = Resources.LoadAll<HeroicChallenge>("Portal-To-Unity/Heroics").ToList();
                await Task.Delay(750);
            }
            refreshingHeroics = false;
        }

        private void ShowContextOption(string text, string path)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path));
            menu.ShowAsContext();
        }

        private void ShowContextOption(string text, string path, HeroicChallenge file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path) => EditorUtility.RevealInFinder(path);

        private void OpenFileLocation(string path, HeroicChallenge file) => OpenFileLocation(path + file.name + ".asset");

        private void InspectHeroicsInfos()
        {
            Debug.Log($"Loaded {heroics.Count} Heroic Challenges");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class PortalInfoManagerWindow : EditorWindow
    {
        private List<PortalInfo> portals;
        private PortalInfo selectedPortal;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private float leftPanelWidth = 0.3333f;
        private bool refreshingPortals;
        private string searchQuery = string.Empty;

        [MenuItem("Portal-To-Unity/Collections/Portal Infos", false, 35)]
        public static void ShowWindow()
        {
            PortalInfoManagerWindow window = GetWindow<PortalInfoManagerWindow>("Portals");
            window.titleContent = new GUIContent("Portals", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Portal" : "Portal-To-Unity/Editor/Icons/Portal"));
        }

        public void OnGUI()
        {
            if (!refreshingPortals)
                LoadPortalInfos();

            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            Event e;

            if (portals == null) return;

            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUILayout.Width(position.width * leftPanelWidth));

            EditorGUILayout.BeginHorizontal();
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
                leftScrollPos.y = 0;

            searchQuery = newSearchQuery;

            /*if (GUILayout.Button("Create", GUILayout.Width(75)))
            {
                PortalInfoCreatorWindow.ShowWindow(searchQuery);
            }*/
            EditorGUILayout.EndHorizontal();

            List<PortalInfo> portalsFiltered = new List<PortalInfo>(portals);
            if (searchQuery.Replace(" ", string.Empty) != string.Empty)
            {
                string search = searchQuery.Trim(' ');
                if (search.StartsWith("led:", StringComparison.OrdinalIgnoreCase))
                    portalsFiltered.RemoveAll(x => !x.LEDType.ToString().Contains(search.Replace("led:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("tags:", StringComparison.OrdinalIgnoreCase))
                    portalsFiltered.RemoveAll(x => !x.MaxSimultaneousRFIDTags.ToString().Contains(search.Replace("tags:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else if (search.StartsWith("hardware:", StringComparison.OrdinalIgnoreCase))
                    portalsFiltered.RemoveAll(x => !FlagsToString(x.AdditionalHardwarePieces).Contains(search.Replace("hardware:", string.Empty).Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase));
                else
                    portalsFiltered.RemoveAll(x => !x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            foreach (PortalInfo portal in portalsFiltered)
            {
                bool button = GUILayout.Button(portal.Name);
                e = Event.current;

                if (!mouseOverWindow) continue;

                if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition) && e.button == 1)
                {
                    ShowContextOption("Show Portal Info in Explorer", "Assets/Resources/Portal-To-Unity/Portals/", portal);
                }
                else if (button && e.button != 1)
                    selectedPortal = portal;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUILayout.Width(position.width * (1 - leftPanelWidth) - 10));

            e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                if (selectedPortal == null)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Portals/");
                else
                    ShowContextOption("Show Portal Info in Explorer", "Assets/Resources/Portal-To-Unity/Portals/", selectedPortal);
            }

            EditorGUILayout.Space();
            if (portals.Count == 0)
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No Portal Infos found. Add some to the database by putting them at \"Assets/Resources/Portal-To-Unity/Portals/\"", boldStyle);
                e = Event.current;

                if (e.type == EventType.ContextClick)
                    ShowContextOption("Show in Explorer", "Assets/Resources/Portal-To-Unity/Portals/");

                selectedPortal = null;
            }
            else if (selectedPortal != null)
            {
                EditorGUILayout.LabelField($"Portal Info ({selectedPortal.name})", boldStyle);
                EditorGUILayout.LabelField("Name", selectedPortal.Name);
                EditorGUILayout.LabelField("ID", BytesToHexString(selectedPortal.ID));
                EditorGUILayout.LabelField("LED Type", selectedPortal.LEDType.ToString());
                EditorGUILayout.LabelField("Max Tags", selectedPortal.MaxSimultaneousRFIDTags.ToString());
                EditorGUILayout.LabelField("Supported Commands", string.Join(" ", selectedPortal.SupportedCommands));
                EditorGUILayout.LabelField("Additional Hardware", FlagsToString(selectedPortal.AdditionalHardwarePieces));
            }
            else
                EditorGUILayout.LabelField("Select a Portal", boldStyle);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Portals", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_Portal" : "Portal-To-Unity/Editor/Icons/Portal"));
            LoadPortalInfos();
            InspectPortalInfos();
        }

        private void OnDisable()
        {
            refreshingPortals = false;
        }

        private async void LoadPortalInfos()
        {
            refreshingPortals = true;
            while (refreshingPortals)
            {
                portals = Resources.LoadAll<PortalInfo>("Portal-To-Unity/Portals").ToList();
                await Task.Delay(750);
            }
            refreshingPortals = false;
        }

        private void ShowContextOption(string text, string path)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path));
            menu.ShowAsContext();
        }

        private void ShowContextOption(string text, string path, PortalInfo file)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(text), false, () => OpenFileLocation(path, file));
            menu.ShowAsContext();
        }

        private void OpenFileLocation(string path) => EditorUtility.RevealInFinder(path);

        private void OpenFileLocation(string path, PortalInfo file) => OpenFileLocation(path + file.name + ".asset");

        private void InspectPortalInfos()
        {
            Debug.Log($"Loaded {portals.Count} Portal Infos");
        }
    }
}
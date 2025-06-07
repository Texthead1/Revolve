using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class HeroicCreatorWindow : EditorWindow
    {
        public static Action<HeroicChallenge> OnHeroicCreated;

        private HeroicChallenge workingHeroic = null;
        private string presetHeroicName = Enum.GetNames(typeof(HeroicChallengeID)).Where(x => Enum.Parse<HeroicChallengeID>(x) != 0).FirstOrDefault();

        public static void ShowWindow(string newName)
        {
            HeroicCreatorWindow window = GetWindow<HeroicCreatorWindow>("Create Heroic Challenge");
            window.workingHeroic = CreateInstance<HeroicChallenge>();
            window.workingHeroic.Name = newName;
            window.Show();
        }

        private void OnGUI()
        {
            if (workingHeroic == null)
                workingHeroic = CreateInstance<HeroicChallenge>();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            GUILayout.Label("New Heroic Challenge", boldStyle);
            workingHeroic.name = EditorGUILayout.TextField("File Name", workingHeroic.name);
            EditorGUILayout.Space();
            workingHeroic.Name = EditorGUILayout.TextField("Name", workingHeroic.Name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset Heroic", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsHeroicChallengeIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            string discoveredEnum = typeof(HeroicChallengeID).Name;

            var findTarget = enumTypes.Where(x => x.Name == workingHeroic.HeroicIDEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(workingHeroic.ID))
                {
                    discoveredEnum = findTarget.Name;
                    presetHeroicName = Enum.GetName(findTarget, (int)workingHeroic.ID);
                }
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == workingHeroic.HeroicIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingHeroic.ID))
                        {
                            discoveredEnum = type.Name;
                            presetHeroicName = Enum.GetName(type, (int)workingHeroic.ID);
                        }
                    }
                }
            }
            else
            {
                foreach (Type type in enumTypes)
                {
                    if (type.Name == workingHeroic.HeroicIDEnumType) continue;

                    if (Enum.GetValues(type).Cast<int>().Contains(workingHeroic.ID))
                    {
                        workingHeroic.HeroicIDEnumType = type.Name;
                        presetHeroicName = Enum.GetName(type, (int)workingHeroic.ID);
                    }
                }
            }

            if (GUILayout.Button(presetHeroicName, EditorStyles.popup))
            {
                HeroicPicker picker = CreateInstance<HeroicPicker>();
                picker.onSelection = (x, y) =>
                { 
                    workingHeroic.ID = (ushort)x;
                    workingHeroic.HeroicIDEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingHeroic.ID = (ushort)Mathf.Clamp(EditorGUILayout.IntField("Heroic ID", workingHeroic.ID), 0, ushort.MaxValue);
            EditorGUILayout.Space();
            workingHeroic.RewardType = (RewardType)EditorGUILayout.EnumPopup("Reward Type", workingHeroic.RewardType);

            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Heroic Challenge")) return;

            string newFileName = workingHeroic.name.Trim();

            if (newFileName == string.Empty)
                newFileName = "NewHeroic";

            string path = $"Assets/Resources/Portal-To-Unity/Heroics/{(int)workingHeroic.ID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Heroic Challenge with file name {newFileName} already exists. Please use a different name instead");
                return;
            }
            AssetDatabase.CreateAsset(workingHeroic, path);
            AssetDatabase.SaveAssets();
            OnHeroicCreated?.Invoke(workingHeroic);
            Close();
        }
    }
}
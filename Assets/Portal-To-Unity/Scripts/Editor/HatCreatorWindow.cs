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
    public class HatCreatorWindow : EditorWindow
    {
        public static Action<Hat> OnHatCreated;

        private Hat workingHat = null;
        private string presetHatName = Enum.GetNames(typeof(HatID)).Where(x => Enum.Parse<HatID>(x) != 0).FirstOrDefault();

        public static void ShowWindow(string newName)
        {
            HatCreatorWindow window = GetWindow<HatCreatorWindow>("Create Hat");
            window.workingHat = CreateInstance<Hat>();
            window.workingHat.Name = newName;
            window.workingHat.ID = 1;
            window.Show();
        }

        private void OnGUI()
        {
            if (workingHat == null)
            {
                workingHat = CreateInstance<Hat>();
                workingHat.ID = 1;
            }

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            GUILayout.Label("New Hat", boldStyle);
            workingHat.name = EditorGUILayout.TextField("File Name", workingHat.name);
            EditorGUILayout.Space();
            workingHat.Name = EditorGUILayout.TextField("Name", workingHat.Name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset Hat", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsHatIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            string discoveredEnum = typeof(HatID).Name;

            var findTarget = enumTypes.Where(x => x.Name == workingHat.HatIDEnumType).FirstOrDefault();
            if (workingHat.ID != 0)
            {
                if (findTarget != null && findTarget != default)
                {
                    if (Enum.GetValues(findTarget).Cast<int>().Contains(workingHat.ID))
                    {
                        discoveredEnum = findTarget.Name;
                        presetHatName = Enum.GetName(findTarget, (int)workingHat.ID);
                    }
                    else
                    {
                        foreach (Type type in enumTypes)
                        {
                            if (type.Name == workingHat.HatIDEnumType) continue;

                            if (Enum.GetValues(type).Cast<int>().Contains(workingHat.ID))
                            {
                                discoveredEnum = type.Name;
                                presetHatName = Enum.GetName(type, (int)workingHat.ID);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == workingHat.HatIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingHat.ID))
                        {
                            workingHat.HatIDEnumType = type.Name;
                            presetHatName = Enum.GetName(type, (int)workingHat.ID);
                        }
                    }
                }
            }

            if (GUILayout.Button(presetHatName, EditorStyles.popup))
            {
                HatPicker picker = CreateInstance<HatPicker>();
                picker.onSelection = (x, y) =>
                { 
                    workingHat.ID = (ushort)x;
                    workingHat.HatIDEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingHat.ID = (ushort)Mathf.Clamp(EditorGUILayout.IntField("Hat ID", workingHat.ID), 0, ushort.MaxValue);
            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Hat")) return;

            workingHat.HatIDEnumType = discoveredEnum;

            if (workingHat.ID == 0)
            {
                Debug.LogError("Cannot create Hat with ID 0. ID is reserved.");
                return;
            }

            string newFileName = workingHat.name.Trim();

            if (newFileName == string.Empty)
                newFileName = "NewHat";

            string path = $"Assets/Resources/Portal-To-Unity/Hats/{(int)workingHat.ID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Hat with file name {newFileName} already exists. Please use a different name instead.");
                return;
            }
            AssetDatabase.CreateAsset(workingHat, path);
            AssetDatabase.SaveAssets();
            OnHatCreated?.Invoke(workingHat);
            Close();
        }
    }
}
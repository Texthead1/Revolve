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
    public class TrinketCreatorWindow : EditorWindow
    {
        public static Action<Trinket> OnTrinketCreated;

        private Trinket workingTrinket = null;
        private string presetTrinketName = Enum.GetNames(typeof(TrinketID)).Where(x => Enum.Parse<TrinketID>(x) != 0).FirstOrDefault();

        public static void ShowWindow(string newName)
        {
            TrinketCreatorWindow window = GetWindow<TrinketCreatorWindow>("Create Trinket");
            window.workingTrinket = CreateInstance<Trinket>();
            window.workingTrinket.Name = newName;
            window.workingTrinket.ID = 1;
            window.Show();
        }

        private void OnGUI()
        {
            if (workingTrinket == null)
            {
                workingTrinket = CreateInstance<Trinket>();
                workingTrinket.ID = 1;
            }
            
            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            GUILayout.Label("New Trinket", boldStyle);
            workingTrinket.name = EditorGUILayout.TextField("File Name", workingTrinket.name);
            EditorGUILayout.Space();
            workingTrinket.Name = EditorGUILayout.TextField("Name", workingTrinket.Name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset Trinket", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsTrinketIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            string discoveredEnum = typeof(TrinketID).Name;

            var findTarget = enumTypes.Where(x => x.Name == workingTrinket.TrinketIDEnumType).FirstOrDefault();
            if (workingTrinket.ID != 0)
            {
                if (findTarget != null && findTarget != default)
                {
                    if (Enum.GetValues(findTarget).Cast<int>().Contains(workingTrinket.ID))
                    {
                        discoveredEnum = findTarget.Name;
                        presetTrinketName = Enum.GetName(findTarget, (int)workingTrinket.ID);
                    }
                    else
                    {
                        foreach (Type type in enumTypes)
                        {
                            if (type.Name == workingTrinket.TrinketIDEnumType) continue;

                            if (Enum.GetValues(type).Cast<int>().Contains(workingTrinket.ID))
                            {
                                discoveredEnum = type.Name;
                                presetTrinketName = Enum.GetName(type, (int)workingTrinket.ID);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == workingTrinket.TrinketIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingTrinket.ID))
                        {
                            workingTrinket.TrinketIDEnumType = type.Name;
                            presetTrinketName = Enum.GetName(type, (int)workingTrinket.ID);
                        }
                    }
                }
            }

            if (GUILayout.Button(presetTrinketName, EditorStyles.popup))
            {
                TrinketPicker picker = CreateInstance<TrinketPicker>();
                picker.onSelection = (x, y) =>
                {
                    workingTrinket.ID = (byte)x;
                    workingTrinket.TrinketIDEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingTrinket.ID = (byte)Mathf.Clamp(EditorGUILayout.IntField("Trinket ID", workingTrinket.ID), 0, byte.MaxValue);
            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Trinket")) return;

            workingTrinket.TrinketIDEnumType = discoveredEnum;

            if (workingTrinket.ID == 0)
            {
                Debug.LogError("Cannot create Trinket with ID 0. ID is reserved.");
                return;
            }

            string newFileName = workingTrinket.name.Trim();

            if (newFileName == string.Empty)
                newFileName = "NewTrinket";

            string path = $"Assets/Resources/Portal-To-Unity/Trinkets/{(int)workingTrinket.ID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Trinket with file name {newFileName} already exists. Please use a different name instead");
                return;
            }
            AssetDatabase.CreateAsset(workingTrinket, path);
            AssetDatabase.SaveAssets();
            OnTrinketCreated?.Invoke(workingTrinket);
            Close();
        }
    }
}
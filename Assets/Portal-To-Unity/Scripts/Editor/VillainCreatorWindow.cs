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
    public class VillainCreatorWindow : EditorWindow
    {
        public static Action<Villain> OnVillainCreated;

        private Villain workingVillain = null;
        private string presetVillainName = Enum.GetNames(typeof(VillainID)).Where(x => Enum.Parse<VillainID>(x) != 0).FirstOrDefault();

        public static void ShowWindow(string newName)
        {
            VillainCreatorWindow window = GetWindow<VillainCreatorWindow>("Create Villain");
            window.workingVillain = CreateInstance<Villain>();
            window.workingVillain.Name = newName;
            window.workingVillain.VillainID = 1;
            window.Show();
        }

        private void OnGUI()
        {
            if (workingVillain == null)
            {
                workingVillain = CreateInstance<Villain>();
                workingVillain.VillainID = 1;
            }

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            GUILayout.Label("New Villain", boldStyle);
            workingVillain.name = EditorGUILayout.TextField("File Name", workingVillain.name);
            EditorGUILayout.Space();
            workingVillain.Name = EditorGUILayout.TextField("Name", workingVillain.Name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset Villain", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsVillainIDAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            string discoveredEnum = typeof(VillainID).Name;

            var findTarget = enumTypes.Where(x => x.Name == workingVillain.VillainIDEnumType).FirstOrDefault();
            if (workingVillain.VillainID != 0)
            {
                if (findTarget != null && findTarget != default)
                {
                    if (Enum.GetValues(findTarget).Cast<int>().Contains(workingVillain.VillainID))
                    {
                        discoveredEnum = findTarget.Name;
                        presetVillainName = Enum.GetName(findTarget, (int)workingVillain.VillainID);
                    }
                    else
                    {
                        foreach (Type type in enumTypes)
                        {
                            if (type.Name == workingVillain.VillainIDEnumType) continue;

                            if (Enum.GetValues(type).Cast<int>().Contains(workingVillain.VillainID))
                            {
                                discoveredEnum = type.Name;
                                presetVillainName = Enum.GetName(type, (int)workingVillain.VillainID);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == workingVillain.VillainIDEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingVillain.VillainID))
                        {
                            workingVillain.VillainIDEnumType = type.Name;
                            presetVillainName = Enum.GetName(type, (int)workingVillain.VillainID);
                        }
                    }
                }
            }

            if (GUILayout.Button(presetVillainName, EditorStyles.popup))
            {
                VillainPicker picker = CreateInstance<VillainPicker>();
                picker.onSelection = (x, y) =>
                {
                    workingVillain.VillainID = (byte)x;
                    workingVillain.VillainIDEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingVillain.VillainID = (byte)Mathf.Clamp(EditorGUILayout.IntField("Villain ID", workingVillain.VillainID), 0, byte.MaxValue);
            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Villain")) return;

            workingVillain.VillainIDEnumType = discoveredEnum;

            if (workingVillain.VillainID == 0)
            {
                Debug.LogError("Cannot create Villain with ID 0. ID is reserved.");
                return;
            }

            string newFileName = workingVillain.name.Trim();

            if (newFileName == string.Empty)
                newFileName = "NewVillain";

            string path = $"Assets/Resources/Portal-To-Unity/Villains/{(int)workingVillain.VillainID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Villain with file name {newFileName} already exists. Please use a different name instead");
                return;
            }
            AssetDatabase.CreateAsset(workingVillain, path);
            AssetDatabase.SaveAssets();
            OnVillainCreated?.Invoke(workingVillain);
            Close();
        }
    }
}
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
    public class SkylanderCreatorWindow : EditorWindow
    {
        public static Action<Skylander> OnSkylanderCreated;

        private Skylander workingSkylander = null;
        private string presetToyCodeName = Enum.GetNames(typeof(ToyCode)).FirstOrDefault();

        public static void ShowWindow(string newName)
        {
            SkylanderCreatorWindow window = GetWindow<SkylanderCreatorWindow>("Create Skylander");
            window.workingSkylander = CreateInstance<Skylander>();
            window.workingSkylander.Name = newName;
            window.Show();
        }

        private void OnGUI()
        {
            if (workingSkylander == null)
                workingSkylander = CreateInstance<Skylander>();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            GUILayout.Label("New Skylander", boldStyle);
            workingSkylander.name = EditorGUILayout.TextField("File Name", workingSkylander.name);
            EditorGUILayout.Space();
            workingSkylander.Prefix = EditorGUILayout.TextField("Prefix", workingSkylander.Prefix);
            workingSkylander.Name = EditorGUILayout.TextField("Name", workingSkylander.Name);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset ToyCode", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsToyCodeAttribute>() != null);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            var findTarget = enumTypes.Where(x => x.Name == workingSkylander.ToyCodeEnumType).FirstOrDefault();
            if (findTarget != null && findTarget != default)
            {
                if (Enum.GetValues(findTarget).Cast<int>().Contains(workingSkylander.CharacterID))
                    presetToyCodeName = Enum.GetName(findTarget, (int)workingSkylander.CharacterID);
                else
                {
                    foreach (Type type in enumTypes)
                    {
                        if (type.Name == workingSkylander.ToyCodeEnumType) continue;

                        if (Enum.GetValues(type).Cast<int>().Contains(workingSkylander.CharacterID))
                            presetToyCodeName = Enum.GetName(type, (int)workingSkylander.CharacterID);
                    }
                }
            }
            else
            {
                foreach (Type type in enumTypes)
                {
                    if (type.Name == workingSkylander.ToyCodeEnumType) continue;

                    if (Enum.GetValues(type).Cast<int>().Contains(workingSkylander.CharacterID))
                    {
                        workingSkylander.ToyCodeEnumType = type.Name;
                        presetToyCodeName = Enum.GetName(type, (int)workingSkylander.CharacterID);
                    }
                }
            }

            if (GUILayout.Button(presetToyCodeName, EditorStyles.popup))
            {
                ToyCodePicker picker = CreateInstance<ToyCodePicker>();
                picker.onSelection = (x, y) =>
                { 
                    workingSkylander.CharacterID = (ushort)x;
                    workingSkylander.ToyCodeEnumType = y;
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 255, 360), picker);
            }

            EditorGUILayout.EndHorizontal();
            workingSkylander.CharacterID = (ushort)Mathf.Clamp(EditorGUILayout.IntField("Character ID", workingSkylander.CharacterID), 0, ushort.MaxValue);
            EditorGUILayout.Space();
            workingSkylander.Type = (SkyType)EditorGUILayout.EnumPopup("Type", workingSkylander.Type);
            workingSkylander.Element = (Element)EditorGUILayout.EnumPopup("Element", workingSkylander.Element);

            EditorGUILayout.Space();
            if (!GUILayout.Button("Create Skylander")) return;

            string newFileName = workingSkylander.name.Trim();

            if (newFileName == string.Empty)
                newFileName = "NewSkylander";

            string path = $"Assets/Resources/Portal-To-Unity/Figures/{(int)workingSkylander.CharacterID}_{newFileName}.asset";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(path))
            {
                Debug.LogError($"Skylander with file name {newFileName} already exists. Please use a different name instead.");
                return;
            }
            AssetDatabase.CreateAsset(workingSkylander, path);
            AssetDatabase.SaveAssets();
            OnSkylanderCreated?.Invoke(workingSkylander);
            Close();
        }
    }
}
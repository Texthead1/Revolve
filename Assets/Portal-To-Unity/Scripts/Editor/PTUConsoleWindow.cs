using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    public class PTUConsoleWindow : EditorWindow
    {
        [MenuItem("Portal-To-Unity/Console", false, 1)]
        public static void ShowWindow()
        {
            PTUConsoleWindow window = GetWindow<PTUConsoleWindow>("Console (Portal-To-Unity)");
            window.titleContent = new GUIContent("Console (Portal-To-Unity)", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/d_placeholder"));
        }
    }
}
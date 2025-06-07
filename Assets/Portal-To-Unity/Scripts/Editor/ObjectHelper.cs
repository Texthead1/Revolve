using System.IO;
using UnityEditor;
using UnityEngine;

namespace PortalToUnity
{
    internal class ObjectHelper
    {
        [MenuItem("Portal-To-Unity/Tools/Reserialize Data Structures", false, 15)]
        private static void ReserializeObjects()
        {
            string[] assetPaths = Directory.GetFiles("Assets/Resources/Portal-To-Unity/", "*.asset", SearchOption.AllDirectories);

            foreach (string path in assetPaths)
            {
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (obj != null)
                    AssetDatabase.ForceReserializeAssets(new string[] { path });
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
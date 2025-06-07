using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class DecoIDPicker : PickerWindow
    {
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<PickerEntryInfo> newItems = new List<PickerEntryInfo>();

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsDecoIDAttribute>() != null && t.Name != typeof(DecoID).Name);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            List<SearchTreeEntry> list = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("Select Deco"), 0) };
            var decos = Enum.GetValues(typeof(DecoID)).Cast<int>().ToList();
            var names = Enum.GetNames(typeof(DecoID));

            for (int i = 0; i < names.Length; i++)
                newItems.Add(new PickerEntryInfo(names[i], (ushort)decos[i], typeof(DecoID).Name));
            
            list.Add(new SearchTreeGroupEntry(new GUIContent(typeof(DecoID).Name), 1));
            Populate(newItems, ref list);
            newItems.Clear();

            foreach (Type type in enumTypes)
            {
                var discoveredIDs = Enum.GetValues(type).Cast<int>().ToList();
                var discoveredNames = Enum.GetNames(type);

                for (int i = 0; i < discoveredNames.Length; i++)
                    newItems.Add(new PickerEntryInfo(discoveredNames[i], (ushort)discoveredIDs[i], type.Name));

                list.Add(new SearchTreeGroupEntry(new GUIContent(type.Name), 1));
                Populate(newItems, ref list);
                newItems.Clear();
            }
            return list;
        }

        private void Populate(List<PickerEntryInfo> additions, ref List<SearchTreeEntry> list)
        {
            foreach (PickerEntryInfo item in additions)
            {
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(item.Name));
                entry.level = 2;
                entry.userData = item;
                list.Add(entry);
            }
        }

        public override bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            onSelection?.Invoke(((PickerEntryInfo)searchTreeEntry.userData).Value, ((PickerEntryInfo)searchTreeEntry.userData).UnderlyingEnumName);
            return true;
        }
    }
}
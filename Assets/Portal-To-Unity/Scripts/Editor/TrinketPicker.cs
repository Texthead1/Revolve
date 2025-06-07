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
    public class TrinketPicker : PickerWindow
    {
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<PickerEntryInfo> newItems = new List<PickerEntryInfo>();

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsTrinketIDAttribute>() != null && t.Name != typeof(TrinketID).Name);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            List<SearchTreeEntry> list = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("Select Trinket"), 0) };
            var trinkets = Enum.GetValues(typeof(TrinketID)).Cast<int>().ToList();
            var names = Enum.GetNames(typeof(TrinketID));

            for (int i = 0; i < names.Length; i++)
            {
                if (trinkets[i] == 0) continue;
                newItems.Add(new PickerEntryInfo(names[i], (ushort)trinkets[i], typeof(TrinketID).Name));
            }

            Populate(newItems, ref list, 0);
            newItems.Clear();

            foreach (Type type in enumTypes)
            {
                var discoveredIDs = Enum.GetValues(type).Cast<int>().ToList();
                var discoveredNames = Enum.GetNames(type);

                for (int i = 0; i < discoveredNames.Length; i++)
                {
                    if (discoveredIDs[i] == 0) continue;
                    newItems.Add(new PickerEntryInfo(discoveredNames[i], (ushort)discoveredIDs[i], type.Name));
                }
                list.Add(new SearchTreeGroupEntry(new GUIContent(type.Name), 1));
                Populate(newItems, ref list, 1);
                newItems.Clear();
            }
            return list;
        }

        private void Populate(List<PickerEntryInfo> additions, ref List<SearchTreeEntry> list, int levelModifier)
        {
            string[] groupNames = new string[] { "Trap Team" };

            List<PickerEntryInfo> entriesFiltered = new List<PickerEntryInfo>(additions);
            IEnumerable<PickerEntryInfo>[] entryGroups = new IEnumerable<PickerEntryInfo>[groupNames.Length];

            entryGroups[0] = entriesFiltered.Where(x => x.Value.RangeInclusive(0, 33)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[0].Contains(x));

            for (int i = 0; i < entryGroups.Length; i++)
            {
                if (entryGroups[i] == null || entryGroups[i].Count() == 0) continue;

                list.Add(new SearchTreeGroupEntry(new GUIContent(groupNames[i]), 1 + levelModifier));

                foreach (PickerEntryInfo item in entryGroups[i])
                {
                    SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(item.Name));
                    entry.level = 2 + levelModifier;
                    entry.userData = item;
                    list.Add(entry);
                }
            }

            if (entriesFiltered.Count != 0)
                list.Add(new SearchTreeGroupEntry(new GUIContent("Other"), 1 + levelModifier));

            for (int i = 0; i < entriesFiltered.Count; i++)
            {
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entriesFiltered[i].Name))
                {
                    level = 2 + levelModifier,
                    userData = entriesFiltered[i]
                };
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
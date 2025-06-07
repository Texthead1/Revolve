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
    public class ToyCodePicker : PickerWindow
    {
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<PickerEntryInfo> newItems = new List<PickerEntryInfo>();

            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly loadedAssembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, "Assembly-CSharp.dll"));

            var enumTypes = loadedAssembly.GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<MarkAsToyCodeAttribute>() != null && t.Name != typeof(ToyCode).Name);
            enumTypes = enumTypes.OrderBy(t => t.Namespace == PORTAL_TO_UNITY_TARGET_NAMESPACE ? 0 : 1).ToList();

            List<SearchTreeEntry> list = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("Select Character ID"), 0) };
            var toyCodes = Enum.GetValues(typeof(ToyCode)).Cast<int>().ToList();
            var names = Enum.GetNames(typeof(ToyCode));

            for (int i = 0; i < names.Length; i++)
                newItems.Add(new PickerEntryInfo(names[i], (ushort)toyCodes[i], typeof(ToyCode).Name));
            
            Populate(newItems, ref list, 0);
            newItems.Clear();

            foreach (Type type in enumTypes)
            {
                var discoveredToyCodes = Enum.GetValues(type).Cast<int>().ToList();
                var discoveredNames = Enum.GetNames(type);

                for (int i = 0; i < discoveredNames.Length; i++)
                    newItems.Add(new PickerEntryInfo(discoveredNames[i], (ushort)discoveredToyCodes[i], type.Name));

                list.Add(new SearchTreeGroupEntry(new GUIContent(type.Name), 1));
                Populate(newItems, ref list, 1);
                newItems.Clear();
            }
            return list;
        }

        private void Populate(List<PickerEntryInfo> additions, ref List<SearchTreeEntry> list, int levelModifier)
        {
            string[] groupNames = new string[]
            {
                "Spyro's Adventure",
                "Giants",
                "SWAP Force",
                "Trap Team",
                "SuperChargers",
                "Imaginators",
                "Toys For Bob Items",
                "Vicarious Visions Items",
                "Minis/Sidekicks"
            };

            List<PickerEntryInfo> entriesFiltered = new List<PickerEntryInfo>(additions);
            IEnumerable<PickerEntryInfo>[] entryGroups = new IEnumerable<PickerEntryInfo>[groupNames.Length];

            entryGroups[0] = entriesFiltered.Where(x => ToyCodeExtensions.IsSSA(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[0].Contains(x));

            entryGroups[1] = entriesFiltered.Where(x => ToyCodeExtensions.IsGiants(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[1].Contains(x));

            entryGroups[2] = entriesFiltered.Where(x => ToyCodeExtensions.IsSwapForce(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[2].Contains(x));

            entryGroups[3] = entriesFiltered.Where(x => ToyCodeExtensions.IsTrapTeam(x.Value) || ToyCodeExtensions.IsTrapTeam_Debug(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[3].Contains(x));

            entryGroups[4] = entriesFiltered.Where(x => ToyCodeExtensions.IsSuperChargers(x.Value) || ToyCodeExtensions.IsTemplateVehicle(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[4].Contains(x));

            entryGroups[5] = entriesFiltered.Where(x => ToyCodeExtensions.IsImaginators_Exclusive(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[5].Contains(x));

            entryGroups[6] = entriesFiltered.Where(x => ToyCodeExtensions.IsTFB_Item(x.Value) || ToyCodeExtensions.IsTFB_Expansion(x.Value) || ToyCodeExtensions.IsCrystal(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[6].Contains(x));

            entryGroups[7] = entriesFiltered.Where(x => ToyCodeExtensions.IsVV_Item(x.Value) || ToyCodeExtensions.IsVV_Expansion(x.Value) || ToyCodeExtensions.IsRacingPack(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[7].Contains(x));

            entryGroups[8] = entriesFiltered.Where(x => ToyCodeExtensions.IsMini(x.Value)).ToList();
            entriesFiltered.RemoveAll(x => entryGroups[8].Contains(x));

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
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PortalToUnity
{
    public abstract class PickerWindow : ScriptableObject, ISearchWindowProvider
    {
        public Action<int, string> onSelection;

        internal struct PickerEntryInfo
        {
            public string Name;
            public int Value;
            public string UnderlyingEnumName;

            public PickerEntryInfo(string name, int value, string enumName)
            {
                Name = name;
                Value = value;
                UnderlyingEnumName = enumName;
            }
        }

        public abstract List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context);
        public abstract bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context);
    }
}
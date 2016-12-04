//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class UTCompatibleTypesResult
    {
        private readonly string[] typeNames;
        private readonly GUIContent[] nicifiedTypeNames;

        public UTCompatibleTypesResult(IEnumerable<Type> types)
        {
            var sortedNames = new SortedDictionary<string, Type>();
            var duplicateKeys = new HashSet<Type>();

            foreach (var type in types)
            {
                var nicifiedTypeName = ObjectNames.NicifyVariableName(type.Name);
                if (!sortedNames.ContainsKey(nicifiedTypeName))
                {
                    sortedNames.Add(nicifiedTypeName, type);
                }
                else
                {
                    duplicateKeys.Add(type);
                    duplicateKeys.Add(sortedNames[nicifiedTypeName]);
                }
            }

            foreach (var type in duplicateKeys)
            {
                // remove any existing key with that name that might still exist.
                var nicifiedTypeName = ObjectNames.NicifyVariableName(type.Name);
                sortedNames.Remove(nicifiedTypeName);
                    
                // for these we need to add the namespace to avoid duplicates
                var namespacedName = nicifiedTypeName + (string.IsNullOrEmpty(type.Namespace) ? " (<global>)" : " (" + type.Namespace + ")");
                if (sortedNames.ContainsKey(namespacedName))
                {
                    throw new ArgumentException("It seems like you have two classes with the same name and namespace in your project. Please correct this.");
                }
                sortedNames.Add(namespacedName , type);
            }


            typeNames = new string[sortedNames.Count];
            nicifiedTypeNames = new GUIContent[sortedNames.Count];

            var i = 0;
            foreach (var key in sortedNames.Keys)
            {
                nicifiedTypeNames[i] = new GUIContent(key);
                typeNames[i] = sortedNames[key].FullName;
                i++;
            }
        }

        public string[] TypeNames
        {
            get
            {
                return typeNames;
            }
        }

        public GUIContent[] NicifiedTypeNames
        {
            get
            {
                return nicifiedTypeNames;
            }
        }
    }
}

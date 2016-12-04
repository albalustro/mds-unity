//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using UnityEditor;

    [UTPropertyRenderer(typeof(UTTypeInfo), typeof(UTType))]
    public class UTTypePropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper fieldWrapper)
        {
            Type baseType = fieldWrapper.InspectorHint.baseType;

            var compatibleTypes = UTComponentScanner.FindCompatibleTypes(baseType);

            var val = (UTTypeInfo)fieldWrapper.Value;
            int currentIndex = -1;
            if (val != null)
            {
                currentIndex = Array.IndexOf(compatibleTypes.TypeNames, val.TypeName);
            }

            int newIndex = -1;
            if (fieldWrapper.Label != null)
            {
                newIndex = EditorGUILayout.Popup(fieldWrapper.Label, currentIndex, compatibleTypes.NicifiedTypeNames);
            }
            else
            {
                newIndex = EditorGUILayout.Popup(currentIndex, compatibleTypes.NicifiedTypeNames);
            }

            if (currentIndex != newIndex)
            {
                if (newIndex == -1)
                {
                    fieldWrapper.Value = null;
                }
                else
                {
                    fieldWrapper.Value = new UTTypeInfo(compatibleTypes.TypeNames[newIndex]);
                }
            }
        }
    }
}

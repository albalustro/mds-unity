//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTPropertyRenderer(typeof(UTVector3), typeof(Vector3))]
    public class UTVector3PropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper fieldWrapper)
        {
            var hint = fieldWrapper.InspectorHint;
            EditorGUILayout.BeginHorizontal();
            if (fieldWrapper.Label != null)
            {
                EditorGUILayout.PrefixLabel(fieldWrapper.Label);
            }
            else
            {
                EditorGUILayout.PrefixLabel(" ");
            }

            Vector3 val = (Vector3)fieldWrapper.Value;
            GUILayout.Label(hint.GetCaptionAtIndex(0, "X"));
            val.x = EditorGUILayout.FloatField(val.x);
            GUILayout.Label(hint.GetCaptionAtIndex(1, "Y"));
            val.y = EditorGUILayout.FloatField(val.y);
            GUILayout.Label(hint.GetCaptionAtIndex(2, "Z"));
            val.z = EditorGUILayout.FloatField(val.z);
            fieldWrapper.Value = val;
            EditorGUILayout.EndHorizontal();
        }
    }
}

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

    [UTPropertyRenderer(typeof(UTQuaternion), typeof(Quaternion))]
    public class UTQuaternionPropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper fieldWrapper)
        {
            EditorGUILayout.BeginHorizontal();
            if (fieldWrapper.Label != null)
            {
                EditorGUILayout.PrefixLabel(fieldWrapper.Label);
            }
            else
            {
                EditorGUILayout.PrefixLabel(" ");
            }

            Quaternion val = (Quaternion)fieldWrapper.Value;
            GUILayout.Label("X");
            val.x = EditorGUILayout.FloatField(val.x);
            GUILayout.Label("Y");
            val.y = EditorGUILayout.FloatField(val.y);
            GUILayout.Label("Z");
            val.z = EditorGUILayout.FloatField(val.z);
            GUILayout.Label("W");
            val.w = EditorGUILayout.FloatField(val.w);
            fieldWrapper.Value = val;
            EditorGUILayout.EndHorizontal();
        }
    }
}

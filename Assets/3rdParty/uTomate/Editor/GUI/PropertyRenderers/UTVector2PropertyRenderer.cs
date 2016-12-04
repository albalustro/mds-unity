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

    [UTPropertyRenderer(typeof(UTVector2), typeof(Vector2))]
    public class UTVector2PropertyRenderer : UTIPropertyRenderer
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

            Vector2 val = (Vector2)fieldWrapper.Value;
            GUILayout.Label(hint.GetCaptionAtIndex(0, "X"));
            val.x = EditorGUILayout.FloatField(val.x);
            GUILayout.Label(hint.GetCaptionAtIndex(1, "Y"));
            val.y = EditorGUILayout.FloatField(val.y);
            fieldWrapper.Value = val;
            EditorGUILayout.EndHorizontal();
        }
    }
}

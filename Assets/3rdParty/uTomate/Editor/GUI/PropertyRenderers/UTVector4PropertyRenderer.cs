//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{


using System;
using UnityEditor;
using UnityEngine;
    using API;

[UTPropertyRenderer(typeof(UTVector4), typeof(Vector4))]
public class UTVector4PropertyRenderer : UTIPropertyRenderer
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

        Vector4 val = (Vector4)fieldWrapper.Value;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(hint.GetCaptionAtIndex(0, "X"));
        val.x = EditorGUILayout.FloatField(val.x);
        GUILayout.Label(hint.GetCaptionAtIndex(1, "Y"));
        val.y = EditorGUILayout.FloatField(val.y);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(hint.GetCaptionAtIndex(2, "Z"));
        val.z = EditorGUILayout.FloatField(val.z);
        GUILayout.Label(hint.GetCaptionAtIndex(3, "W"));
        val.w = EditorGUILayout.FloatField(val.w);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        fieldWrapper.Value = val;
    }
}
}

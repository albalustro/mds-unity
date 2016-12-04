//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR: 5.3

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTPropertyRenderer(typeof(UTFileType), typeof(UTSetPlayerSettingsWindowsStoreAction.FileType))]
    public class UTFileTypePropertyRenderer : UTIPropertyRenderer
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

            var val = (UTSetPlayerSettingsWindowsStoreAction.FileType)fieldWrapper.Value;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Content Type:");
            val.contentType = EditorGUILayout.TextField(val.contentType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("File Type:      "); // TODO: properly layout these labels
            val.fileType = EditorGUILayout.TextField(val.fileType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            fieldWrapper.Value = val;
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif

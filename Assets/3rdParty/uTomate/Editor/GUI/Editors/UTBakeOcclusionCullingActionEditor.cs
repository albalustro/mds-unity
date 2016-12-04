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

    [CustomEditor(typeof(UTBakeOcclusionCullingAction))]
    public class UTBakeOcclusionCullingActionEditor : UTInspectorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load defaults"))
            {
                LoadDefaults();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void LoadDefaults()
        {
            var action = (UTBakeOcclusionCullingAction)target;

            Undo.RecordObject(action, "Load default occlusion culling settings");
            UTBakeOcclusionCullingAction.LoadDefaults(action);
            EditorUtility.SetDirty(action);
        }
    }
}

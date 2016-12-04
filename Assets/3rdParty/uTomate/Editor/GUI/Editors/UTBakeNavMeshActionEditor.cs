//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System.Reflection;
    using API;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UTBakeNavMeshAction))]
    public class UTBakeNavMeshActionEditor : UTInspectorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("For convenience, you can load the currently set editor settings into this action, using the button below.", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load settings"))
            {
                LoadSettings();
            }
            EditorGUILayout.EndHorizontal();
        }

        public override UTVisibilityDecision IsVisible(FieldInfo fieldInfo)
        {
            var self = (UTBakeNavMeshAction) target;
            if (fieldInfo.Name == "voxelSize")
            {
                return self.manualVoxelSize.HasValueOrExpression(true) ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }
            return base.IsVisible(fieldInfo);
        }

        private void LoadSettings()
        {
            var action = (UTBakeNavMeshAction)target;

            Undo.RecordObject(action, "Load default nav mesh settings");
            UTBakeNavMeshAction.LoadFromSettings(action);
            GUIUtility.keyboardControl = 0; // unfocus anything
            EditorUtility.SetDirty(action);
        }
    }
}

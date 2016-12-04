//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using UnityEditor;
    using UnityEngine;

    public class UTShowMigrationInfoDialog : EditorWindow
    {
        private bool hideNextTime;
        private static readonly GUIStyle DialogStyle;
        private const string DontShowAgainSettingName = "AncientLightStudios.uTomate.hideMigrationInfoDialog";

        static UTShowMigrationInfoDialog()
        {
            DialogStyle = new GUIStyle {padding = new RectOffset(10, 10, 10, 10)};
        }

        [InitializeOnLoadMethod]
        private static void AutoShowDialog()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorPrefs.GetBool(DontShowAgainSettingName, false))
            {
                EditorApplication.update += ShowDialogWhenReady;
            }
        }

        private static void ShowDialogWhenReady()
        {
            if (!EditorApplication.isCompiling)
            {
                // ReSharper disable once DelegateSubtraction
                EditorApplication.update -= ShowDialogWhenReady;
                var window = GetWindow<UTShowMigrationInfoDialog>(true, "uTomate Migration Information", true);
                window.minSize = new Vector2(400, 200);
                window.maxSize = new Vector2(400, 200);
            
            }
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(DialogStyle);
            GUILayout.Label("Thank you for upgrading uTomate to the latest version. " + 
                                    "With the release of Unity 5 some changes were introduced into uTomate that you should be aware of. " +
                                    "We strongly suggest that you go through our " +
                                    "Unity 5 migration tutorial before continuing to work with uTomate. Thank you very much. \n\n Happy automating!", 
                                    UTEditorResources.WrapLabelStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();

            hideNextTime = GUILayout.Toggle(hideNextTime, "Don't show again.");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Show migration tutorial"))
            {
                OpenMigrationTutorial();
                DoClose();
            }
            if (GUILayout.Button("Close"))
            {
                DoClose();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        [MenuItem("Window/uTomate/Help/Unity 5 Migration Tutorial", false, 1000)]
        private static void OpenMigrationTutorial()
        {
            UTils.OpenHelpFile("uTomate-Unity5-Migration-Tutorial", "http://www.ancientlightstudios.com/utomate/tutorials/switching_to_unity_5.html");
        }

        private void DoClose()
        {
            EditorPrefs.SetBool(DontShowAgainSettingName, hideNextTime);
            Close();
        }
    }
}

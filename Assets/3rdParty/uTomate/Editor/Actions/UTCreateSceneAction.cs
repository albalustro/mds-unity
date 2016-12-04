//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{ 
    using API;
    using System.Collections;
    using System.IO;
    using UnityEditor;
#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR 5.3
    using UnityEditor.SceneManagement;
#endif

    [UTActionInfo(actionCategory = "Scene Manipulation", sinceUTomateVersion = "1.3.0")]
    [UTDoc(title = "New Scene", description = "Creates a new scene and saves it to the project. All changes in the currently open scene will be discarded.")]
    [UTDefaultAction]
    public class UTCreateSceneAction : UTAction
    {
        [UTInspectorHint(required = true, displayAs = UTInspectorHint.DisplayAs.SaveFileSelect, caption = "Save scene as.", extension = "unity")]
        [UTDoc(description = "The path where the scene should be saved to. Path must be inside of $project:root.")]
        public UTString scene;

        public override IEnumerator Execute(UTContext context)
        {
            var theScene = scene.EvaluateIn(context);
            if (string.IsNullOrEmpty(theScene))
            {
                throw new UTFailBuildException("You need to specify a path where the scene should be saved to.", this);
            }
            theScene = UTFileUtils.FullPathToProjectPath(theScene);

            var theFullPath = UTFileUtils.CombineToPath(UTFileUtils.ProjectRoot, theScene);
            if (File.Exists(theFullPath))
            {
                throw new UTFailBuildException("There is already a file at '" + theScene + "'.", this);
            }

            UTFileUtils.EnsureParentFolderExists(theFullPath);
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR [5.0, 5.2]
            EditorApplication.NewScene();
#else
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR [5.0, 5.2]
            EditorApplication.SaveScene(theScene, false);
#else
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), theScene, false);
#endif
            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Scene Manipulation/New Scene", false, 270)]
        public static void AddAction()
        {
            Create<UTCreateSceneAction>();
        }
    }
}

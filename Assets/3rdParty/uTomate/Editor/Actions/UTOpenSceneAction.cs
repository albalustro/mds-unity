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
    using UnityEditor;
    using UnityEngine;

 #if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR 5.3
    using UnityEditor.SceneManagement;
#endif

    [UTActionInfo(actionCategory = "Scene Manipulation")]
    [UTDoc(title = "Open Scene", description = "Opens the given scene. Any changes in the currently open scene will be discarded.")]
    [UTDefaultAction]
    public class UTOpenSceneAction : UTAction
    {
        [UTInspectorHint(required = true, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, caption = "Select scene to open.", extension = "unity")]
        [UTDoc(description = "The scene to be opened. Can contain wildcards but must yield a unique scene name. Path must be inside of $project.root.")]
        public UTString scene;

        public override IEnumerator Execute(UTContext context)
        {
            var theScene = scene.EvaluateIn(context);

            if (theScene.Contains("*"))
            {
                var finalList = UTFileUtils.CalculateFileset(new string[] { theScene }, new string[0]);
                if (finalList.Length != 1)
                {
                    throw new UTFailBuildException("Scene wildcard " + theScene + " yielded " +
                        finalList.Length + " results but should yield exactly one scene.", this);
                }
                UTFileUtils.FullPathToProjectPath(finalList);
                theScene = finalList[0];
            }

            theScene = UTFileUtils.FullPathToProjectPath(theScene);

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Opening scene: " + theScene, this);
            }
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR [5.0, 5.2]
            var result = EditorApplication.OpenScene(theScene);
#else
            var result = EditorSceneManager.OpenScene(theScene);
#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR [5.0, 5.2]
            if (!result)
#else
            if (!result.IsValid())
#endif
            {
                throw new UTFailBuildException("Scene " + theScene + " could not be opened.", this);
            }
            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Scene Manipulation/Open Scene", false, 280)]
        public static void AddAction()
        {
            Create<UTOpenSceneAction>();
        }
    }
}

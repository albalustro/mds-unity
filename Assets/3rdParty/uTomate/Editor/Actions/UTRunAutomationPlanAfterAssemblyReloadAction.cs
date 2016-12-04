//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections;
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Run", sinceUTomateVersion = "1.6.0")]
	[UTDoc(title = "Run Plan After Assembly Reload", 
	       description = "Runs the given automation plan after an assembly reload. This is useful if you automate things that " +
	       	"require an assembly reload and allows to continue after the reload is finished. Note that this action will wait for " +
	       	"an assembly reload, so make sure you perform an action that actually triggers an assembly reload right before " +
	       	"this action. This action should always be the LAST action in your automation plan.")]
    [UTInspectorGroups(groups = new []{"General"})]
    public class UTRunAutomationPlanAfterAssemblyReloadAction : UTAction
    {
        [UTDoc(description = "The plan that should be run.")]
        [UTInspectorHint(group = "General", order = 1, required = true)]
        public UTAutomationPlan plan;

        public override IEnumerator Execute(UTContext context)
        {
            if (plan == null)
            {
                throw new UTFailBuildException("You have not specified a plan that should run after assembly reload. Please specify one.", this);
            }

            var path = AssetDatabase.GenerateUniqueAssetPath(UTFileUtils.FullPathToProjectPath(UTFileUtils.CombineToPath(UTFileUtils.ProjectAssets, "uTomate_Temporary_" + Guid.NewGuid().ToString() + ".asset")));
            var item = ScriptableObject.CreateInstance<UTDelayedExecution>();
            AssetDatabase.CreateAsset(item, path);
            item.RunAfterDeserialization = plan;
            EditorUtility.SetDirty(item);
			if (UTPreferences.DebugMode) {
				Debug.Log ("Wrote: " + item, item);
			}
			yield break;
        }


        [MenuItem("Assets/Create/uTomate/Run/Run Plan After Assembly Reload", false, 270)]
        public static void AddAction()
        {
            Create<UTRunAutomationPlanAfterAssemblyReloadAction>();
        }
    }
}

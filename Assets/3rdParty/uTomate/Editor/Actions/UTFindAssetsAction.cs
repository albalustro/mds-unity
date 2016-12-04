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
    using UObject = UnityEngine.Object;

    [UTActionInfo(actionCategory = "General", sinceUTomateVersion = "1.5.0")]
    [UTDoc(title = "Find Assets", description = "Finds assets in the current project and puts them into a property.")]
    [UTDefaultAction]
    public class UTFindAssetsAction : UTAction
    {
        [UTDoc(title = "Name", description = "The name of the property to be set.")]
        [UTInspectorHint(required = true, order = 0)]
        public UTString propertyName;

        [UTDoc(description = "The assets to include.")]
        [UTInspectorHint(order = 2)]
        public UTString[] includes;

        [UTDoc(description = "The assets to exclude.")]
        [UTInspectorHint(order = 3)]
        public UTString[] excludes;

        [UTDoc(description = "If this is ticked, only the first asset will be put into the property. Otherwise an array with all found assets will be put into the property.")]
        [UTInspectorHint(order = 4)]
        public UTBool onlyFirstAsset;

        public override IEnumerator Execute(UTContext context)
        {
            var theName = propertyName.EvaluateIn(context);
            if (string.IsNullOrEmpty(theName))
            {
                throw new UTFailBuildException("The name of the property must not be empty.", this);
            }

            var theRealIncludes = EvaluateAll(includes, context);
            var theRealExcludes = EvaluateAll(excludes, context);

            var fileSet = UTFileUtils.CalculateFileset(theRealIncludes, theRealExcludes);
            UTFileUtils.FullPathToProjectPath(fileSet);

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Found " + fileSet.Length + " matching asset(s).", this);
            }

            var findOnlyFirstAsset = onlyFirstAsset.EvaluateIn(context);
            if (findOnlyFirstAsset)
            {
                if (fileSet.Length == 0)
                {
                    context[theName] = null;
                }
                else
                {
                    context[theName] = AssetDatabase.LoadMainAssetAtPath(fileSet[0]);
                }
            }
            else
            {
                var result = new UObject[fileSet.Length];
                var idx = 0;
                foreach (var file in fileSet)
                {
                    result[idx] = AssetDatabase.LoadMainAssetAtPath(file);
                    yield return null;
                }
                context[theName] = result;
            }
        }

        [MenuItem("Assets/Create/uTomate/General/Find Assets", false, 375)]
        public static void AddAction()
        {
            Create<UTFindAssetsAction>();
        }
    }
}

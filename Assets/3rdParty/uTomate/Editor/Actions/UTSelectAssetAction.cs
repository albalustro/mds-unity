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

    [UTActionInfo(actionCategory = "General", sinceUTomateVersion = "1.3.0")]
    [UTDoc(title = "Select Asset", description = "Selects the asset at the given path.")]
    [UTDefaultAction]
    public class UTSelectAssetAction : UTAction
    {
        [UTDoc(description = "The path to the asset that should be selected.")]
        [UTInspectorHint(required = true)]
        public UTString assetPath;

        public override IEnumerator Execute(UTContext context)
        {
            var pathToInstance = assetPath.EvaluateIn(context);
            if (pathToInstance.Contains("*"))
            {
                var fileSet = UTFileUtils.CalculateFileset(new string[] { pathToInstance }, new string[0]);
                if (fileSet.Length != 1)
                {
                    throw new UTFailBuildException("Selector must yield exactly one result file, but yields " + fileSet.Length, this);
                }
                pathToInstance = fileSet[0];
            }
            pathToInstance = UTFileUtils.FullPathToProjectPath(pathToInstance);

            var theAsset = AssetDatabase.LoadMainAssetAtPath(pathToInstance);
            Selection.activeObject = theAsset;

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/General/Select Asset", false, 380)]
        public static void AddAction()
        {
            Create<UTSelectAssetAction>();
        }
    }
}

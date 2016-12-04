//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using AncientLightStudios.uTomate.API;
    using System;
    using System.Collections;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UObject = UnityEngine.Object;

    [UTActionInfo(actionCategory = "Build", sinceUTomateVersion="1.5.0", minUnityVersion="5.0")]
    [UTDoc(title = "Build Asset Bundles From Asset Database", description = "Builds all asset bundles currently defined in the asset database.")]
    [UTInspectorGroups(groups = new string[] { "Bundle" })]
    [UTDefaultAction]
    public class UTBuildAssetBundlesFromAssetDatabaseAction : UTAction
    {

		[UTDoc(description = "The target platform for which the asset bundle should be built.")]
		[UTInspectorHint(group = "Bundle", order = 0, required = true)]
		public UTBuildTarget targetPlatform;

        [UTDoc(description = "Where should the built asset bundles be put?")]
        [UTInspectorHint(group = "Bundle", order = 1, required = true)]
        public UTString outputPath;

        [UTDoc(title = "No Type Information", description = "Do not include type information within the AssetBundle.")]
        [UTInspectorHint(group = "Bundle", order = 2)]
        public UTBool disableWriteTypeTree;

        [UTDoc(title = "Ignore Type Tree Changes", description = "Do not rebuild the asset bundle if there were type tree changes, only.")]
        [UTInspectorHint(group = "Bundle", order = 3)]
        public UTBool ignoreTypeTreeChanges;

        [UTDoc(title = "Uncompressed", description = "Builds an uncompressed asset bundle.")]
        [UTInspectorHint(group = "Bundle", order = 4)]
        public UTBool uncompressedAssetBundle;

        [UTDoc(title = "Append Hash", description = "Append hash to asset bundle name?")]
        [UTInspectorHint(group = "Bundle", order = 5)]
        public UTBool appendHash;

        [UTDoc(title = "Force rebuild", description = "Should the bundles be rebuilt even when there is no change?")]
        [UTInspectorHint(group = "Bundle", order = 6)]
        public UTBool forceRebuild;

        public override IEnumerator Execute(UTContext context)
        {
            var theOutputPath = outputPath.EvaluateIn(context);
            if (string.IsNullOrEmpty(theOutputPath))
            {
                throw new UTFailBuildException("You must specify an output path.", this);
            }

			UTFileUtils.EnsureFolderExists(theOutputPath);

			if (UTFileUtils.IsFile(theOutputPath))
            {
                throw new UTFailBuildException("The specified output path is a file. It must be a folder.", this);
            }

            Debug.Log("Building asset bundles.");
            var realDisableWriteTypeTree = disableWriteTypeTree.EvaluateIn(context);
            var realIgnoreTypeTreeChanges = ignoreTypeTreeChanges.EvaluateIn(context);

            if (realDisableWriteTypeTree && realIgnoreTypeTreeChanges)
            {
                throw new UTFailBuildException("You have disabled the writing of type information but have enabled that type changes should be ignored." +
                                               "Please either enable type information or disable 'Ignore Type Tree Changes'.", this);
            }
            var realUncompressedAssetBundle = uncompressedAssetBundle.EvaluateIn(context);
            var realAppendHash = appendHash.EvaluateIn(context);
            var realForceRebuild = forceRebuild.EvaluateIn(context);

            var buildOpts = (BuildAssetBundleOptions)0;

            if (realDisableWriteTypeTree)
            {
                buildOpts |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }

            if (realIgnoreTypeTreeChanges)
            {
                buildOpts |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            }

            if (realUncompressedAssetBundle)
            {
                buildOpts |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            if (realAppendHash)
            {
                buildOpts |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }

            if (realForceRebuild)
            {
                buildOpts |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

			BuildTarget target = targetPlatform.EvaluateIn(context);
			BuildPipeline.BuildAssetBundles(theOutputPath, buildOpts, target);
            Debug.Log("Built asset bundles at " + theOutputPath);
            yield break;
        }

        [MenuItem("Assets/Create/uTomate/Build/Build Asset Bundles From Asset Database (Unity 5)", false, 371)]
        public static void AddAction()
        {
            Create<UTBuildAssetBundlesFromAssetDatabaseAction>();
        }
    }
}

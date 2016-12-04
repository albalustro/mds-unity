//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

using UnityEngine;
using System.Collections;

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;
    using UObject = UnityEngine.Object;

    [UTActionInfo(actionCategory = "General", sinceUTomateVersion = "1.7.0", minUnityVersion = "5.0")]
    [UTDoc(title = "Set Asset Bundle", description = "Sets the asset bundle for the given assets.")]
    [UTInspectorGroups(groups = new string[] { "Bundle", "Assets" })]
    [UTDefaultAction]
    public class UTSetAssetBundleAction : UTAction
    {
        [UTDoc(description = "The name of the asset bundle to be assigned. An empty name will unassign the asset bundle.")]
        [UTInspectorHint(group = "Bundle", order = 10)]
        public UTString bundleName;

        [UTDoc(description = "The name of the variant to be assigned. An empty name will unassign the asset bundle variant.")]
        [UTInspectorHint(group = "Bundle", order = 20)]
        public UTString variant;


        //
        [UTDoc(description = "The assets that should be included.")]
        [UTInspectorHint(group = "Assets", order = 10)]
        public UTString[] includes;

        [UTDoc(description = "The assets that should be excluded.")]
        [UTInspectorHint(group = "Assets", order = 20)]
        public UTString[] excludes;

        public override IEnumerator Execute(UTContext context)
        {
            var files = UTFileUtils.CalculateFileset(EvaluateAll(includes, context), EvaluateAll(excludes, context));
            UTFileUtils.FullPathToProjectPath(files); // repath them to be relative to project root


            var theName = bundleName.EvaluateIn(context);
            if (string.IsNullOrEmpty(theName))
            {
                theName = null;
            }

            var theVariant = variant.EvaluateIn(context);
            if (string.IsNullOrEmpty(theVariant))
            {
                theVariant = null;
            }

            if (!string.IsNullOrEmpty(theVariant) && string.IsNullOrEmpty(theName))
            {
                throw new UTFailBuildException("You must set an asset bundle name if you want to set the asset bundle variant.", this);
            }

            Debug.Log("Assigning " + files.Length + " file(s) to asset bundle " + theName + " with variant " + theVariant);

            foreach (var file in files)
            {
                var asset = AssetDatabase.LoadAssetAtPath(file, typeof(UObject));
                if (asset is MonoScript)
                {
                    Debug.LogWarning("Asset " + file + " is a mono script. It cannot be included into an asset bundle. Ignoring it.", this);
                    continue;
                }

                var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
                if (importer == null)
                {
                    Debug.LogWarning("Unable to get importer for asset " + file + ". Please report this. Ignoring this file for now.", this);
                    continue;
                }

                if (UTPreferences.DebugMode)
                {

                    Debug.Log("Assigning " + file + " to asset bundle " + theName + " with variant " + theVariant);
                }

                importer.assetBundleName = theName;

                // we cannot set the variant if we set the name to null
                if (!string.IsNullOrEmpty(importer.assetBundleName))
                {
                    importer.assetBundleVariant = theVariant;
                }


                yield return "";
            }

            // make sure the editor gets what it needs.
            UTInternalCall.InvokeStatic("UnityEditor.EditorApplication", "Internal_CallAssetBundleNameChanged");
        }

        [MenuItem("Assets/Create/uTomate/General/Set Asset Bundle", false, 380)]
        public static void AddAction()
        {
            var action = Create<UTSetAssetBundleAction>();
            action.excludes = new UTString[3];
            // exclude meta files by default
            action.excludes[0] = new UTString
            {
                StaticValue = "**/*.meta"
            };
            // exclude C# scripts by default
            action.excludes[1] = new UTString()
            {
                StaticValue = "**/*.cs"
            };

            // exclude JS scripts by default
            action.excludes[2] = new UTString()
            {
                StaticValue = "**/*.js"
            };

        }
    }

}


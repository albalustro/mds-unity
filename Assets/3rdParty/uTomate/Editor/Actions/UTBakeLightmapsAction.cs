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

    [UTActionInfo(actionCategory = "Bake")]
    [UTDoc(title = "Bake Lightmaps", description = "Bakes the lightmaps for the currently open scene using the given lightmap settings.")]
    [UTDefaultAction]
    public class UTBakeLightmapsAction : UTAction
    {
        [UTDoc(description = "What should be baked?")]
        [UTInspectorHint(required = true, order = 0)]
        public UTBakeType whatToBake;

        public override IEnumerator Execute(UTContext context)
        {
            var whatReallyToBake = whatToBake.EvaluateIn(context);
            switch (whatReallyToBake)
            {
                case UTTypeOfBake.Everything:
                    Debug.Log("Building lightmaps for current scene. This may take a while.", this);
                    if (!Lightmapping.BakeAsync())
                    {
                        throw new UTFailBuildException("Lightmapping was not finished successfully.", this);
                    }
                    break;

                case UTTypeOfBake.SelectionOnly:
                    Debug.Log("Building lightmaps for current selection. This may take a while.", this);
                    if (!Lightmapping.BakeAsync())
                    {
                        throw new UTFailBuildException("Lightmapping was not finished successfully.", this);
                    }
                    break;

                case UTTypeOfBake.LightProbesOnly:
                    Debug.Log("Building light probes for current scene. This may take a while.", this);
                    if (!Lightmapping.BakeAsync())
                    {
                        throw new UTFailBuildException("Lightmapping was not finished successfully.", this);
                    }
                    break;
            }
            do
            {
                yield return "";
                if (context.CancelRequested)
                {
                    Lightmapping.Cancel();
                }
            } while (Lightmapping.isRunning);
            Debug.Log("Lightmapping finished.", this);
        }

        [MenuItem("Assets/Create/uTomate/Bake/Bake Lightmaps", false, 210)]
        public static void AddAction()
        {
            Create<UTBakeLightmapsAction>();
        }

        public enum UTTypeOfBake
        {
            Everything,
            SelectionOnly,
            LightProbesOnly
        }

    }
}

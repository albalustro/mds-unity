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
    [UTDoc(title = "Bake Occlusion Culling", description = "Bakes the occlusion culling for the current scene.")]
    [UTRequiresLicense(UTLicense.UnityPro)]
    [UTDefaultAction]
    public class UTBakeOcclusionCullingAction : UTAction
    {
        // Unity 4.3+
        [UTDoc(description = "The size of the smallest object that will be used to hide other objects when doing occlusion culling.")]
        [UTInspectorHint(order = 1, required = true)]
        public UTFloat smallestOccluder;

        [UTDoc(description = "The smallest hole in the geometry through which the camera is supposed to see.")]
        [UTInspectorHint(order = 2, required = true)]
        public UTFloat smallestHole;

        [UTDoc(description = "The backface threshold is a size optimization that reduces unneccessary details by testing backfaces.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 5.0f, maxValue = 100f, order = 3, required = true)]
        public UTFloat backfaceThreshold;

        public override IEnumerator Execute(UTContext context)
        {
            var realSmallestOccluder = smallestOccluder.EvaluateIn(context);
            var realSmallestHole = smallestHole.EvaluateIn(context);
            var realBackfaceThreshold = backfaceThreshold.EvaluateIn(context);

            Debug.Log("Starting baking of occlusion culling.");

            StaticOcclusionCulling.smallestOccluder = realSmallestOccluder;
            StaticOcclusionCulling.smallestHole = realSmallestHole;
            StaticOcclusionCulling.backfaceThreshold = realBackfaceThreshold;

            StaticOcclusionCulling.GenerateInBackground();

            do
            {
                yield return "";
                if (context.CancelRequested)
                {
                    StaticOcclusionCulling.Cancel();
                }
            }
            while (StaticOcclusionCulling.isRunning);
            Debug.Log("Occlusion culling bake process finished.");
        }

        [MenuItem("Assets/Create/uTomate/Bake/Bake Occlusion Culling", false, 230)]
        public static void AddAction()
        {
            var action = Create<UTBakeOcclusionCullingAction>();
            LoadDefaults(action);
        }

        public static void LoadDefaults(UTBakeOcclusionCullingAction action)
        {
            action.smallestOccluder.UseExpression = false;
            action.smallestOccluder.Value = 5f;

            action.smallestHole.UseExpression = false;
            action.smallestHole.Value = 0.25f;

            action.backfaceThreshold.UseExpression = false;
            action.backfaceThreshold.Value = 100f;
        }
    }
}

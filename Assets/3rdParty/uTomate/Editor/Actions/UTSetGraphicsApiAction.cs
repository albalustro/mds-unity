//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1_0 // VR: 5.1.1
namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections;
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(sinceUTomateVersion = "1.6.0", actionCategory = "Build")]
    [UTDoc(title = "Set Graphics API", description = "Allows to set the graphics API for a build target.")]
    [UTDefaultAction]
    [UTInspectorGroups(groups = new []{ "General"})]
    public class UTSetGraphicsApiAction : UTAction
    {
        [UTDoc(description = "The target platform for which the graphics API should be set.")]
        [UTInspectorHint(group = "General", order = 1, required = true)]
        public UTBuildTarget buildTarget;

        [UTDoc(title = "Automatic graphics API", description = "Should the graphics API be detected automatically.")]
        [UTInspectorHint(group = "General", order = 2)]
        public UTBool automaticGraphicsApi;

        [UTDoc(title = "Graphics APIs", description = "The graphics APIs that should be used for this target platform.")]
        [UTInspectorHint(group = "General", order = 3, required = true)]
        public UTGraphicsDeviceType[] graphicsApis;

        public override IEnumerator Execute(UTContext context)
        {
            var theBuildTarget = buildTarget.EvaluateIn(context);
            var doAutomaticGraphicsApi = automaticGraphicsApi.EvaluateIn(context);

            if (doAutomaticGraphicsApi)
            {
                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Setting graphics API for target platform " + Enum.GetName(typeof(BuildTarget), theBuildTarget) + " to automatic.");
                }
                PlayerSettings.SetUseDefaultGraphicsAPIs(theBuildTarget, true);
            }
            else
            {
                // ReSharper disable once CoVariantArrayConversion
                var graphicsDeviceTypes = EvaluateAll(graphicsApis, context);
                if (graphicsDeviceTypes.Length == 0)
                {
                    throw new UTFailBuildException("You have to specify at least one graphics API.", this);
                }
                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Setting graphics API for target platform " + Enum.GetName(typeof(BuildTarget), theBuildTarget) + " to " + graphicsDeviceTypes );
                }
                PlayerSettings.SetUseDefaultGraphicsAPIs(theBuildTarget, false);
                PlayerSettings.SetGraphicsAPIs(theBuildTarget, graphicsDeviceTypes);
            }
            yield break;

        }

        [MenuItem("Assets/Create/uTomate/Build/Set Graphics API", false, 280)]
        public static void AddAction()
        {
            Create<UTSetGraphicsApiAction>();
        }
    }
}
#endif

//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;

    [CustomEditor(typeof(UTBuildPlayerAction))]
    public class UTBuildPlayerActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo field)
        {
            var action = (UTBuildPlayerAction)target;
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
            if (field.Name == "offlineDeployment" || field.Name == "deployOnline")
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(BuildTarget.WebPlayer, BuildTarget.WebPlayerStreamed) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }
#endif

            if (field.Name == "textureCompression")
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(BuildTarget.Android) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            if (field.Name.In("wsaSdk", "unityCSharpProjects"))
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(BuildTarget.WSAPlayer) ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            if (field.Name == "symlinkLibraries")
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(BuildTarget.iOS) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            if (field.Name == "includes" || field.Name == "excludes")
            {
                return (action.useScenesFromBuildSettings.UseExpression || !action.useScenesFromBuildSettings.Value) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            if (field.Name == "enableHeadlessMode")
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(BuildTarget.StandaloneLinux, BuildTarget.StandaloneLinux64, BuildTarget.StandaloneLinuxUniversal) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            if (field.Name == "addPlatformExtension")
            {
                var platform = action.targetPlatform;
                return platform.HasValueOrExpression(
                    BuildTarget.StandaloneWindows,
                    BuildTarget.StandaloneWindows64,
                    BuildTarget.StandaloneOSXIntel, 
                    BuildTarget.StandaloneOSXUniversal,
                    BuildTarget.Android) ?
                    UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            return base.IsVisible(field);
        }
    }
}

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

    [UTActionInfo(actionCategory = "Build")]
    [UTDoc(title = "Build Unity Player", description = "Builds a player for the desired target platform.")]
    [UTInspectorGroups(groups = new[] {"Player", "Dependencies"})]
    [UTDefaultAction]
    public class UTBuildPlayerAction : UTAction
    {
        [UTDoc(description = "The target platform for which the player should be build.")]
        [UTInspectorHint(group = "Player", order = 0, required = true)]
        public UTBuildTarget targetPlatform;

        [UTDoc(title = "Scenes From Settings",
            description = "If true the list of scenes for the build will be read from Unity's editor build settings. Otherwise you can specify them using the includes/excludes properties.")]
        [UTInspectorHint(group = "Player", order = 1)]
        public UTBool useScenesFromBuildSettings;

        [UTDoc(description = "The scenes to include into the build.")]
        [UTInspectorHint(group = "Player", order = 2)]
        public UTString[] includes;

        [UTDoc(description = "The scenes to exclude from the build.")]
        [UTInspectorHint(group = "Player", order = 3)]
        public UTString[] excludes;

        [UTDoc(description = "Android targets, only: The texture compression.")]
        [UTInspectorHint(group = "Player", order = 4, required = true)]
        public UTMobileTextureSubtarget textureCompression;

        [UTDoc(title = "Unity C# Project", description = "Should a Window Store compatible C# project be created?")]
        [UTInspectorHint(group = "Player", order = 4)]
        public UTBool unityCSharpProjects;

        [UTDoc(title = "WSA SDK", description = "The WSA SDK to be used.")]
        [UTInspectorHint(group = "Player", order = 5, required = true)]
        public UTWsaSdk wsaSdk;

        [UTDoc(description = "Should this be a development build?")]
        [UTInspectorHint(group = "Player", order = 10)]
        public UTBool developmentBuild;

        [UTDoc(description = "Should the build player be run after build?")]
        [UTInspectorHint(group = "Player", order = 11)]
        public UTBool runTheBuiltPlayer;

        [UTDoc(description = "Show the build player in explorer/finder after build?")]
        [UTInspectorHint(group = "Player", order = 12)]
        public UTBool showTheBuiltPlayer;

        [UTDoc(description = "Should the scenes be built in a way that they can be accessed with the WWW class?")]
        [UTInspectorHint(group = "Player", order = 13)]
        public UTBool buildStreamedScenes;

        [UTDoc(description = "Should external modifications to the player be accepted? If not the player will be overwritten when building it.")]
        [UTInspectorHint(group = "Player", order = 14)]
        public UTBool acceptExternalModifications;

        [UTDoc(description = "Web-Player, only: Should UnityObject.js be copied alongside Web Player so it wouldn't have to be downloaded from internet?")]
        [UTInspectorHint(group = "Player", order = 15)]
        public UTBool offlineDeployment;

        [UTDoc(description = "Should the built player try to connect to the profiler in the editor when starting?")]
        [UTInspectorHint(group = "Player", order = 16)]
        public UTBool connectWithProfiler;

        [UTDoc(description = "Should the player be debuggable?")]
        [UTInspectorHint(group = "Player", order = 17)]
        public UTBool allowDebugging;

        [UTDoc(description = "iOS targets, only: should runtime libraries be symlinked when generating iOS XCode project. (Faster iteration time)?")]
        [UTInspectorHint(group = "Player", order = 18)]
        public UTBool symlinkLibraries;

        [UTDoc(description = "Don't compress the data when creating the asset bundle?")]
        [UTInspectorHint(group = "Player", order = 19)]
        public UTBool uncompressedAssetBundle;

        // this has been removed in 5.2
#if UNITY_5_0 || UNITY_5_1 // VR: [5.0,5.1]

        [UTDoc(description = "Web-Player, only: Publish Web Player online?")]
        [UTInspectorHint(group = "Player", order = 20)]
        public UTBool deployOnline;
#endif

        // Unity 4.2 +
        [UTDoc(description = "Linux, only: Enable headless mode?")]
        [UTInspectorHint(group = "Player", order = 21)]
        public UTBool enableHeadlessMode;

        [UTDoc(description = "The output file name of the player. Make sure you add the correct extension for the player type (e.g. .app, .exe, etc.) or tick the 'Add Platform Extension' checkbox.")]
        [UTInspectorHint(required = true, group = "Player", order = 22, displayAs = UTInspectorHint.DisplayAs.SaveFileSelect, caption = "Select output file.")]
        public UTString outputFileName;

        [UTDoc(description = "Should the platform specific extension (e.g. .app, .exe) be appended to the output file name?")]
        [UTInspectorHint(group = "Player", order = 23)]
        public UTBool addPlatformExtension;


        public void OnEnable()
        {
            // property is not yet initialized for new actions, only for existing ones
            if (CreatedWithActionVersion != null && float.Parse(CreatedWithActionVersion) < 1.2f)
            {
                if (targetPlatform.UseExpression || targetPlatform.Value == BuildTarget.Android)
                {
                    Debug.LogWarning("The 'Build Player' action '" + name + "' has been migrated. The value of the 'Texture Compression' property cannot be migrated automatically. " +
                                     "Please check and correct this property, if necessary. You can highlight the migrated " +
                                     "action by clicking on this message.", this);
                }
                CreatedWithActionVersion = ActionVersion;
                EditorUtility.SetDirty(this);
            }
        }

        public override IEnumerator Execute(UTContext context)
        {
            var theOutput = outputFileName.EvaluateIn(context);
            if (string.IsNullOrEmpty(theOutput))
            {
                throw new UTFailBuildException("You must specify an output file name.", this);
            }

            if (theOutput.StartsWith(Application.dataPath))
            {
                throw new UTFailBuildException("Building a player inside the assets folder will break the build. Please place it somewhere else.", this);
            }

            var theTarget = targetPlatform.EvaluateIn(context);

            if (addPlatformExtension.EvaluateIn(context))
            {
                theOutput += GetPlatformExtension(theTarget);
            }

            UTFileUtils.EnsureParentFolderExists(theOutput);

            var useBuildSettings = useScenesFromBuildSettings.EvaluateIn(context);

            string[] scenes;
            if (!useBuildSettings)
            {
                // get them from includes/excludes
                var theIncludes = EvaluateAll(includes, context);
                var theExcludes = EvaluateAll(excludes, context);

                var fileSet = UTFileUtils.CalculateFileset(theIncludes, theExcludes);
                if (fileSet.Length == 0)
                {
                    throw new UTFailBuildException("The file set yielded no scenes to include into the player.", this);
                }

                scenes = fileSet;
            }
            else
            {
                var scenesFromEditor = EditorBuildSettings.scenes;
                if (scenesFromEditor.Length == 0)
                {
                    throw new UTFailBuildException("There are no scenes set up in the editor build settings.", this);
                }
                var active = Array.FindAll(scenesFromEditor, scene => scene.enabled);
                scenes = Array.ConvertAll(active, scene => scene.path);
            }

            if (UTPreferences.DebugMode)
            {
                foreach (var entry in scenes)
                {
                    Debug.Log("Adding scene: " + entry, this);
                }
            }

            var buildOptions = BuildOptions.None;
            if (developmentBuild.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.Development;
            }

            if (runTheBuiltPlayer.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.AutoRunPlayer;
            }

            if (showTheBuiltPlayer.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.ShowBuiltPlayer;
            }

            if (buildStreamedScenes.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.BuildAdditionalStreamedScenes;
            }

            if (acceptExternalModifications.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            }

            if (connectWithProfiler.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.ConnectWithProfiler;
            }

            if (allowDebugging.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.AllowDebugging;
            }

            if (uncompressedAssetBundle.EvaluateIn(context))
            {
                buildOptions |= BuildOptions.UncompressedAssetBundle;
            }

#if  UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
            if (theTarget == BuildTarget.WebPlayer || theTarget == BuildTarget.WebPlayerStreamed)
            {
                if (offlineDeployment.EvaluateIn(context))
                {
                    buildOptions |= BuildOptions.WebPlayerOfflineDeployment;
                }
#if UNITY_5_0 || UNITY_5_1 // VR: [5.0,5.1]
                if (deployOnline.EvaluateIn(context))
                {
                    buildOptions |= BuildOptions.DeployOnline;
                }
#endif
            }
#endif

            if (theTarget == BuildTarget.Android)
            {
                // default value is general (a.k.a. don't override). So if this property is null after the migration we have the same result as before
                EditorUserBuildSettings.androidBuildSubtarget = textureCompression.EvaluateIn(context);
            }

            if (theTarget == BuildTarget.iOS)
            {
                if (symlinkLibraries.EvaluateIn(context))
                {
                    buildOptions |= BuildOptions.SymlinkLibraries;
                }
            }

            if ((theTarget == BuildTarget.StandaloneLinux || theTarget == BuildTarget.StandaloneLinux64 || theTarget == BuildTarget.StandaloneLinuxUniversal))
            {
                if (enableHeadlessMode != null && enableHeadlessMode.EvaluateIn(context))
                {
                    buildOptions |= BuildOptions.EnableHeadlessMode;
                }
            }


            if (theTarget == BuildTarget.WSAPlayer)
            {
                EditorUserBuildSettings.wsaGenerateReferenceProjects = unityCSharpProjects.EvaluateIn(context);
                EditorUserBuildSettings.wsaSDK = wsaSdk.EvaluateIn(context);
            }

            Debug.Log("Building " + ObjectNames.NicifyVariableName(theTarget.ToString()) + " player including " + scenes.Length + " scenes to " + theOutput);
            yield return "";

            // build the player.
            var result = BuildPipeline.BuildPlayer(scenes, theOutput, theTarget, buildOptions);
            if (!string.IsNullOrEmpty(result))
            {
                throw new UTFailBuildException("Building the player failed. " + result, this);
            }
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
            // the reload lock seems to be finally fixed in 5.4 (after being broken since 3.5), for older versions we still need this.
            EditorApplication.LockReloadAssemblies();
#endif
        }

        private static string GetPlatformExtension(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";

                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXUniversal:
                    return ".app";

                case BuildTarget.Android:
                    return ".apk";

                default:
                    return "";
            }
        }

        [MenuItem("Assets/Create/uTomate/Build/Build Unity Player", false, 380)]
        public static void AddAction()
        {
            Create<UTBuildPlayerAction>();
        }
    }
}

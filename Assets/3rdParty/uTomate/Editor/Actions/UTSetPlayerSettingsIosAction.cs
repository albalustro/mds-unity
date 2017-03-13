//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

// This is really a mess. Unity needs to get their act together with the player settings. They change
// all the time and there is no longer a consistent API to change them. So we use different APIs in this
// action. Most of the code here was reverse-engineered from their PlayerSettingsEditor class.



namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Build")]
    [UTDoc(title = "Set iOS Player Settings", description = "Sets the player settings for iOS builds.")]
    [UTInspectorGroups(groups = new[] { "Resolution & Presentation", "Icon", "Splash Image", "Debugging & Crash Reporting", "Rendering", "Identification", "Configuration", "Optimization" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsIosAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        // Resolution & presentation
        [UTDoc(description = "Use animation for auto-rotation?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 2, indentLevel = 1)]
        public UTBool useAnimatedAutoRotation;

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2_0 || UNITY_5_2_1 || UNITY_5_2_2) // VR: 5.2.3
        [UTDoc(description = "When ticked opts out of iOS 9.0 multitasking support.")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 9)]
        public UTBool requiresFullScreen;
#endif

        [UTDoc(description = "Should status bar be hidden?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 10)]
        public UTBool statusBarHidden;

        [UTDoc(description = "The status bar style to use.")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 11)]
        public UTiOSStatusBarStyle statusBarStyle;

#if UNITY_5_0 // VR: [5.0, 5.1)
        [UTDoc(description = "Use 32-bit display buffer?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 12)]
        public UTBool use32BitDisplayBuffer; 
#endif        

        [UTDoc(description = "Disable the depth and stencil buffers?", title = "Disable depth & stencil")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 13)]
        public UTBool disableDepthAndStencil;

        [UTDoc(description = "The type of activity indicator to be shown when the application loads.", title = "Activity Indicator")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 14)]
        public UTiOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading;


        // Icon
        [UTInspectorHint(group = "Icon", order = 1)]
        [UTDoc(description = "Should a different icon be used for iPhone?", title = "Override for iPhone")]
        public UTBool overrideIconForIphone;

        [UTInspectorHint(group = "Icon", order = 2)]
        [UTDoc(description = "Icon for 180x180 pixels.", title = "180x180")]
        public UTTexture2D iconSize180;

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        [UTInspectorHint(group = "Icon", order = 3)]
        [UTDoc(description = "Icon for 167x167 pixels.", title = "167x167")]
        public UTTexture2D iconSize167;
#endif

        [UTInspectorHint(group = "Icon", order = 4)]
        [UTDoc(description = "Icon for 152x152 pixels.", title = "152x152")]
        public UTTexture2D iconSize152;

        [UTInspectorHint(group = "Icon", order = 5)]
        [UTDoc(description = "Icon for 144x144 pixels.", title = "144x144")]
        public UTTexture2D iconSize144;

        [UTInspectorHint(group = "Icon", order = 6)]
        [UTDoc(description = "Icon for 120x120 pixels.", title = "120x120")]
        public UTTexture2D iconSize120;

        [UTInspectorHint(group = "Icon", order = 7)]
        [UTDoc(description = "Icon for 114x114 pixels.", title = "114x114")]
        public UTTexture2D iconSize114;

        [UTInspectorHint(group = "Icon", order = 8)]
        [UTDoc(description = "Icon for 76x76 pixels.", title = "76x76")]
        public UTTexture2D iconSize76;

        [UTInspectorHint(group = "Icon", order = 9)]
        [UTDoc(description = "Icon for 72x72 pixels.", title = "72x72")]
        public UTTexture2D iconSize72;

        [UTInspectorHint(group = "Icon", order = 10)]
        [UTDoc(description = "Icon for 57x57 pixels.", title = "57x57")]
        public UTTexture2D iconSize57;

        [UTInspectorHint(group = "Icon", order = 20)]
        [UTDoc(description = "Icon is prerendered?")]
        public UTBool prerenderedIcon;


        // Splash image
        [UTDoc(description = "The mobile splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 5)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D mobileSplashScreen;

        [UTDoc(title = "iPhone 3.5\"/Retina", description = "High resolution iPhone 3.5\" splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 6)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D highResIphone;

        [UTDoc(title = "iPhone 4\"/Retina", description = "High resolution iPhone 4\" splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 7)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D highResIphoneTall;

        [UTDoc(title = "iPhone 4.7\"/Retina", description = "High resolution iPhone 4.7\" splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 8)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D iphone47Inch;

        [UTDoc(title = "iPhone 5.5\"/Retina Portrait", description = "High resolution iPhone 5.5\" portrait splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 9)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D iphone55InchPortrait;

        [UTDoc(title = "iPhone 5.5\"/Retina Landscape", description = "High resolution iPhone 5.5\" landscape splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 10)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D iphone55InchLandscape;

        [UTDoc(title = "iPad Portrait", description = "iPad portrait splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 11)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D iPadPortrait;

        [UTDoc(title = "iPad Landscape", description = "iPad landscape splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 12)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D iPadLandscape;

        [UTDoc(title = "High Res. iPad Portrait", description = "High resolution iPad portrait splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 13)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D highResIpadPortrait;


        [UTDoc(title = "High Res. iPad Landscape", description = "High resolution iPad landscape splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 14)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTTexture2D highResIpadLandscape;

        [UTDoc(title="iPhone Launch Screen", description = "Launch screen type for iPhone")]
        [UTInspectorHint(group = "Splash Image", order = 15)]
        public UTIosLaunchScreenType iPhoneLaunchScreen;

        [UTDoc(description = "Portrait image for the launch screen.", title = "Portrait Image")]
        [UTInspectorHint(group = "Splash Image", order = 16, indentLevel = 1)]
        public UTTexture2D launchScreenPortraitImage;

        [UTDoc(description = "Landscape image for the launch screen.", title = "Landscape Image")]
        [UTInspectorHint(group = "Splash Image", order = 17, indentLevel = 1)]
        public UTTexture2D launchScreenLandscapeImage;

        [UTDoc(description = "Background color of the launch screen.", title = "Background Color")]
        [UTInspectorHint(group = "Splash Image", order = 18, indentLevel = 1)]
        public UTColor launchScreenBackgroundColor;

        [UTDoc(description = "Fill percentage of the launch screen image.", title = "Fill Percentage")]
        [UTInspectorHint(group = "Splash Image", order = 19, indentLevel = 1, minValue = 0, maxValue = 100f, displayAs = UTInspectorHint.DisplayAs.Slider)]
        public UTFloat launchScreenFillPercentage;

#if !UNITY_5_0 // VR: 5.1
        [UTDoc(description = "Size in points of the launch screen image.", title = "Size in Points")]
        [UTInspectorHint(group = "Splash Image", order = 20, indentLevel = 1)]
        public UTFloat launchScreenSizeInPoints;
#endif


        [UTDoc(description = "Path to a custom XIB file ", title = "Custom XIB")]
        [UTInspectorHint(group = "Splash Image", order = 21, indentLevel = 1, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect)]
        public UTString customXibPath;

        [UTDoc(title="iPad Launch Screen", description = "Launch screen type for iPad")]
        [UTInspectorHint(group = "Splash Image", order = 22)]
        public UTIosLaunchScreenType iPadLaunchScreen;

        [UTDoc(description = "Portrait image for the launch screen.", title = "Image")]
        [UTInspectorHint(group = "Splash Image", order = 23, indentLevel = 1)]
        public UTTexture2D iPadLaunchScreenImage;

        [UTDoc(description = "Background color of the launch screen.", title = "Background Color")]
        [UTInspectorHint(group = "Splash Image", order = 24, indentLevel = 1)]
        public UTColor iPadLaunchScreenBackgroundColor;

        [UTDoc(description = "Fill percentage of the launch screen image.", title = "Fill Percentage")]
        [UTInspectorHint(group = "Splash Image", order = 25, indentLevel = 1, minValue = 0, maxValue = 100f, displayAs = UTInspectorHint.DisplayAs.Slider)]
        public UTFloat iPadLaunchScreenFillPercentage;

#if !UNITY_5_0 // VR: 5.1
        [UTDoc(description = "Size in points of the launch screen image.", title = "Size in Points")]
        [UTInspectorHint(group = "Splash Image", order = 26, indentLevel = 1)]
        public UTFloat iPadLaunchScreenSizeInPoints;
#endif

        [UTDoc(description = "Path to a custom XIB file ", title = "Custom XIB")]
        [UTInspectorHint(group = "Splash Image", order = 27, indentLevel = 1, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect)]
        public UTString iPadCustomXibPath;


        // Debugging & Crash Reporting 
        [UTDoc(description = "Enable internal profiler.")]
        [UTInspectorHint(group = "Debugging & Crash Reporting", order = 1)]
        public UTBool enableInternalProfiler;

        [UTDoc(description = "What should be done when an unhandled .NET exception occurs?", title = "On unhandled .NET exception")]
        [UTInspectorHint(group = "Debugging & Crash Reporting", order = 2)]
        public UTActionOnDotNetUnhandledException onUnhandledDotNetException;

        [UTDoc(description = "Log uncaught Objective-C exceptions?", title = "Log uncaught Obj-C exceptions")]
        [UTInspectorHint(group = "Debugging & Crash Reporting", order = 3)]
        public UTBool logUncaughtObjectiveCExceptions;

        [UTDoc(description = "Enable crash report API?", title = "Enable crash report API")]
        [UTInspectorHint(group = "Debugging & Crash Reporting", order = 4)]
        public UTBool enableCrashReportApi;


        // Identification
        [UTDoc(description = "Application bundle identifier.")]
        [UTInspectorHint(group = "Identification", order = 1, required = true)]
        public UTString bundleIdentifier;

        [UTDoc(description = "Application bundle version.")]
        [UTInspectorHint(group = "Identification", order = 2, required = true)]
        public UTString bundleVersion;

        // Configuration
        [UTInspectorHint(group = "Configuration", order = 0)]
        [UTDoc(description = "Which scripting backend should be used?")]
        public UTScriptingImplementation scriptingBackend;

#if UNITY_5_0 // VR: [5.0, 5.1)
        [UTInspectorHint(group = "Configuration", order = 1)]
        [UTDoc(title="Use IL2CPP Precompiled Header", description = "Should a precompiled header be used for IL2CPP? This can improve performance in some cases, however it may lead to performance degradation in other cases.")]
        public UTBool useIl2CppPrecompiledHeader;
#endif

        [UTDoc(description = "The target device to build for.")]
        [UTInspectorHint(group = "Configuration", order = 2)]
        public UTiOSTargetDevice targetDevice;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR: [5.0, 5.2]
        [UTDoc(description = "Targeted resolution.")]
        [UTInspectorHint(group = "Configuration", order = 3)]
        public UTiOSTargetResolution targetResolution;
#endif
        
#if UNITY_5_0  // VR: [5.0, 5.0]
        [UTDoc(description = "Target GLES graphics.", title = "Graphics API")]
        [UTInspectorHint(group = "Configuration", order = 4)]
        public UTTargetGlesGraphics targetGlesGraphics;
#endif



        [UTDoc(description = "Accelerometer frequency in Hertz. Valid values are 0, 15, 30, 60 and 100.")]
        [UTInspectorHint(group = "Configuration", order = 5)]
        public UTInt accelerometerFrequency;

        [UTDoc(description = "The reason for using the player's location.")]
        [UTInspectorHint(group = "Configuration", order = 6)]
        public UTString locationUsageDescription;

        [UTDoc(description = "Should the iOS recording APIs be initialised?", title = "Prepare iOS for recording")]
        [UTInspectorHint(group = "Configuration", order = 8)]
        public UTBool prepareIosForRecording;

        [UTInspectorHint(group = "Configuration", order = 9)]
        [UTDoc(description = "Application requires persistent WiFi?")]
        public UTBool requiresPersistentWifi;

        [HideInInspector] // obsolete with Unity 5 but we need the value for migration so we keep the property.
        public UTBool exitOnSuspend;

        [UTDoc(description = "How should the application behave when in background?")]
        [UTInspectorHint(group = "Configuration", order = 10)]
        public UTiOSAppInBackgroundBehavior backgroundBehaviour;

        [UTDoc(description = "Architecture to target.")]
        [UTInspectorHint(group = "Configuration", order = 21)]
        public UTIosArchitecture architecture;

        // Optimization
        [UTDoc(description = "Additional AOT compilation options.")]
        [UTInspectorHint(group = "Optimization", order = 5)]
        public UTString aotCompilationOptions;

        [UTDoc(description = "Active iOS SDK version used for build")]
        [UTInspectorHint(group = "Optimization", order = 6)]
        public UTiOSSdkVersion sdkVersion;

        [UTDoc(description = "Deployment minimal version of iOS.")] [UTInspectorHint(group = "Optimization", order = 7)]
        public UTString targetOsVersion;


        [UTDoc(description = "Script calling optimization level.")]
        [UTInspectorHint(group = "Optimization", order = 9)]
        public UTScriptCallOptimizationLevel scriptCallOptimizationLevel;


        // Version Enum -> String map for enum change in Unity 5.5
        private static readonly Dictionary<string,string> versionMap = new Dictionary<string, string>();

        static UTSetPlayerSettingsIosAction()
        {
#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: [5.0, 5.3]
            versionMap["22"] = "6.0";
#endif
            versionMap["24"] = "7.0";
            versionMap["26"] = "7.1";
            versionMap["28"] = "8.0";
            versionMap["30"] = "8.1";
#if !UNITY_5_0 // VR: 5.1
            versionMap["32"] = "8.2";
            versionMap["34"] = "8.3";
            versionMap["36"] = "8.4";
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.1, 5.3]
            versionMap["40"] = "9.0";
#else
            versionMap["38"] = "9.0"; // yes this changed in 5.4...
#endif
#if !(UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            versionMap["40"] = "9.1"; // and 40 is now 9.1 in 5.4
            versionMap["42"] = "9.2";
            versionMap["44"] = "9.3";
            versionMap["46"] = "10.0";
#endif
#endif
        }

        public void OnEnable()
        {
            // property is not yet initialized for new actions, only for existing ones
            UTils.MigrateActionIfRequired(this, "1.2", delegate(UTSetPlayerSettingsIosAction action)
            {
                if (action.exitOnSuspend.UseExpression)
                {
                    action.backgroundBehaviour.UseExpression = true;
                    action.backgroundBehaviour.Expression = "(" + action.exitOnSuspend.Expression + ") ? 'Exit' : 'Suspend'";
                    Debug.LogWarning("The 'Set Player Settings IOS' action '" + action.name + "' has been migrated. Please check, if " +
                        "the expression in the 'Background Behaviour' field is still correct. You can highlight the migrated " +
                                     "action by clicking on this message.", action);
                }
                else
                {
                    action.backgroundBehaviour.UseExpression = false;
                    action.backgroundBehaviour.Value = action.exitOnSuspend.Value ? iOSAppInBackgroundBehavior.Exit : iOSAppInBackgroundBehavior.Suspend;
                }

            });

            UTils.MigrateActionIfRequired(this, "1.6", delegate(UTSetPlayerSettingsIosAction action)
            {
                if (action.targetOsVersion.UseExpression)
                {
                    Debug.Log(
                        "The 'Set Player Settings IOS' action now uses a string field for the target IOS version instead of an enum. " +
                        "Your action '" + action.name +
                        "' uses an expression for this field which cannot be automatically migrated. Please " +
                        "check and correct the expression in the 'Target OS Version' field to return a proper version string now. You can " +
                        "highlight the action by clicking on this message.", action);
                }
                else
                {
                    // this is being migrated from an enum to a string in Unity 5.5
                    if (versionMap.ContainsKey(action.targetOsVersion.Value))
                    {
                        action.targetOsVersion.StaticValue = versionMap[action.targetOsVersion.Value];
                        Debug.Log(
                            "The 'Set Player Settings IOS' action now uses a string field for the target iOS version. Your action " +
                            action.name +
                            "' has been migrated automatically. Please verify that the migrated value is correct. You can " +
                            "highlight the action by clicking on this message.", action);
                    }
                    else
                    {
                        // use a proper version in this case
                        action.targetOsVersion.StaticValue = "7.0";

                        Debug.Log(
                            "The 'Set Player Settings IOS' action now uses a string field for the target iOS version. Your action " +
                            action.name +
                            "' could not be migrated automatically. Please check the value of the 'Target OS Version' property. You can " +
                            "highlight the action by clicking on this message.", action);

                    }

                }
            });
        }

        public override IEnumerator Execute(UTContext context)
        {

            if (!UTils.IsPlatformSupportLoaded(BuildTarget.iOS))
            {
                Debug.LogWarning("iOS module is not loaded. Cannot change iOS player settings.");
                yield break;
            }

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying iOS player settings.", this);
            }
            var theBundleIdentifier = bundleIdentifier.EvaluateIn(context);
            if (string.IsNullOrEmpty(theBundleIdentifier))
            {
                throw new UTFailBuildException("You need to specify the bundle identifier.", this);
            }

            var theBundleVersion = bundleVersion.EvaluateIn(context);
            if (string.IsNullOrEmpty(theBundleVersion))
            {
                throw new UTFailBuildException("You need to specify the bundle version.", this);
            }

            var theFrequency = accelerometerFrequency.EvaluateIn(context);
            if (theFrequency != 0 && theFrequency != 15 && theFrequency != 30 && theFrequency != 60 && theFrequency != 100)
            {
                throw new UTFailBuildException("Invalid accelerometer frequency. Valid values for accelerometer frequencies are 0, 15, 30, 60 and 100.", this);
            }

            var theTargetOsVersion = targetOsVersion.EvaluateIn(context);
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Version(theTargetOsVersion);
            }
            catch (Exception)
            {
                throw new UTFailBuildException("The given target OS version '" + theTargetOsVersion + "' is no valid version string.", this);
            }

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
            // The exposed enum isn't complete, at least not in Unity 5.2. So we're using our conversion map to come up
            // with the correct value.
            var key = "";
            foreach (var entry in versionMap)
            {
                if (entry.Value == theTargetOsVersion)
                {
                    key = entry.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new UTFailBuildException("The iOS version " + theTargetOsVersion + " not supported by this version of Unity.", this);
            }
            var theTargetOsVersionEnum = int.Parse(key);
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2_0 || UNITY_5_2_1 || UNITY_5_2_2) // VR: 5.2.3
            PlayerSettings.iOS.requiresFullScreen = requiresFullScreen.EvaluateIn(context);
#endif
            PlayerSettings.statusBarHidden = statusBarHidden.EvaluateIn(context);
            PlayerSettings.iOS.statusBarStyle = statusBarStyle.EvaluateIn(context);
#if UNITY_5_0 // VR: [5.0, 5.1)
            PlayerSettings.use32BitDisplayBuffer = use32BitDisplayBuffer.EvaluateIn(context);
#endif           
            PlayerSettings.iOS.showActivityIndicatorOnLoading = showActivityIndicatorOnLoading.EvaluateIn(context);



            PlayerSettings.iOS.prerenderedIcon = prerenderedIcon.EvaluateIn(context);

            PlayerSettings.bundleIdentifier = theBundleIdentifier;
            PlayerSettings.bundleVersion = theBundleVersion;

            PlayerSettings.iOS.targetDevice = targetDevice.EvaluateIn(context);

#if UNITY_5_0  // VR: [5.0, 5.0]
            PlayerSettings.targetGlesGraphics = targetGlesGraphics.EvaluateIn(context);
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR: [5.0, 5.2]
            PlayerSettings.iOS.targetResolution = targetResolution.EvaluateIn(context);
#endif
            PlayerSettings.accelerometerFrequency = theFrequency;
            PlayerSettings.iOS.requiresPersistentWiFi = requiresPersistentWifi.EvaluateIn(context);
            PlayerSettings.iOS.appInBackgroundBehavior = backgroundBehaviour.EvaluateIn(context);
            PlayerSettings.aotOptions = aotCompilationOptions.EvaluateIn(context);
            PlayerSettings.iOS.sdkVersion = sdkVersion.EvaluateIn(context);
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            PlayerSettings.iOS.targetOSVersionString = theTargetOsVersion;

#else
            // this actually works, even if the int is outside of the range of the exposed enum
            PlayerSettings.iOS.targetOSVersion = (iOSTargetOSVersion) theTargetOsVersionEnum;
#endif
            PlayerSettings.iOS.scriptCallOptimization = scriptCallOptimizationLevel.EvaluateIn(context);
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, scriptingBackend.EvaluateIn(context));
#else
            PlayerSettings.SetPropertyInt("ScriptingBackend", (int)scriptingBackend.EvaluateIn(context), BuildTarget.iOS);
#endif
#if UNITY_5_0 // VR: [5.0, 5.1)
			PlayerSettings.SetPropertyBool("UseIl2CppPrecompiledHeader", useIl2CppPrecompiledHeader.EvaluateIn(context), BuildTarget.iOS);
#endif

            if (overrideIconForIphone.EvaluateIn(context))
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, new[]
                {
                    iconSize180.EvaluateIn(context),
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                    iconSize167.EvaluateIn(context),
#endif
                    iconSize152.EvaluateIn(context),
                    iconSize144.EvaluateIn(context),
                    iconSize120.EvaluateIn(context),
                    iconSize114.EvaluateIn(context),
                    iconSize76.EvaluateIn(context),
                    iconSize72.EvaluateIn(context),
                    iconSize57.EvaluateIn(context)
                });
            }
            else
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, new Texture2D[0]);
            }

            // all the stuff that is not available through the regular API.
            using (var wrapper = new UTPlayerSettingsWrapper())
            {
                wrapper.SetInt("iOSLaunchScreenType", (int)iPhoneLaunchScreen.EvaluateIn(context));
#if UNITY_5_0 // VR: [5.0, 5.1)
				wrapper.SetInt("iOSLaunchScreenFillPct", Mathf.RoundToInt(launchScreenFillPercentage.EvaluateIn(context)));
#else
                wrapper.SetFloat("iOSLaunchScreenFillPct", launchScreenFillPercentage.EvaluateIn(context));
#endif

#if !UNITY_5_0 // VR: 5.1
                wrapper.SetFloat("iOSLaunchScreenSize", launchScreenSizeInPoints.EvaluateIn(context));
#endif
                wrapper.SetObject("iOSLaunchScreenPortrait", launchScreenPortraitImage.EvaluateIn(context));
                wrapper.SetObject("iOSLaunchScreenLandscape", launchScreenLandscapeImage.EvaluateIn(context));
                wrapper.SetColor("iOSLaunchScreenBackgroundColor", launchScreenBackgroundColor.EvaluateIn(context));

                var theCustomXibPath = customXibPath.EvaluateIn(context);
                if (!string.IsNullOrEmpty(theCustomXibPath))
                {
                    // convert to project relative path as required by the property
                    theCustomXibPath = UTFileUtils.FullPathToProjectPath(theCustomXibPath);
                }
                wrapper.SetString("iOSLaunchScreenCustomXibPath", theCustomXibPath);


#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                wrapper.SetInt("iOSLaunchScreeniPadType", (int) iPadLaunchScreen.EvaluateIn(context));
                wrapper.SetFloat("iOSLaunchScreeniPadFillPct", iPadLaunchScreenFillPercentage.EvaluateIn(context));
                wrapper.SetFloat("iOSLaunchScreeniPadSize", iPadLaunchScreenSizeInPoints.EvaluateIn(context));
                wrapper.SetObject("iOSLaunchScreeniPadImage", iPadLaunchScreenImage.EvaluateIn(context));
                wrapper.SetColor("iOSLaunchScreeniPadBackgroundColor", iPadLaunchScreenBackgroundColor.EvaluateIn(context));
                var theIpadCustomXibPath = iPadCustomXibPath.EvaluateIn(context);
                if (!string.IsNullOrEmpty(theIpadCustomXibPath))
                {
                    theIpadCustomXibPath = UTFileUtils.FullPathToProjectPath(theIpadCustomXibPath);
                }
                wrapper.SetString("iOSLaunchScreeniPadCustomXibPath", theIpadCustomXibPath);
#endif

                wrapper.SetBool("useOSAutorotation", useAnimatedAutoRotation.EvaluateIn(context));

                wrapper.SetBool("uIPrerenderedIcon", prerenderedIcon.EvaluateIn(context));

                wrapper.SetObject("iPhoneSplashScreen", mobileSplashScreen.EvaluateIn(context));
                wrapper.SetObject("iPhoneHighResSplashScreen", highResIphone.EvaluateIn(context));
                wrapper.SetObject("iPhoneTallHighResSplashScreen", highResIphoneTall.EvaluateIn(context));
                wrapper.SetObject("iPhone47inSplashScreen", iphone47Inch.EvaluateIn(context));
                wrapper.SetObject("iPhone55inPortraitSplashScreen", iphone55InchPortrait.EvaluateIn(context));
                wrapper.SetObject("iPhone55inLandscapeSplashScreen", iphone55InchLandscape.EvaluateIn(context));
                wrapper.SetObject("iPadPortraitSplashScreen", iPadPortrait.EvaluateIn(context));
                wrapper.SetObject("iPadLandscapeSplashScreen", iPadLandscape.EvaluateIn(context));
                wrapper.SetObject("iPadHighResPortraitSplashScreen", highResIpadPortrait.EvaluateIn(context));
                wrapper.SetObject("iPadHighResLandscapeSplashScreen", highResIpadLandscape.EvaluateIn(context));

                wrapper.SetBool("disableDepthAndStencilBuffers", disableDepthAndStencil.EvaluateIn(context));
                wrapper.SetBool("enableInternalProfiler", enableInternalProfiler.EvaluateIn(context));
                wrapper.SetInt("actionOnDotNetUnhandledException", (int)onUnhandledDotNetException.EvaluateIn(context));
                wrapper.SetBool("logObjCUncaughtExceptions", logUncaughtObjectiveCExceptions.EvaluateIn(context));
                wrapper.SetBool("enableCrashReportAPI", enableCrashReportApi.EvaluateIn(context));
                wrapper.SetString("locationUsageDescription", locationUsageDescription.EvaluateIn(context));
                wrapper.SetBool("Prepare IOS For Recording", prepareIosForRecording.EvaluateIn(context));

                ApplyCommonSettings(wrapper, context);
            }

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, (int) architecture.EvaluateIn(context));
#else
            PlayerSettings.SetPropertyInt("Architecture", (int)architecture.EvaluateIn(context), BuildTargetGroup.iOS);
#endif

            if (UTPreferences.DebugMode)
            {
                Debug.Log("iOS player settings modified.", this);
            }

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Build/Set iOS Player Settings", false, 250)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsIosAction>();
            result.LoadSettings();
        }

        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {

            if (!UTils.IsPlatformSupportLoaded(BuildTarget.iOS))
            {
                Debug.LogWarning("iOS module is not loaded. Cannot load current iOS settings.");
                return;
            }

            var wrapper = new UTPlayerSettingsWrapper();

            useAnimatedAutoRotation.StaticValue = wrapper.GetBool("useOSAutorotation");
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2_0 || UNITY_5_2_1 || UNITY_5_2_2) // VR: 5.2.3
            requiresFullScreen.StaticValue = PlayerSettings.iOS.requiresFullScreen;
#endif
            statusBarHidden.StaticValue = PlayerSettings.statusBarHidden;
            statusBarStyle.StaticValue = PlayerSettings.iOS.statusBarStyle;

#if UNITY_5_0 // VR: [5.0, 5.1)
            use32BitDisplayBuffer.StaticValue = PlayerSettings.use32BitDisplayBuffer;
#endif

            showActivityIndicatorOnLoading.StaticValue = PlayerSettings.iOS.showActivityIndicatorOnLoading;

            bundleIdentifier.StaticValue = PlayerSettings.bundleIdentifier;
            bundleVersion.StaticValue = PlayerSettings.bundleVersion;

            targetDevice.StaticValue = PlayerSettings.iOS.targetDevice;
#if UNITY_5_0  // VR: [5.0, 5.0]
            targetGlesGraphics.StaticValue = PlayerSettings.targetGlesGraphics;
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR: [5.0, 5.2]
            targetResolution.StaticValue = PlayerSettings.iOS.targetResolution;
#endif
            accelerometerFrequency.StaticValue = PlayerSettings.accelerometerFrequency;
            requiresPersistentWifi.StaticValue = PlayerSettings.iOS.requiresPersistentWiFi;
            exitOnSuspend.StaticValue = PlayerSettings.iOS.appInBackgroundBehavior == iOSAppInBackgroundBehavior.Exit;
            backgroundBehaviour.StaticValue = PlayerSettings.iOS.appInBackgroundBehavior;


            aotCompilationOptions.StaticValue = PlayerSettings.aotOptions;
            sdkVersion.StaticValue = PlayerSettings.iOS.sdkVersion;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            targetOsVersion.StaticValue = PlayerSettings.iOS.targetOSVersionString;
#else
            var version = (int) PlayerSettings.iOS.targetOSVersion;
            if (version != 999) // 999 == Unknown, I'm not sure what unknown is supposed to mean
            {
                // Unknown
                var versionString = version.ToString();
                string result;
                if (versionMap.TryGetValue(versionString, out result))
                {
                    targetOsVersion.StaticValue = result;
                }
                else
                {
                    Debug.LogWarning("Unable to convert enum value " + version +
                                     " into version string. This is almost certainly a bug. Please report this to support@ancientlightstudios.com. Thank you very much.");
                }
            }
            else
            {
                targetOsVersion.StaticValue = "7.0";
            }
#endif
            scriptCallOptimizationLevel.StaticValue = PlayerSettings.iOS.scriptCallOptimization;

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            scriptingBackend.StaticValue = PlayerSettings.GetScriptingBackend(BuildTargetGroup.iOS);
#else
            scriptingBackend.StaticValue = (ScriptingImplementation)PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.iOS);
#endif

#if UNITY_5_0 // VR: [5.0, 5.1)
			var il2ppsetting = false;
            PlayerSettings.GetPropertyOptionalBool("UseIl2CppPrecompiledHeader", ref il2ppsetting, BuildTargetGroup.iOS );
			useIl2CppPrecompiledHeader.StaticValue = il2ppsetting;
#endif

            disableDepthAndStencil.StaticValue = wrapper.GetBool("disableDepthAndStencilBuffers");
            enableInternalProfiler.StaticValue = wrapper.GetBool("enableInternalProfiler");
            onUnhandledDotNetException.StaticValue = (ActionOnDotNetUnhandledException)wrapper.GetInt("actionOnDotNetUnhandledException");
            logUncaughtObjectiveCExceptions.StaticValue = wrapper.GetBool("logObjCUncaughtExceptions");
            enableCrashReportApi.StaticValue = wrapper.GetBool("enableCrashReportAPI");

            locationUsageDescription.StaticValue = wrapper.GetString("locationUsageDescription");

            prepareIosForRecording.StaticValue = wrapper.GetBool("Prepare IOS For Recording");


#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            architecture.StaticValue = (IosArchitecture)PlayerSettings.GetArchitecture(BuildTargetGroup.iOS);
#else
            architecture.StaticValue = (IosArchitecture)PlayerSettings.GetPropertyInt("Architecture", BuildTargetGroup.iOS);
#endif

            mobileSplashScreen.StaticValue = wrapper.GetObject("iPhoneSplashScreen") as Texture2D;
            highResIphone.StaticValue = wrapper.GetObject("iPhoneHighResSplashScreen") as Texture2D;
            highResIphoneTall.StaticValue = wrapper.GetObject("iPhoneTallHighResSplashScreen") as Texture2D;
            iphone47Inch.StaticValue = wrapper.GetObject("iPhone47inSplashScreen") as Texture2D;
            iphone55InchPortrait.StaticValue = wrapper.GetObject("iPhone55inPortraitSplashScreen") as Texture2D;
            iphone55InchLandscape.StaticValue = wrapper.GetObject("iPhone55inLandscapeSplashScreen") as Texture2D;
            iPadPortrait.StaticValue = wrapper.GetObject("iPadPortraitSplashScreen") as Texture2D;
            iPadLandscape.StaticValue = wrapper.GetObject("iPadLandscapeSplashScreen") as Texture2D;
            highResIpadPortrait.StaticValue = wrapper.GetObject("iPadHighResPortraitSplashScreen") as Texture2D;
            highResIpadLandscape.StaticValue = wrapper.GetObject("iPadHighResLandscapeSplashScreen") as Texture2D;

            var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.iOS);

            if (icons != null && icons.Length > 0)
            {
                // did I say that this is a MESS?
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                const int NumberOfIcons = 9;
#else
                const int NumberOfIcons = 8;
#endif

                if (icons.Length == NumberOfIcons)
                {
                    var index = 0;
                    iconSize180.StaticValue = icons[index++];
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                    iconSize167.StaticValue = icons[index++];
#endif
                    iconSize152.StaticValue = icons[index++];
                    iconSize144.StaticValue = icons[index++];
                    iconSize120.StaticValue = icons[index++];
                    iconSize114.StaticValue = icons[index++];
                    iconSize76.StaticValue = icons[index++];
                    iconSize72.StaticValue = icons[index++];
                    iconSize57.StaticValue = icons[index++];
                    overrideIconForIphone.StaticValue = true;
                }
                else
                {
                    Debug.LogWarning("Amount of icon sizes for iOS has changed (was " + icons.Length + " but should be " + NumberOfIcons + "). Please run this action again to update the number of images. If you still see this issue after running this action, please report this to support@ancientlightstudios.com. Thank you!", this);
                }
            }
            else
            {
                overrideIconForIphone.StaticValue = false;
                iconSize180.StaticValue = null;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                iconSize167.StaticValue = null;
#endif
                iconSize152.StaticValue = null;
                iconSize144.StaticValue = null;
                iconSize120.StaticValue = null;
                iconSize114.StaticValue = null;
                iconSize76.StaticValue = null;
                iconSize72.StaticValue = null;
                iconSize57.StaticValue = null;
            }

            prerenderedIcon.StaticValue = wrapper.GetBool("uIPrerenderedIcon");


            iPhoneLaunchScreen.StaticValue = (IosLaunchScreenType)wrapper.GetInt("iOSLaunchScreenType");
#if UNITY_5_0 // VR: [5.0, 5.1)
			launchScreenFillPercentage.StaticValue = wrapper.GetInt ("iOSLaunchScreenFillPct");
#else
            launchScreenFillPercentage.StaticValue = wrapper.GetFloat("iOSLaunchScreenFillPct");

#endif
#if !UNITY_5_0 // VR: 5.1
            launchScreenSizeInPoints.StaticValue = wrapper.GetFloat("iOSLaunchScreenSize");
#endif
            launchScreenPortraitImage.StaticValue = wrapper.GetObject("iOSLaunchScreenPortrait") as Texture2D;
            launchScreenLandscapeImage.StaticValue = wrapper.GetObject("iOSLaunchScreenLandscape") as Texture2D;
            launchScreenBackgroundColor.StaticValue = wrapper.GetColor("iOSLaunchScreenBackgroundColor");

            var xibPath = wrapper.GetString("iOSLaunchScreenCustomXibPath");
            if (!string.IsNullOrEmpty(xibPath))
            {
                // convert to full path
                xibPath = UTFileUtils.CombineToPath(UTFileUtils.ProjectRoot, xibPath);
            }
            customXibPath.StaticValue = xibPath ?? "";

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
            iPadLaunchScreen.StaticValue = (IosLaunchScreenType) wrapper.GetInt("iOSLaunchScreeniPadType");
            iPadLaunchScreenFillPercentage.StaticValue = wrapper.GetFloat("iOSLaunchScreeniPadFillPct");
            iPadLaunchScreenSizeInPoints.StaticValue = wrapper.GetFloat("iOSLaunchScreeniPadSize");
            iPadLaunchScreenImage.StaticValue = wrapper.GetObject("iOSLaunchScreeniPadImage") as Texture2D;
            iPadLaunchScreenBackgroundColor.StaticValue = wrapper.GetColor("iOSLaunchScreeniPadBackgroundColor");

            var iPadXibPath = wrapper.GetString("iOSLaunchScreeniPadCustomXibPath");
            if (!string.IsNullOrEmpty(xibPath))
            {
                // to full path
                iPadXibPath = UTFileUtils.CombineToPath(UTFileUtils.ProjectRoot, iPadXibPath);
            }
            iPadCustomXibPath.StaticValue = iPadXibPath ?? "";
#endif

            LoadCommonSettings(wrapper);

        }

        public string LoadSettingsUndoText
        {
            get
            {
                return "Load iOS specific player settings";
            }
        }

        /// <summary>
        /// This is our own version of the inaccessible UnityEditor.iOS.Architecture
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum IosArchitecture
        {
            ARMv7,
            ARM64,
            Universal,
        }

        /// <summary>
        /// This is our own version of the inaccessible UnityEditor.iOSLaunchScreenType
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum IosLaunchScreenType
        {
            Default,
            ImageAndBackgroundRelative,
            CustomXib,
#if !UNITY_5_0 // VR: 5.1
            None,
            ImageAndBackgroundConstant,
#endif
        }

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        public override bool SupportsVirtualReality
        {
            get { return false; }
        }
#endif

        protected override bool IsMobilePlatform
        {
            get
            {
               return true;
            }
        }

        public override bool SupportsMutingAudio
        {
            get { return true; }
        }

        protected override BuildTarget Platform
        {
            get
            {
                return BuildTarget.iOS;
            }
        }

        public override bool SupportsStripping { get { return true;  } }

        public override bool HasAutorotation
        {
            get { return true; }
        }
    }
}


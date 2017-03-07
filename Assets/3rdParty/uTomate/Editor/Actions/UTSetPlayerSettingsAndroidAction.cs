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
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Build", sinceUTomateVersion = "1.1.0")]
    [UTDoc(title = "Set Android Player Settings", description = "Sets the player settings for Android builds.")]
    [UTInspectorGroups(groups = new string[] { "Resolution & Presentation", "Icon", "Splash Image", "Rendering", "Identification", "Configuration", "Optimization", "Publishing Settings" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsAndroidAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        // Resolution & Presentation
        [UTDoc(description = "Should status bar be hidden?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 10)]
        public UTBool statusBarHidden;

        [UTDoc(description = "Use 32-bit display buffer?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 11)]
        public UTBool use32BitDisplayBuffer;

        [UTDoc(title = "Disable Depth and Stencil Buffers", description = "Disable depth and stencil buffers?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 12)]
        public UTBool use24BitDepthBuffer;

        [UTDoc(description = "The type of activity indicator to be shown when the application loads.", title = "Activity Indicator")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 13)]
        public UTAndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading;

        // Icon
        [UTInspectorHint(group = "Icon", order = 1)]
        [UTDoc(description = "Should a different icon be used for Android?", title = "Override for Android")]
        public UTBool overrideIconForAndroid;

        [UTInspectorHint(group = "Icon", order = 2)]
        [UTDoc(description = "Icon for 192x192 pixels.", title = "192x192")]
        public UTTexture2D iconSize192;

        [UTInspectorHint(group = "Icon", order = 3)]
        [UTDoc(description = "Icon for 144x144 pixels.", title = "144x144")]
        public UTTexture2D iconSize144;

        [UTInspectorHint(group = "Icon", order = 4)]
        [UTDoc(description = "Icon for 96x96 pixels.", title = "96x96")]
        public UTTexture2D iconSize96;

        [UTInspectorHint(group = "Icon", order = 5)]
        [UTDoc(description = "Icon for 72x72 pixels.", title = "72x72")]
        public UTTexture2D iconSize72;

        [UTInspectorHint(group = "Icon", order = 6)]
        [UTDoc(description = "Icon for 48x48 pixels.", title = "48x48")]
        public UTTexture2D iconSize48;

        [UTInspectorHint(group = "Icon", order = 7)]
        [UTDoc(description = "Icon for 36x36 pixels.", title = "36x36")]
        public UTTexture2D iconSize36;

        [UTInspectorHint(group = "Icon", order = 20)]
        [UTDoc(description = "Should the banner for android be enabled.?")]
        public UTBool enableAndroidBanner;

        [UTInspectorHint(group = "Icon", order = 21)]
        [UTDoc(description = "Banner for 320x180 pixels", title = "320x180")]
        public UTTexture2D banner320x180;

        // Splash Image
        [UTDoc(description = "The mobile splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 5)]
        [UTRequiresLicense(UTLicense.AndroidPro)]
        public UTTexture2D mobileSplashScreen;

        [UTDoc(description = "Scaling operation to apply on the android splash image.")]
        [UTInspectorHint(group = "Splash Image", order = 6)]
        [UTRequiresLicense(UTLicense.AndroidPro)]
        public UTAndroidSplashScreenScale splashScaling;

        // Rendering
        [UTDoc(description = "Enable multithreaded rendering?")]
        [UTInspectorHint(group = "Rendering", order = 2)]
        public UTBool multithreadedRendering;
        
#if !UNITY_5_0 // VR: 5.1    
        [UTDoc(description="Does the game support virtual reality headsets?")]
        [UTInspectorHint(group = "Rendering", order = 8)]        
        public UTBool virtualRealitySupported;
#endif


#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
        [UTDoc(description = "Protect GPU memory from being read (on supported devices). Will prevent user from taking screenshots.")]
        [UTInspectorHint(group = "Rendering", order = 10)]
        public UTBool protectGraphicsMemory;
#endif

        // Identification
        [UTDoc(description = "Application bundle identifier.")]
        [UTInspectorHint(group = "Identification", order = 1, required = true)]
        public UTString bundleIdentifier;

        [UTDoc(description = "Application bundle versions.")]
        [UTInspectorHint(group = "Identification", order = 2, required = true)]
        public UTString bundleVersion;

        [UTDoc(description = "Bundle version code")]
        [UTInspectorHint(group = "Identification", order = 3, required = false)]
        public UTInt bundleVersionCode;

        [UTDoc(description = "Minium Android API level.", title = "Minimum API Level")]
        [UTInspectorHint(group = "Identification", order = 4, required = false)]
        public UTAndroidSdkVersions minimumApiLevel;

        // Configuration
       
        // This has been moved to UTSetGraphicsApiAction
#if UNITY_5_0  // VR: [5.0,5.0]
        [UTDoc(description = "Target graphics level.", title = "Graphics Level")]
        [UTInspectorHint(group = "Configuration", order = 1)]
        public UTTargetGlesGraphics targetGlesGraphics;
#endif

        [UTDoc(description = "The target device to build for.", title = "Device Filter")]
        [UTInspectorHint(group = "Configuration", order = 21)]
        public UTAndroidTargetDevice targetDevice;


        [UTDoc(description = "Preferred install location.")]
        [UTInspectorHint(group = "Configuration", order = 22)]
        public UTAndroidPreferredInstallLocation installLocation;


        [UTDoc(description = "Force internet permission?")]
        [UTInspectorHint(group = "Configuration", order = 23)]
        public UTBool forceInternetPermission;

        [UTInspectorHint(group = "Configuration", order = 24)]
        [UTDoc(description = "Force SD Card write permission?")]
        public UTBool forceSDCardPermission;

        [UTDoc(description = "Enable Android TV compatibility?", title = "Android TV Compatibility")]
        [UTInspectorHint(group = "Configuration", order = 25)]
        public UTBool androidTvCompatibility;

        [UTDoc(description = "Android Game?")]
        [UTInspectorHint(group = "Configuration", order = 26)]
        public UTBool androidGame;

#if !UNITY_5_0 && !UNITY_5_1_0 // VR: 5.1.1
        [UTDoc(description = "Which type of gamepads are supported?")]
        [UTInspectorHint(group = "Configuration", order = 27)]
        public UTAndroidGamepadSupportLevel androidGamepadSupport;
#endif   

 
        [UTDoc(description = "Should the internal android profiler be enabled?")]
        [UTInspectorHint(group = "Optimization", order = 6)]
        public UTBool enableInternalProfiler;

     

        // Publishing settings
        [UTDoc(description = "Path to the keystore to use.")]
        [UTInspectorHint(group = "Publishing Settings", order = 1, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect)]
        public UTString keyStore;

        [UTDoc(description = "Password for the key store.")]
        [UTInspectorHint(group = "Publishing Settings", order = 2, displayAs = UTInspectorHint.DisplayAs.Password)]
        public UTString keyStorePassword;

        [UTDoc(description = "Alias of the key to use")]
        [UTInspectorHint(group = "Publishing Settings", order = 3)]
        public UTString keyAlias;

        [UTDoc(description = "Password for the key to use.")]
        [UTInspectorHint(group = "Publishing Settings", order = 4, displayAs = UTInspectorHint.DisplayAs.Password)]
        public UTString keyPassword;

        // Unity 4.2 +
        [UTDoc(description = "Split application into .apk and .obb file.")]
        [UTInspectorHint(group = "Publishing Settings", order = 5)]
        public UTBool splitApplicationBinary;

        public override IEnumerator Execute(UTContext context)
        {
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying Android player settings.", this);
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

            var theKeyStore = keyStore.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theKeyStore) && !File.Exists(theKeyStore))
            {
                throw new UTFailBuildException("The specified keystore does not exist.", this);
            }

            PlayerSettings.defaultInterfaceOrientation = defaultOrientation.EvaluateIn(context);
            PlayerSettings.statusBarHidden = statusBarHidden.EvaluateIn(context);
            PlayerSettings.use32BitDisplayBuffer = use32BitDisplayBuffer.EvaluateIn(context);
            PlayerSettings.Android.disableDepthAndStencilBuffers = use24BitDepthBuffer.EvaluateIn(context);
            PlayerSettings.Android.showActivityIndicatorOnLoading = showActivityIndicatorOnLoading.EvaluateIn(context);

            if (overrideIconForAndroid.EvaluateIn(context))
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[]
                {
                    iconSize192.EvaluateIn(context),
                    iconSize144.EvaluateIn(context),
                    iconSize96.EvaluateIn(context),
                    iconSize72.EvaluateIn(context),
                    iconSize48.EvaluateIn(context),
                    iconSize36.EvaluateIn(context),
                });
            }
            else
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new Texture2D[0]);
            }

            PlayerSettings.Android.splashScreenScale = splashScaling.EvaluateIn(context);

           

            PlayerSettings.bundleIdentifier = theBundleIdentifier;
            PlayerSettings.bundleVersion = theBundleVersion;
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode.EvaluateIn(context);
            PlayerSettings.Android.minSdkVersion = minimumApiLevel.EvaluateIn(context);

            PlayerSettings.Android.targetDevice = targetDevice.EvaluateIn(context);

#if UNITY_5_0  // VR: [5.0,5.0]            
            PlayerSettings.targetGlesGraphics = targetGlesGraphics.EvaluateIn(context);
#endif            
            PlayerSettings.Android.preferredInstallLocation = installLocation.EvaluateIn(context);
            PlayerSettings.Android.forceInternetPermission = forceInternetPermission.EvaluateIn(context);
            PlayerSettings.Android.forceSDCardPermission = forceSDCardPermission.EvaluateIn(context);

#if !UNITY_5_0 // VR: 5.1            
            var isVirtualRealitySupported = virtualRealitySupported.EvaluateIn(context);

            if (isVirtualRealitySupported != PlayerSettings.virtualRealitySupported) {
                PlayerSettings.virtualRealitySupported = isVirtualRealitySupported;
#if UNITY_5_1 // VR: [5.1, 5.1]
                // this only seems to be needed in 5.1, it's no longer done in 5.2
                PlayerPrefs.SetString("vrmode", !PlayerSettings.virtualRealitySupported ? string.Empty : "oculus");
                UTInternalCall.InvokeStatic("UnityEditor.ShaderUtil", "RecreateGfxDevice"); 
#endif
            }
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            PlayerSettings.protectGraphicsMemory = protectGraphicsMemory.EvaluateIn(context);
#endif

            PlayerSettings.Android.keystoreName = theKeyStore;
            PlayerSettings.Android.keystorePass = keyStorePassword.EvaluateIn(context);

            PlayerSettings.Android.keyaliasName = keyAlias.EvaluateIn(context);
            PlayerSettings.Android.keyaliasPass = keyPassword.EvaluateIn(context);
            PlayerSettings.Android.useAPKExpansionFiles = splitApplicationBinary.EvaluateIn(context);


            using (var wrapper = new UTPlayerSettingsWrapper())
            {
#if UNITY_5_0 // VR: [5.0,5.0]                
                wrapper.SetObject("iPhoneSplashScreen", mobileSplashScreen.EvaluateIn(context));
#else 
                wrapper.SetObject("androidSplashScreen", mobileSplashScreen.EvaluateIn(context));
#endif
                wrapper.SetBool("androidEnableBanner", enableAndroidBanner.EvaluateIn(context));

                wrapper.SetBool("m_MobileMTRendering", multithreadedRendering.EvaluateIn(context));
                wrapper.SetBool("gpuSkinning", gpuSkinning.EvaluateIn(context));

                wrapper.SetBool("AndroidTVCompatibility", androidTvCompatibility.EvaluateIn(context));
                wrapper.SetBool("AndroidIsGame", androidGame.EvaluateIn(context));

#if !UNITY_5_0 && !UNITY_5_1_0 // VR: 5.1.1
                wrapper.SetEnum("androidGamepadSupportLevel", androidGamepadSupport.EvaluateIn(context));
#endif

                wrapper.SetBool("AndroidProfiler", enableInternalProfiler.EvaluateIn(context));
                ApplyCommonSettings(wrapper, context);
            }
            var banners = new Texture2D[1];
            banners[0] = banner320x180.EvaluateIn(context);
            UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings+Android", "SetAndroidBanners", new object[] { banners });

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Android player settings modified.", this);
            }


            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Build/Set Android Player Settings", false, 270)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsAndroidAction>();
            result.LoadSettings();
        }

        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {
            var wrapper = new UTPlayerSettingsWrapper();

            defaultOrientation.StaticValue = PlayerSettings.defaultInterfaceOrientation;

      
            statusBarHidden.StaticValue = PlayerSettings.statusBarHidden;
            use32BitDisplayBuffer.StaticValue = PlayerSettings.use32BitDisplayBuffer;
            use24BitDepthBuffer.StaticValue = PlayerSettings.Android.disableDepthAndStencilBuffers;
            showActivityIndicatorOnLoading.StaticValue = PlayerSettings.Android.showActivityIndicatorOnLoading;

            var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Android);

            if (icons != null && icons.Length > 0)
            {
                if (icons.Length == 6)
                {
                    iconSize192.StaticValue = icons[0];
                    iconSize144.StaticValue = icons[1];
                    iconSize96.StaticValue = icons[2];
                    iconSize72.StaticValue = icons[3];
                    iconSize48.StaticValue = icons[4];
                    iconSize36.StaticValue = icons[5];
                    overrideIconForAndroid.StaticValue = true;
                }
                else
                {
                    Debug.LogWarning("Amount of icon sizes for Android has changed (was " + icons.Length + " but should be 6). Please report this issue to support@ancientlightstudios.com. Thank you!");
                }
            }
            else
            {
                overrideIconForAndroid.StaticValue = false;
                iconSize192.StaticValue = null;
                iconSize144.StaticValue = null;
                iconSize96.StaticValue = null;
                iconSize72.StaticValue = null;
                iconSize48.StaticValue = null;
                iconSize36.StaticValue = null;
            }

            enableAndroidBanner.StaticValue = wrapper.GetBool("androidEnableBanner");

            // these are structs of type UnityEditor.AndroidBanner, which are sadly also internal, so..
            // this returns an ArrayOf UnityEditor.AndroidBanner, since we cannot cast it, we have to fall back to IEnumerable
            var androidBanners = (IEnumerable)UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings+Android", "GetAndroidBanners");
            var enumerator = androidBanners.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var banner320 = enumerator.Current;
                if (banner320 != null) {
                    banner320x180.StaticValue = UTInternalCall.GetField(banner320, "banner") as Texture2D;
                }
            }
            else
            {
                Debug.LogWarning("Amount of banner sizes for Android has changed (was " + icons.Length + " but should be 1). Please report this issue to support@ancientlightstudios.com. Thank you!");
            }

#if UNITY_5_0 // VR: [5.0, 5.0]
    mobileSplashScreen.StaticValue = wrapper.GetObject("iPhoneSplashScreen") as Texture2D;
#else
            mobileSplashScreen.StaticValue = wrapper.GetObject("androidSplashScreen") as Texture2D;
#endif
            splashScaling.StaticValue = PlayerSettings.Android.splashScreenScale;

            multithreadedRendering.StaticValue = wrapper.GetBool("m_MobileMTRendering");


            bundleIdentifier.StaticValue = PlayerSettings.bundleIdentifier;
            bundleVersion.StaticValue = PlayerSettings.bundleVersion;
            bundleVersionCode.StaticValue = PlayerSettings.Android.bundleVersionCode;
            minimumApiLevel.StaticValue = PlayerSettings.Android.minSdkVersion;
            targetDevice.StaticValue = PlayerSettings.Android.targetDevice;

#if UNITY_5_0  // VR: [5.0,5.0]
            targetGlesGraphics.StaticValue = PlayerSettings.targetGlesGraphics;
#endif

            installLocation.StaticValue = PlayerSettings.Android.preferredInstallLocation;
            forceInternetPermission.StaticValue = PlayerSettings.Android.forceInternetPermission;
            forceSDCardPermission.StaticValue = PlayerSettings.Android.forceSDCardPermission;
            androidTvCompatibility.StaticValue = wrapper.GetBool("AndroidTVCompatibility");
            androidGame.StaticValue = wrapper.GetBool("AndroidIsGame");
#if !UNITY_5_0 && !UNITY_5_1_0 // VR: 5.1.1
            androidGamepadSupport.StaticValue = wrapper.GetEnum<AndroidGamepadSupportLevel>("androidGamepadSupportLevel");
            virtualRealitySupported.StaticValue = PlayerSettings.virtualRealitySupported;
#endif
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            protectGraphicsMemory.StaticValue = PlayerSettings.protectGraphicsMemory;
#endif
            enableInternalProfiler.StaticValue = wrapper.GetBool("AndroidProfiler");

            keyStore.StaticValue = PlayerSettings.Android.keystoreName;
            keyStorePassword.StaticValue = PlayerSettings.Android.keystorePass;
            keyAlias.StaticValue = PlayerSettings.Android.keyaliasName;
            keyPassword.StaticValue = PlayerSettings.Android.keyaliasPass;
            splitApplicationBinary.StaticValue = PlayerSettings.Android.useAPKExpansionFiles;
            
            LoadCommonSettings(wrapper);
        }

        public string LoadSettingsUndoText
        {
            get
            {
                return "Load Android specific player settings";
            }
        }

        public override bool SupportsMutingAudio
        {
            get
            {
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
                return true;
#else
                return false;
#endif
            }
        }

        protected override bool IsMobilePlatform
        {
            get
            {
               return true;
            }
        }

        protected override BuildTarget Platform
        {
            get
            {
                return BuildTarget.Android;
            }
        }

        public override bool SupportsStripping
        {
            get { return true;  }
        }

        public override bool HasAutorotation
        {
            get { return true; }
        }
    }
}

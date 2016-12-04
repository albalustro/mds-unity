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
    using UnityEngine;
    using System.Collections;

    [UTDoc(title = "Set PC, Mac and Linux Standalone Player Settings", description = "Sets the player settings for PC, Mac and Linux standalone builds.")]
    [UTActionInfo(actionCategory = "Build")]
    [UTInspectorGroups(groups = new[] { "Resolution", "Standalone Player Options", "Aspect Ratios", "Icon", "Splash Image", "Identification", "Rendering", "Configuration", "Optimization" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsPcMacAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        // ----------- RESOLUTION ------------
        [UTDoc(description = "If enabled, the game will default to fullscreen mode.")]
        [UTInspectorHint(group = "Resolution", order = 1)]
        public UTBool defaultIsFullscreen;

        [UTDoc(description = "Default screen width of the standalone player window.")]
        [UTInspectorHint(group = "Resolution", order = 2)]
        public UTInt defaultScreenWidth;

        [UTDoc(description = "Default screen height of the standalone player window.")]
        [UTInspectorHint(group = "Resolution", order = 3)]
        public UTInt defaultScreenHeight;

        [UTDoc(description = "Continue running when application loses focus?")]
        [UTInspectorHint(group = "Resolution", order = 4)]
        public UTBool runInBackground;

        // ----------- STANDALONE PLAYER OPTIONS ------------

        [UTDoc(description = "Defines if fullscreen games should darken secondary display.")]
        [UTInspectorHint(group = "Standalone Player Options", order = 5)]
        public UTBool captureSingleScreen;

        [UTDoc(description = "Behaviour of the resolution dialog on game launch.")]
        [UTInspectorHint(group = "Standalone Player Options", order = 6)]
        public UTResolutionDialogSetting resolutionDialog;

        [UTDoc(description = "Write a debug file with logging information?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 7)]
        public UTBool usePlayerLog;

        [UTDoc(description = "Should the window be resizable?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 8)]

        public UTBool resizableWindow;

        [UTDoc(description = "Enable Mac App Store validation?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 9)]
        public UTBool macAppStoreValidation;

        [UTDoc(description = "Fullscreen mode to use on Macs.")]
        [UTInspectorHint(group = "Standalone Player Options", order = 10)]
        public UTMacFullscreenMode macFullscreenMode;

        [UTDoc(description = "Fullscreen mode to use on DirectX 9.", title = "D3D9 Fullscreen Mode")]
        [UTInspectorHint(group = "Standalone Player Options", order = 11)]
        public UTD3D9FullscreenMode d3d9FullscreenMode;

        [UTDoc(description = "Fullscreen mode to use on DirectX 11.", title = "D3D11 Fullscreen Mode")]
        [UTInspectorHint(group = "Standalone Player Options", order = 12)]
        public UTD3D11FullscreenMode d3d11FullscreenMode;

        [UTDoc(description = "Should the app be visible in background?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 13)]
        public UTBool visibleInBackground;

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        [UTDoc(description = "Allow full screen switch?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 14)]
        public UTBool allowFullscreenSwitch;
#endif

        [UTDoc(description = "Can only be a single instance of the app be opened?")]
        [UTInspectorHint(group = "Standalone Player Options", order = 15)]
        public UTBool forceSingleInstance;
        // --------------   ASPECT RATIOS ------------------
        [UTDoc(description = "Support 4:3 aspect ratio", title = "4:3")]
        [UTInspectorHint(group = "Aspect Ratios", order = 1)]
        public UTBool support4by3;

        [UTDoc(description = "Support 5:4 aspect ratio", title = "5:4")]
        [UTInspectorHint(group = "Aspect Ratios", order = 2)]
        public UTBool support5by4;

        [UTDoc(description = "Support 16:10 aspect ratio", title = "16:10")]
        [UTInspectorHint(group = "Aspect Ratios", order = 3)]
        public UTBool support16by10;

        [UTDoc(description = "Support 16:9 aspect ratio", title = "16:9")]
        [UTInspectorHint(group = "Aspect Ratios", order = 4)]
        public UTBool support16by9;

        [UTDoc(description = "Support other aspect ratios", title = "Other")]
        [UTInspectorHint(group = "Aspect Ratios", order = 5)]
        public UTBool supportOther;

        // ------------------ ICON -----------------
        [UTInspectorHint(group = "Icon", order = 1)]
        [UTDoc(description = "Should a different icon be used for iPhone?", title = "Override for Standalone")]
        public UTBool overrideIconForStandalone;

        [UTInspectorHint(group = "Icon", order = 2)]
        [UTDoc(description = "Icon for 1024x1024 pixels.", title = "1024x1024")]
        public UTTexture2D iconSize1024;

        [UTInspectorHint(group = "Icon", order = 3)]
        [UTDoc(description = "Icon for 512x512 pixels.", title = "512x512")]
        public UTTexture2D iconSize512;

        [UTInspectorHint(group = "Icon", order = 4)]
        [UTDoc(description = "Icon for 256x256 pixels.", title = "256x256")]
        public UTTexture2D iconSize256;

        [UTInspectorHint(group = "Icon", order = 5)]
        [UTDoc(description = "Icon for 128x128 pixels.", title = "128x128")]
        public UTTexture2D iconSize128;

        [UTInspectorHint(group = "Icon", order = 6)]
        [UTDoc(description = "Icon for 48x48 pixels.", title = "48x48")]
        public UTTexture2D iconSize48;

        [UTInspectorHint(group = "Icon", order = 7)]
        [UTDoc(description = "Icon for 32x32 pixels.", title = "32x32")]
        public UTTexture2D iconSize32;

        [UTInspectorHint(group = "Icon", order = 8)]
        [UTDoc(description = "Icon for 16x16 pixels.", title = "16x16")]
        public UTTexture2D iconSize16;



        // ------------- SPLASH SCREEN ------------
        [UTDoc(description = "The splash image for the resolution selection dialog.", title = "Config Dialog Banner")]
        [UTInspectorHint(group = "Splash Image", order = 5)]
        public UTTexture2D splashImage;

        // -------------- RENDERING ---------------

        [UTDoc(description = "The color space for lightmaps.")]
        [UTInspectorHint(group = "Rendering", order = 2)]
        public UTColorSpace colorSpace;

#if UNITY_5_0 || UNITY_5_1_0 // VR: [5.0, 5.1.0]
        [UTDoc(description = "Use Direct3D 11?", title = "Use Direct3D 11")]
        [UTInspectorHint(group = "Rendering", order = 3)]
        public UTBool useDirect3D11;
#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
        [UTDoc(description = "Enable stereoscopic rendering?")]
        [UTInspectorHint(group = "Rendering", order = 10)]
        public UTBool stereoscopicRendering;
#endif

#if !UNITY_5_0 // VR: 5.1    
        [UTDoc(description="Does the game support virtual reality headsets?")]
        [UTInspectorHint(group = "Rendering", order = 11)]        
        public UTBool virtualRealitySupported; 
#endif 

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4 
        [UTDoc(description = "Enable single-pass stereoscopic rendering?")]
        [UTInspectorHint(group = "Rendering", order = 12)]
        public UTBool singlePassStereoscopicRendering;
#endif
       

        public override IEnumerator Execute(UTContext context)
        {
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying PC/Mac player settings.", this);
            }

            PlayerSettings.defaultScreenWidth = defaultScreenWidth.EvaluateIn(context);
            PlayerSettings.defaultScreenHeight = defaultScreenHeight.EvaluateIn(context);
            PlayerSettings.runInBackground = runInBackground.EvaluateIn(context);
            PlayerSettings.defaultIsFullScreen = defaultIsFullscreen.EvaluateIn(context);
            PlayerSettings.captureSingleScreen = captureSingleScreen.EvaluateIn(context);
            PlayerSettings.displayResolutionDialog = resolutionDialog.EvaluateIn(context);
            PlayerSettings.usePlayerLog = usePlayerLog.EvaluateIn(context);
            PlayerSettings.useMacAppStoreValidation = macAppStoreValidation.EvaluateIn(context);
            PlayerSettings.macFullscreenMode = macFullscreenMode.EvaluateIn(context);

            PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, support4by3.EvaluateIn(context));
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, support5by4.EvaluateIn(context));
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, support16by10.EvaluateIn(context));
            PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, support16by9.EvaluateIn(context));
            PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, supportOther.EvaluateIn(context));

            if (overrideIconForStandalone.EvaluateIn(context))
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, new[]
                {
                    iconSize1024.EvaluateIn(context),
                    iconSize512.EvaluateIn(context),
                    iconSize256.EvaluateIn(context),
                    iconSize128.EvaluateIn(context),
                    iconSize48.EvaluateIn(context),
                    iconSize32.EvaluateIn(context),
                    iconSize16.EvaluateIn(context),
                });
            }
            else
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, new Texture2D[0]);
            }

            PlayerSettings.resolutionDialogBanner = splashImage.EvaluateIn(context);

            PlayerSettings.colorSpace = colorSpace.EvaluateIn(context);

#if UNITY_5_0  // VR: [5.0, 5.0]
            PlayerSettings.useDirect3D11 = useDirect3D11.EvaluateIn(context);
#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
            PlayerSettings.stereoscopic3D = stereoscopicRendering.EvaluateIn(context);
#endif

#if !UNITY_5_0 // VR: 5.1            
            var isVirtualRealitySupported = virtualRealitySupported.EvaluateIn(context);

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (isVirtualRealitySupported != PlayerSettings.virtualRealitySupported) {
                PlayerSettings.virtualRealitySupported = isVirtualRealitySupported;
#if UNITY_5_1 // VR: [5.1, 5.1]
                // this only seems to be needed in 5.1, it's no longer done in 5.2
                PlayerPrefs.SetString("vrmode", !PlayerSettings.virtualRealitySupported ? string.Empty : "oculus");
                UTInternalCall.InvokeStatic("UnityEditor.ShaderUtil", "RecreateGfxDevice"); 
#endif
            }

#if  !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            var doSinglePassStereoscopicRendering = singlePassStereoscopicRendering.EvaluateIn(context);
            // single-pass stereo rendering must only be enabled when vr is enabled.
            PlayerSettings.singlePassStereoRendering = doSinglePassStereoscopicRendering && isVirtualRealitySupported;
#endif
            
#endif 
            using (var wrapper = new UTPlayerSettingsWrapper())
            {

                wrapper.SetBool("resizableWindow", resizableWindow.EvaluateIn(context));
                wrapper.SetEnum("d3d9FullscreenMode", d3d9FullscreenMode.EvaluateIn(context));
                wrapper.SetEnum("d3d11FullscreenMode", d3d11FullscreenMode.EvaluateIn(context));
                wrapper.SetBool("visibleInBackground", visibleInBackground.EvaluateIn(context));
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 ) // VR: 5.3
                wrapper.SetBool("allowFullscreenSwitch", allowFullscreenSwitch.EvaluateIn(context));
#endif
                wrapper.SetBool("forceSingleInstance", forceSingleInstance.EvaluateIn(context));

                ApplyCommonSettings(wrapper, context);
            }

            if (UTPreferences.DebugMode)
            {
                Debug.Log("PC/Mac player settings modified.", this);
            }

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Build/Set PC, Mac and Linux Standalone Player Settings", false, 240)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsPcMacAction>();
            result.LoadSettings();
        }

        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {
            var wrapper = new UTPlayerSettingsWrapper();


            defaultScreenWidth.StaticValue = PlayerSettings.defaultScreenWidth;
            defaultScreenHeight.StaticValue = PlayerSettings.defaultScreenHeight;
            runInBackground.StaticValue = PlayerSettings.runInBackground;
            defaultIsFullscreen.StaticValue = PlayerSettings.defaultIsFullScreen;

            captureSingleScreen.StaticValue = PlayerSettings.captureSingleScreen;
            resolutionDialog.StaticValue = PlayerSettings.displayResolutionDialog;
            usePlayerLog.StaticValue = PlayerSettings.usePlayerLog;
            resizableWindow.StaticValue = wrapper.GetBool("resizableWindow");

            macAppStoreValidation.StaticValue = PlayerSettings.useMacAppStoreValidation;
            macFullscreenMode.StaticValue = PlayerSettings.macFullscreenMode;
            d3d9FullscreenMode.StaticValue = wrapper.GetEnum<D3D9FullscreenMode>("d3d9FullscreenMode");
            d3d11FullscreenMode.StaticValue = wrapper.GetEnum<D3D11FullscreenMode>("d3d11FullscreenMode");
            visibleInBackground.StaticValue = wrapper.GetBool("visibleInBackground");
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 ) // VR: 5.3
            allowFullscreenSwitch.StaticValue = wrapper.GetBool("allowFullscreenSwitch");
#endif
            forceSingleInstance.StaticValue = wrapper.GetBool("forceSingleInstance");

            support4by3.StaticValue = PlayerSettings.HasAspectRatio(AspectRatio.Aspect4by3);
            support5by4.StaticValue = PlayerSettings.HasAspectRatio(AspectRatio.Aspect5by4);
            support16by10.StaticValue = PlayerSettings.HasAspectRatio(AspectRatio.Aspect16by10);
            support16by9.StaticValue = PlayerSettings.HasAspectRatio(AspectRatio.Aspect16by9);
            supportOther.StaticValue = PlayerSettings.HasAspectRatio(AspectRatio.AspectOthers);
            var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Standalone);

            if (icons != null && icons.Length > 0)
            {
                if (icons.Length == 7)
                {
                    iconSize1024.StaticValue = icons[0];
                    iconSize512.StaticValue = icons[1];
                    iconSize256.StaticValue = icons[2];
                    iconSize128.StaticValue = icons[3];
                    iconSize48.StaticValue = icons[4];
                    iconSize32.StaticValue = icons[5];
                    iconSize16.StaticValue = icons[6];
                    overrideIconForStandalone.StaticValue = true;
                }
                else
                {
                    Debug.LogWarning("Amount of icon sizes for Standalone has changed (was " + icons.Length + " but should be 7). Please report this issue to support@ancientlightstudios.com. Thank you!");
                }
            }
            else
            {
                iconSize1024.StaticValue = null;
                iconSize512.StaticValue = null;
                iconSize256.StaticValue = null;
                iconSize128.StaticValue = null;
                iconSize48.StaticValue = null;
                iconSize32.StaticValue = null;
                iconSize16.StaticValue = null;
                overrideIconForStandalone.StaticValue = false;
            }
             
            splashImage.StaticValue = PlayerSettings.resolutionDialogBanner;


            colorSpace.StaticValue = PlayerSettings.colorSpace;

#if UNITY_5_0  // VR: [5.0, 5.0]
            useDirect3D11.StaticValue = PlayerSettings.useDirect3D11;
#endif

          
#if  UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0,5.3]
            stereoscopicRendering.StaticValue = PlayerSettings.stereoscopic3D;
#endif

#if  !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            singlePassStereoscopicRendering.StaticValue = PlayerSettings.singlePassStereoRendering;
#endif
#if !UNITY_5_0 // VR: 5.1
            virtualRealitySupported.StaticValue = PlayerSettings.virtualRealitySupported;
#endif
            LoadCommonSettings(wrapper);
        }

        public string LoadSettingsUndoText
        {
            get { return "Load PC/Mac/Linux specific player settings"; }
        }
        
           protected override bool IsMobilePlatform
        {
            get
            {
               return false;
            }
        }

        protected override BuildTarget Platform
        {
            get
            {
                return BuildTarget.StandaloneWindows;
            }
        }
    }
}

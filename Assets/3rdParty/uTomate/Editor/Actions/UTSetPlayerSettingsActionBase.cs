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

    public abstract class UTSetPlayerSettingsActionBase : UTAction
    {
        // --------- PRESENTATION -------------
        [UTDoc(description = "Default screen orientation for mobiles.")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 1)]
        public UTUIOrientation defaultOrientation;

        [UTDoc(description = "Allow auto-rotation into portrait mode?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 3, indentLevel = 1)]
        public UTBool allowPortrait;


        [UTDoc(description = "Allow auto-rotation into portrait mode?", title = "Allow Portrait Upside-Down")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 4, indentLevel = 1)]
        public UTBool allowPortraitUpsideDown;

        [UTDoc(description = "Allow auto-rotation into landscape right mode?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 5, indentLevel = 1)]
        public UTBool allowLandscapeRight;

        [UTDoc(description = "Allow auto-rotation into landscape left mode?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 6, indentLevel = 1)]
        public UTBool allowLandscapeLeft;

        // --------- SPLASH IMAGE -------------

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        [UTDoc(description = "The splash image for virtual reality titles.", title = "VR Splash Image")]
        [UTInspectorHint(group = "Splash Image", order = 2)]
        public UTTexture2D virtualRealitySplashImage;
#endif

#if !UNITY_5_0 // VR: 5.1
        [UTInspectorHint(group = "Splash Image", order = 3)]
        [UTDoc(description = "Show the Unity splash screen?")]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTBool showUnitySplashScreen;
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
        [UTDoc(description = "Style of splash screen to use.")]
        [UTInspectorHint(group = "Splash Image", order = 4)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTSplashScreenStyle splashScreenStyle;
#endif    

        // --------- RENDERING ----------------
        [UTDoc(description = "The rendering path to use.")]
        [UTInspectorHint(group = "Rendering", order = 1,
            captions = new[] { "Legacy Vertex Lit", "Forward", "Legacy Deferred (light prepass)", "Deferred" },
            allowedValues = new[] { "VertexLit", "Forward", "DeferredLighting", "DeferredShading" })]
        public UTRenderingPath renderingPath;

        [UTDoc(description = "Enable static batching?")]
        [UTInspectorHint(group = "Rendering", order = 5)]
        public UTBool staticBatching;

        [UTDoc(description = "Enable dynamic batching?")]
        [UTInspectorHint(group = "Rendering", order = 6)]
        public UTBool dynamicBatching;

        [UTDoc(description = "Enable GPU skinning?", title = "GPU skinning")]
        [UTInspectorHint(group = "Rendering", order = 7)]
        public UTBool gpuSkinning;



        // ------------ CONFIGURATION --------------
        [UTDoc(description = "Don't send hardware statistics to Unity.", title = "Disable Analytics")]
        [UTInspectorHint(group = "Configuration", order = 20)]
        [UTRequiresLicense(UTLicense.UnityPro)]
        public UTBool disableHardwareStatistics;

        // ------------ OPTIMIZATION ---------------
        [UTDoc(description = ".NET API compatibility level.")]
        [UTInspectorHint(group = "Optimization", order = 1)]
        public UTApiCompatibilityLevel apiCompatibilityLevel;

        [UTDoc(description = "Should collision meshes be pre-baked?")]
        [UTInspectorHint(group = "Optimization", order = 2)]
        public UTBool prebakeCollisionMeshes;


        [UTDoc(description = "Should shaders be pre-loaded?")]
        [UTInspectorHint(group = "Optimization", order = 3)]
        public UTBool preloadShaders;

        [UTDoc(description = "Assets that should be pre-loaded.")]
        [UTInspectorHint(group = "Optimization", order = 4)]
        public UTUnityObject[] preloadedAssets;
#if !(UNITY_5_0 || UNITY_5_1) // VR: 5.2            
        [UTDoc(description = "Strip unused engine code. Note that byte code stripping of managed assemblies is always enabled for the IL2CPP scripting backend.")]
        [UTInspectorHint(group="Optimization", order=5)]
        public UTBool stripEngineCode;
#endif

        [UTDoc(description = "Stripping level")]
        [UTInspectorHint(group = "Optimization", order = 5)]
        public UTStrippingLevel strippingLevel;

#if !UNITY_5_0 && !UNITY_5_1 // VR: 5.2
        [UTDoc(description = "Vertex compression flags.")]
        [UTInspectorHint(group = "Optimization", order = 20, multiSelect = true)]
        public UTVertexChannelCompressionFlags vertexCompression;
#endif

        [UTDoc(description = "Should unused Mesh components be excluded from game build?")]
        [UTInspectorHint(group = "Optimization", order = 21)]
        public UTBool optimizeMeshData;


        protected void ApplyCommonSettings(UTPlayerSettingsWrapper wrapper, UTContext context)
        {

            // ----------- RESOLUTION & PRESENTATION ----------

            if (HasAutorotation)
            {
                PlayerSettings.defaultInterfaceOrientation = defaultOrientation.EvaluateIn(context);
                wrapper.SetBool("allowedAutorotateToPortrait", allowPortrait.EvaluateIn(context));
                wrapper.SetBool("allowedAutorotateToPortraitUpsideDown", allowPortraitUpsideDown.EvaluateIn(context));
                wrapper.SetBool("allowedAutorotateToLandscapeRight", allowLandscapeRight.EvaluateIn(context));
                wrapper.SetBool("allowedAutorotateToLandscapeLeft", allowLandscapeLeft.EvaluateIn(context));
            }

            // ------------- SPLASH SCREEN ------------
            if (HasSplashScreen)
            {
#if !UNITY_5_0 // VR: 5.1
                if (UTils.IsUnityPro) // Keep in line with Unity's licensing terms.
                {
                    wrapper.SetBool("m_ShowUnitySplashScreen", showUnitySplashScreen.EvaluateIn(context));
                }
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
                if (UTils.IsUnityPro) // Keep in line with Unity's licensing terms.
                {
                    wrapper.SetEnum("m_SplashScreenStyle", splashScreenStyle.EvaluateIn(context));
                }
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                if (SupportsVirtualReality)
                {
                    wrapper.SetObject("m_VirtualRealitySplashScreen", virtualRealitySplashImage.EvaluateIn(context));
                }
#endif
            }
            // ------------ RENDERING -------------
            if (IsMobilePlatform)
            {
                wrapper.SetEnum("m_MobileRenderingPath", renderingPath.EvaluateIn(context));
            }
            else
            {
                PlayerSettings.renderingPath = renderingPath.EvaluateIn(context);
            }
            UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings", "SetBatchingForPlatform", Platform,
            staticBatching.EvaluateIn(context) ? 1 : 0, dynamicBatching.EvaluateIn(context) ? 1 : 0);

            wrapper.SetBool("gpuSkinning", gpuSkinning.EvaluateIn(context));

            // ------------ CONFIGURATION ---------------
            // we only set this if the licenses permits it, so we don't get any trouble with Unity.
            if (UTils.HasAdvancedLicenseOn(Platform))
            {
                wrapper.SetBool("submitAnalytics", !disableHardwareStatistics.EvaluateIn(context));
            }


            // ------------ OPTIMIZATION -------------

            PlayerSettings.apiCompatibilityLevel = apiCompatibilityLevel.EvaluateIn(context);
            wrapper.SetBool("StripUnusedMeshComponents",  optimizeMeshData.EvaluateIn(context));

            wrapper.SetBool("preloadShaders", preloadShaders.EvaluateIn(context));
            wrapper.SetBool("bakeCollisionMeshes", prebakeCollisionMeshes.EvaluateIn(context));
            // ReSharper disable once CoVariantArrayConversion
            wrapper.SetArray("preloadedAssets", EvaluateAll(preloadedAssets, context));

            if (SupportsStripping)
            {
                wrapper.SetInt("iPhoneStrippingLevel", (int)strippingLevel.EvaluateIn(context));
            }

#if !(UNITY_5_0 || UNITY_5_1) // VR: 5.2            
            wrapper.SetBool("stripEngineCode", stripEngineCode.EvaluateIn(context));
#endif
#if  !UNITY_5_0 && !UNITY_5_1 // VR: 5.2
            wrapper.SetEnum("VertexChannelCompressionMask", vertexCompression.EvaluateIn(context));
#endif
        }


        protected void LoadCommonSettings(UTPlayerSettingsWrapper wrapper)
        {

            // -------------  PRESENTATION & RESOLUTION -----------
            if (HasAutorotation)
            {
                defaultOrientation.StaticValue = PlayerSettings.defaultInterfaceOrientation;
                allowPortrait.StaticValue = wrapper.GetBool("allowedAutorotateToPortrait");
                allowPortraitUpsideDown.StaticValue = wrapper.GetBool("allowedAutorotateToPortraitUpsideDown");
                allowLandscapeRight.StaticValue = wrapper.GetBool("allowedAutorotateToLandscapeRight");
                allowLandscapeLeft.StaticValue = wrapper.GetBool("allowedAutorotateToLandscapeLeft");
            }

            // --------------- SPLASH SCREEN -----------
            if (HasSplashScreen)
            {
#if !UNITY_5_0 // VR: 5.1
                showUnitySplashScreen.StaticValue = wrapper.GetBool("m_ShowUnitySplashScreen");
#endif

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
                splashScreenStyle.StaticValue = wrapper.GetEnum<SplashScreenStyle>("m_SplashScreenStyle");
#endif
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
                if (SupportsVirtualReality)
                {
                    virtualRealitySplashImage.StaticValue = (Texture2D) wrapper.GetObject("m_VirtualRealitySplashScreen");
                }
#endif
            }

            // -------------- RENDERING ----------------
            if (IsMobilePlatform)
            {
                renderingPath.StaticValue = wrapper.GetEnum<RenderingPath>("m_MobileRenderingPath");
            }
            else
            {
                renderingPath.StaticValue = PlayerSettings.renderingPath;
            }
            var parameters = new object[3];
            parameters[0] = Platform;

            // get the current batching settings. This is an out parameter call, therefore we need that array
            UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings", "GetBatchingForPlatform", parameters);
            var existingStaticBatching = (int)parameters[1];
            var existingDynamicBatching = (int)parameters[2];
            staticBatching.StaticValue = existingStaticBatching == 1;
            dynamicBatching.StaticValue = existingDynamicBatching == 1;
            gpuSkinning.StaticValue = wrapper.GetBool("gpuSkinning");

            // ------------ CONFIGURATION ---------------

            disableHardwareStatistics.StaticValue = !wrapper.GetBool("submitAnalytics");

            // ----------- OPTIMIZATION -----------------

            apiCompatibilityLevel.StaticValue = PlayerSettings.apiCompatibilityLevel;
            optimizeMeshData.StaticValue = wrapper.GetBool("StripUnusedMeshComponents");
            preloadShaders.StaticValue = wrapper.GetBool("preloadShaders");
#if !(UNITY_5_0 || UNITY_5_1) // VR: 5.2            
            stripEngineCode.StaticValue = wrapper.GetBool("stripEngineCode");
#endif            
            prebakeCollisionMeshes.StaticValue = wrapper.GetBool("bakeCollisionMeshes");


            var array = wrapper.GetArray("preloadedAssets");
            preloadedAssets = new UTUnityObject[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                preloadedAssets[i] = new UTUnityObject { StaticValue = array[i] };
            }

            if (SupportsStripping)
            {
                strippingLevel.StaticValue = (StrippingLevel)wrapper.GetInt("iPhoneStrippingLevel");
            }
#if !UNITY_5_0 && !UNITY_5_1 // VR: 5.2
            vertexCompression.StaticValue = wrapper.GetEnum<VertexChannelCompressionFlags>("VertexChannelCompressionMask");
#endif
        }

        protected abstract bool IsMobilePlatform { get; }

        protected abstract BuildTarget Platform { get; }

        public virtual bool SupportsStripping
        {
            get { return false; }
        }

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        public virtual bool SupportsVirtualReality 
        {
            get { return true; }
        }
#endif

        public virtual bool HasSplashScreen
        {
            get { return true; }
        }

        public virtual bool HasAutorotation
        {
            get { return false; }
        }
    }

}

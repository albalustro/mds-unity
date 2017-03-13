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
    using System;

    [UTActionInfo(actionCategory = "Build")]
    [UTDoc(title = "Set Web Player Settings", description = "Sets the player settings for Web player builds."
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 ) // VR: 5.4
            , notice = "The web player has been removed in Unity 5.4. This action will do nothing in Unity 5.4 or later."
#endif
     )]
    [UTInspectorGroups(groups = new[] { "Resolution & Presentation", "Rendering", "Streaming", "Configuration", "Optimization" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsWebAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        [UTDoc(description = "Default screen width of the web player window.")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 1)]
        public UTInt defaultScreenWidth;

        [UTDoc(description = "Default screen height of the web player window.")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 2)]
        public UTInt defaultScreenHeight;

        [UTDoc(description = "Continue running when application loses focus?")]
        [UTInspectorHint(group = "Resolution & Presentation", order = 3)]
        public UTBool runInBackground;

        // -------------- RENDERING ---------------
        [UTDoc(description = "The color space for lightmaps.")] 
        [UTInspectorHint(group = "Rendering", order = 2)]
        public UTColorSpace colorSpace;

#if UNITY_5_0 // VR: [5.0, 5.0]
        [UTDoc(description = "Use Direct3D 11?", title = "Use Direct3D 11")]
        [UTInspectorHint(group = "Rendering", order = 3)]
        public UTBool useDirect3D11;
#endif

        //
        [UTDoc(description = "First Streamed Level")]
        [UTInspectorHint(group = "Streaming", order = 1)]
        public UTInt firstStreamedLevel;

        public override IEnumerator Execute(UTContext context)
        {
 
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 ) // VR: 5.4
            Debug.LogWarning("Web Player has been removed from Unity 5.4. This action will do nothing.", this);
            yield break;          
#else            
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying Web player settings.", this);
            }

            PlayerSettings.defaultWebScreenWidth = defaultScreenWidth.EvaluateIn(context);
            PlayerSettings.defaultWebScreenHeight = defaultScreenHeight.EvaluateIn(context);
            PlayerSettings.runInBackground = runInBackground.EvaluateIn(context);

            PlayerSettings.renderingPath = renderingPath.EvaluateIn(context);
            PlayerSettings.colorSpace = colorSpace.EvaluateIn(context);

#if UNITY_5_0  // VR: [5.0, 5.0]
            PlayerSettings.useDirect3D11 = useDirect3D11.EvaluateIn(context);
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR: [5.0, 5.2]
            PlayerSettings.firstStreamedLevelWithResources = firstStreamedLevel.EvaluateIn(context);
#endif
            using(var wrapper = new UTPlayerSettingsWrapper()) {
                ApplyCommonSettings(wrapper, context);
            }

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Web player settings modified.", this);
            }

            yield return "";
#endif
        }

        [MenuItem("Assets/Create/uTomate/Build/Set Web Player Settings", false, 260)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsWebAction>();
            result.LoadSettings();
        }

        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {
            #if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
            Debug.LogWarning("The web player has been removed in Unity 5.4. Settings were not loaded.");
            #else
            var wrapper = new UTPlayerSettingsWrapper();
            defaultScreenWidth.StaticValue = PlayerSettings.defaultWebScreenWidth;
            defaultScreenHeight.StaticValue = PlayerSettings.defaultWebScreenHeight;
            runInBackground.StaticValue = PlayerSettings.runInBackground;

            renderingPath.StaticValue = PlayerSettings.renderingPath;
            colorSpace.StaticValue = PlayerSettings.colorSpace;

#if UNITY_5_0  // VR: [5.0, 5.0]
            useDirect3D11.StaticValue = PlayerSettings.useDirect3D11;
            useDirect3D11.UseExpression = false;
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR: [5.0, 5.2]
            firstStreamedLevel.StaticValue = PlayerSettings.firstStreamedLevelWithResources;
#endif
            LoadCommonSettings(wrapper);
            #endif
        }

        public string LoadSettingsUndoText
        {
            get
            {
                return "Load Web player settings";
            }
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
                #if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
                return BuildTarget.WebPlayer;
                #else
                throw new InvalidOperationException("This should not be called in Unity 5.4 or later.");
                #endif
            }
        }

        public override bool HasSplashScreen
        {
            get { return false; }
        }
    }
}

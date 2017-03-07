namespace AncientLightStudios.uTomate
{
    using System.Collections;
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Build", sinceUTomateVersion = "1.8.0")]
    [UTDoc(title = "Set WebGL Player Settings", description = "Sets the player settings for WebGL player builds.")]
    [UTInspectorGroups(groups = new[] { "Resolution & Presentation", "Splash Image", "Rendering", "Configuration", "Optimization", "Publishing Settings" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsWebGLAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        [UTDoc(description = "Default screen width of the web player window.")]
        [UTInspectorHint(@group = "Resolution & Presentation", order = 1)]
        public UTInt defaultScreenWidth;

        [UTDoc(description = "Default screen height of the web player window.")]
        [UTInspectorHint(@group = "Resolution & Presentation", order = 2)]
        public UTInt defaultScreenHeight;

        [UTDoc(description = "Continue running when application loses focus?")]
        [UTInspectorHint(@group = "Resolution & Presentation", order = 3)]
        public UTBool runInBackground;


        // ----------------- PUBLISHING SETTINGS -----------------------------------------
        [UTDoc(description = "Lets you specifiy how much memory (in MB) the content should allocate for its heap. If this value is too low, you will get �out of memory� errors when your loaded content and scenes would not fit into the available memory. However if you set this value too high, your content might fail to load in some browsers or on some machine.")]
        [UTInspectorHint(@group = "Publishing settings", order = 1)]
        public UTInt memorySize;

        [UTDoc(description = "Lets you enable exception support in WebGL. If you don�t need any exception support, set this to �None�, which will give you the best performance and smallest builds.")]
        [UTInspectorHint(@group = "Publishing settings", order = 2)]
        public UTWebGLExceptionSupport enableExceptions;

        [UTDoc(description = "Lets you enable automatic local caching of your player data. If this is enabled, your assets will be stored to a local cached in the browsers IndexedDB database, so that they won�t have to be re-downloaded in subsequent runs of your content.")]
        [UTInspectorHint(@group = "Publishing settings", order = 3)]
        public UTBool dataCaching;

        public override IEnumerator Execute(UTContext context)
        {
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying WebGL player settings.", this);
            }

            PlayerSettings.defaultWebScreenWidth = defaultScreenWidth.EvaluateIn(context);
            PlayerSettings.defaultWebScreenHeight = defaultScreenHeight.EvaluateIn(context);
            PlayerSettings.runInBackground = runInBackground.EvaluateIn(context);

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            PlayerSettings.WebGL.memorySize = memorySize.EvaluateIn(context);
            PlayerSettings.WebGL.exceptionSupport = enableExceptions.EvaluateIn(context);
            PlayerSettings.WebGL.dataCaching = dataCaching.EvaluateIn(context);
#endif
            using(var wrapper = new UTPlayerSettingsWrapper()) {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
                wrapper.SetInt(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "memorySize"), memorySize.EvaluateIn(context));
                wrapper.SetEnum(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "exceptionSupport"), enableExceptions.EvaluateIn(context));
                wrapper.SetBool(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "dataCaching"), dataCaching.EvaluateIn(context));
#endif

                ApplyCommonSettings(wrapper, context);
            }

            if (UTPreferences.DebugMode)
            {
                Debug.Log("WebGL player settings modified.", this);
            }

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Build/Set WebGL Player Settings", false, 261)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsWebGLAction>();
            result.LoadSettings();
        }

        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {
            var wrapper = new UTPlayerSettingsWrapper();
            defaultScreenWidth.StaticValue = PlayerSettings.defaultWebScreenWidth;
            defaultScreenHeight.StaticValue = PlayerSettings.defaultWebScreenHeight;
            runInBackground.StaticValue = PlayerSettings.runInBackground;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
            memorySize.StaticValue = PlayerSettings.WebGL.memorySize;
            enableExceptions.StaticValue = PlayerSettings.WebGL.exceptionSupport;
            dataCaching.StaticValue = PlayerSettings.WebGL.dataCaching;
#else
            memorySize.StaticValue = wrapper.GetInt(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "memorySize"));
            enableExceptions.StaticValue = wrapper.GetEnum<WebGLExceptionSupport>(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "exceptionSupport"));
            dataCaching.StaticValue = wrapper.GetBool(UTPlayerSettingsWrapper.GetPropertyNameForBuildTarget(BuildTargetGroup.WebGL, "dataCaching"));
#endif
            LoadCommonSettings(wrapper);
        }

        public string LoadSettingsUndoText
        {
            get
            {
                return "Load WebGL player settings";
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
                return BuildTarget.WebGL;
            }
        }

        public override bool SupportsStripping
        {
            get {  return true;}
        }

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2) // VR: 5.3
        public override bool SupportsVirtualReality
        {
            get { return false; }
        }
#endif

    }
}

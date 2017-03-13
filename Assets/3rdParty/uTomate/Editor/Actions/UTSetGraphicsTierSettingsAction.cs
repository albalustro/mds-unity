//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
//
// http://www.ancientlightstudios.com
//


#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
namespace AncientLightStudios.uTomate
{
    using System.Collections;
    using API;
    using UnityEditor;
    using UnityEditor.Rendering;
    using UnityEngine;

    [UTDefaultAction]
    [UTActionInfo(actionCategory = "Build", minUnityVersion = "5.5", sinceUTomateVersion = "1.9.0")]
    [UTDoc(title="Set Graphics Tier Settings", description = "Sets the graphics settings for the given graphics tier and target platform.")]
    [UTInspectorGroups(groups=new[]{"General", "Tier Settings"})]
    public class UTSetGraphicsTierSettingsAction : UTAction, UTICanLoadSettingsFromEditor
    {
        // ------------- General ----------------

        [UTDoc(description = "The build target for which the settings should be applied.")]
        [UTInspectorHint(@group = "General", order = 10)]
        public UTBuildTargetGroup buildTarget;

        [UTDoc(description = "The tier for which the settings should be applied.")]
        [UTInspectorHint(group = "General", order = 20)]
        public UTGraphicsTier tier;

        [UTDoc(description = "Should default settings be used for this tier?")]
        [UTInspectorHint(group = "General", order = 30)]
        public UTBool useDefaultSettings;


        // ------------- Tier Settings ----------------

        [UTDoc(description = "The quality level to use for the standard shaders.")]
        [UTInspectorHint(group = "Tier Settings", order = 10)]
        public UTShaderQuality standardShaderQuality;

        [UTDoc(description = "Should shadow maps be cascaded?")]
        [UTInspectorHint(group = "Tier Settings", order = 20)]
        public UTBool cascadedShadowMaps;

        [UTDoc(description = "Should box projection be used on reflection probes?")]
        [UTInspectorHint(group = "Tier Settings", order = 30)]
        public UTBool reflectionProbeBoxProjection;

        [UTDoc(description = "Should reflection probes be blended in the areas where their zones overlap?")]
        [UTInspectorHint(group = "Tier Settings", order = 40)]
        public UTBool reflectionProbeBlending;

        [UTDoc(description = "The rendering path to be used.")]
        [UTInspectorHint(group = "Tier Settings", order = 50,
             captions = new[] { "Legacy Vertex Lit", "Forward", "Legacy Deferred (light prepass)", "Deferred" },
             allowedValues = new[] { "VertexLit", "Forward", "DeferredLighting", "DeferredShading" })]
        public UTRenderingPath renderingPath;

        public override IEnumerator Execute(UTContext context)
        {
            var theBuildTarget = buildTarget.EvaluateIn(context);
            var theGraphicsTier = tier.EvaluateIn(context);

            if (UTPreferences.DebugMode)
            {
                Debug.Log("Applying tier settings for target " + theBuildTarget + " and tier " + theGraphicsTier);
            }

            var doUseDefaultSettings = useDefaultSettings.EvaluateIn(context);
            UTInternalCall.InvokeStatic("UnityEditor.Rendering.EditorGraphicsSettings", "MakeTierSettingsAutomatic",
                theBuildTarget, theGraphicsTier, doUseDefaultSettings);

            if (!doUseDefaultSettings)
            {
                var tierSettings = new TierSettings
                {
                    cascadedShadowMaps = cascadedShadowMaps.EvaluateIn(context),
                    reflectionProbeBlending = reflectionProbeBlending.EvaluateIn(context),
                    reflectionProbeBoxProjection = reflectionProbeBoxProjection.EvaluateIn(context),
                    standardShaderQuality = standardShaderQuality.EvaluateIn(context),
                    renderingPath =  renderingPath.EvaluateIn(context)
                };

                EditorGraphicsSettings.SetTierSettings(theBuildTarget, theGraphicsTier, tierSettings);
            }

            yield break;
        }

        public void LoadSettings()
        {
            if (buildTarget.UseExpression || tier.UseExpression)
            {
                Debug.LogWarning("In order to load tier settings, both the 'Build Target' and 'Tier' properties must not be expressions. Please switch " +
                                 "them to the build target and tier you want to load the settings for and then try again.", this);
                return;
            }

            var theBuildTarget = buildTarget.Value;
            var theTier = tier.Value;

            var doUseDefaultSettings = (bool) UTInternalCall.InvokeStatic("UnityEditor.Rendering.EditorGraphicsSettings",
                "AreTierSettingsAutomatic", theBuildTarget, theTier);


            useDefaultSettings.StaticValue = doUseDefaultSettings;
            var tierSettings = EditorGraphicsSettings.GetTierSettings(theBuildTarget, theTier);

            cascadedShadowMaps.StaticValue = tierSettings.cascadedShadowMaps;
            reflectionProbeBlending.StaticValue = tierSettings.reflectionProbeBlending;
            reflectionProbeBoxProjection.StaticValue = tierSettings.reflectionProbeBoxProjection;
            standardShaderQuality.StaticValue = tierSettings.standardShaderQuality;
            renderingPath.StaticValue = tierSettings.renderingPath;
        }

        public string LoadSettingsUndoText
        {
            get { return "Undo load graphics tier settings"; }
        }


        [MenuItem("Assets/Create/uTomate/Build/Set Graphics Tier Settings", false, 285)]
        public static void AddAction()
        {
            Create<UTSetGraphicsTierSettingsAction>();
        }

    }
}
#endif
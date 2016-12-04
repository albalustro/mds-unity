//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

using UnityEngine;
using System.Collections;

namespace AncientLightStudios.uTomate
{
	using API;
	using System.Collections;
	using UnityEditor;
	using UnityEngine;
	using UObject = UnityEngine.Object;
	using UnityEngine.Rendering;
	
	[UTActionInfo(actionCategory = "Bake", sinceUTomateVersion = "1.5.0")]
	[UTDoc(title = "Set Lighting Settings", description = "Sets the lighting settings for the currently open scene using.")]
	[UTInspectorGroups(groups = new string[] { "Environment Lighting", "Precomputed Realtime GI", "Baked GI", "General GI", "Fog", "Other Settings" })]
	[UTDefaultAction]
	public class UTSetLightingSettingsAction : UTAction, UTICanLoadSettingsFromEditor
	{

		[UTDoc(description = "The skybox material for the lighting.")]
		[UTInspectorHint(required = false, order = 0, group = "Environment Lighting")]
		public UTMaterial skybox;

		[UTDoc(description = "The light used by the skybox. If none, the light from the scene settings will be used.")]
		[UTInspectorHint(required = false, order = 1, group = "Environment Lighting")]
		public UTLight sun;

		[UTDoc(description = "The source of the ambient light.")]
		[UTInspectorHint(required = true, order = 2, group = "Environment Lighting")]
		public UTAmbientMode ambientSource;

		[UTDoc(description = "The color for the ambient light.")]
		[UTInspectorHint(required = true, order = 3, group = "Environment Lighting", indentLevel = 1)]
		public UTColor ambientLightColor;

		[UTDoc(description = "The color for the ambient light coming from above.")]
		[UTInspectorHint(required = true, order = 4, group = "Environment Lighting", indentLevel = 1)]
		public UTColor ambientSkyColor;
		
		[UTDoc(description = "The color for the ambient light coming from the side.")]
		[UTInspectorHint(required = true, order = 5, group = "Environment Lighting", indentLevel = 1)]
		public UTColor ambientEquatorColor;
		
		[UTDoc(description = "The color for the ambient light coming from below.")]
		[UTInspectorHint(required = true, order = 6, group = "Environment Lighting", indentLevel = 1)]
		public UTColor ambientGroundColor;

		[UTDoc(description = "How much the light from the ambient source affects the scene.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0.0f, maxValue = 1f, required = true, order = 7, group = "Environment Lighting")]
		public UTFloat ambientIntensity;

		[UTDoc(description = "The global illumination mode that should be used.")]
		[UTInspectorHint(required = true, order = 8, group = "Environment Lighting")]
		public UTAmbientGI ambientGI;

		[UTDoc(description = "The source for the reflection cubemap.")]
		[UTInspectorHint(required = true, order = 9, group = "Environment Lighting")]
		public UTDefaultReflectionMode reflectionSource;

		[UTDoc(description = "The resolution for the generated reflection cubemap.")]
		[UTInspectorHint(required = true, order = 10, group = "Environment Lighting", allowedValues = new string[] {"Resolution128", "Resolution256", "Resolution512", "Resolution1024"}, captions = new string[]{"128", "256", "512", "1024"}, indentLevel = 1)]
		public UTTextureSize reflectionResolution;

		[UTDoc(description = "The custom reflection cubemap.")]
		[UTInspectorHint(required = true, order = 11, group = "Environment Lighting", indentLevel = 1)]
		public UTCubemap customReflection;

		[UTDoc(description = "How much the reflection affects the scene.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0.0f, maxValue = 1f, required = true, order = 12, group = "Environment Lighting")]
		public UTFloat reflectionIntensity;

		[UTDoc(description = "How many times a reflection reflects another reflection.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 1f, maxValue = 5f, required = true, order = 13, group = "Environment Lighting")]
		public UTInt reflectionBounces;

		[UTDoc(description = "Activate precomputed realtime GI.")]
		[UTInspectorHint(order = 0, group = "Precomputed Realtime GI")]
		public UTBool activateRealtimeGI;

		[UTDoc(description = "Realtime lightmap resolution in texels per world unit.")]
		[UTInspectorHint(required = true, order = 1, group = "Precomputed Realtime GI")]
		public UTFloat realtimeResolution;

		[UTDoc(description = "How much cpu usage to assign to the final lighting calculations at runtime.")]
		[UTInspectorHint(required = true, order = 2, group = "Precomputed Realtime GI")]
		public UTRuntimeCpuUsage cpuUsage;

		[UTDoc(description = "Activate baked GI.")]
		[UTInspectorHint(order = 0, group = "Baked GI")]
		public UTBool activateBakedGI;

		[UTDoc(description = "Baked lightmap resolution in texels per world unit.")]
		[UTInspectorHint(required = true, order = 1, group = "Baked GI")]
		public UTFloat bakedResolution;

		[UTDoc(description = "Texel separation between shapes.")]
		[UTInspectorHint(required = true, order = 2, group = "Baked GI")]
		public UTInt bakedPadding;

		[UTDoc(description = "Whether or not to compress baked lightmap.")]
		[UTInspectorHint(order = 3, group = "Baked GI")]
		public UTBool compressed;

		[UTDoc(description = "Indirect lightmap resolution in texels per world unit.")]
		[UTInspectorHint(required = true, order = 4, group = "Baked GI")]
		public UTFloat indirectResolution;

		[UTDoc(description = "Changes contrast of amibent occlusion.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0.0f, maxValue = 1f, required = true, order = 5, group = "Baked GI")]
		public UTFloat ambientOcclusion;

		[UTDoc(description = "Beyond this distance a ray is considered to be unoccluded.")]
		[UTInspectorHint(minValue = 0.0f, required = true, order = 6, group = "Baked GI", indentLevel = 1)]
		public UTFloat maxDistance;

		[UTDoc(description = "Whether or not to use final gather.")]
		[UTInspectorHint(order = 7, group = "Baked GI")]
		public UTBool finalGather;

		[UTDoc(description = "How many rays to use for final gather per bake output texel")]
		[UTInspectorHint(required = true, order = 8, group = "Baked GI", indentLevel = 1)]
		public UTInt rayCount;

		[UTDoc(description = "Lightmaps encode incoming dominant light direction.")]
		[UTInspectorHint(required = true, order = 0, group = "General GI")]
		public UTLightmapsMode directionalMode;

		[UTDoc(description = "Scales indirect lighting.")]
		[UTInspectorHint(required = true, displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0f, maxValue = 5f, order = 1, group = "General GI")]
		public UTFloat indirectIntensity;
		
		[UTDoc(description = "When light bounces of a surface it is multiplied by the albedo of this surface.")]
		[UTInspectorHint(required = true, displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 1f, maxValue = 10f, order = 2, group = "General GI")]
		public UTFloat bounceBoost;

		[UTDoc(description = "The resolution for the generated reflection cubemap.")]
		[UTInspectorHint(required = true, order = 3, group = "General GI", captions = new string[]{"32", "64", "128", "256", "512", "1024", "2048", "4096"})]
		public UTTextureSize atlasSize;

		[UTDoc(description = "Activate Fog.")]
		[UTInspectorHint(order = 0, group = "Fog")]
		public UTBool activateFog;

		[UTDoc(description = "Fog color.")]
		[UTInspectorHint(required = true, order = 1, group = "Fog")]
		public UTColor fogColor;

		[UTDoc(description = "Fog mode to use.")]
		[UTInspectorHint(required = true, order = 2, group = "Fog")]
		public UTFogMode fogMode;

		[UTDoc(description = "Density of the fog.")]
		[UTInspectorHint(required = true, order = 3, group = "Fog", indentLevel = 1)]
		public UTFloat fogDensity;

		[UTDoc(description = "Start of the fog.")]
		[UTInspectorHint(required = true, order = 4, group = "Fog", indentLevel = 1)]
		public UTFloat fogStart;

		[UTDoc(description = "End of the fog.")]
		[UTInspectorHint(required = true, order = 5, group = "Fog", indentLevel = 1)]
		public UTFloat fogEnd;

		[UTDoc(description = "The light halo texture.")]
		[UTInspectorHint(order = 0, group = "Other Settings")]
		public UTTexture2D haloTexture;

		[UTDoc(description = "The strength of the halo.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0f, maxValue = 1f, order = 1, group = "Other Settings")]
		public UTFloat haloStrength;

		[UTDoc(description = "Fade time for a flare.")]
		[UTInspectorHint(order = 2, group = "Other Settings")]
		public UTFloat flareFadeSpeed;

		[UTDoc(description = "The strength of the flare.")]
		[UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 0f, maxValue = 1f, order = 3, group = "Other Settings")]
		public UTFloat flareStrength;

		[UTDoc(description = "The default spotlight cookie. If none, the light from the scene settings will be used.")]
		[UTInspectorHint(order = 4, group = "Other Settings")]
		public UTTexture2D spotCookie;

		public override IEnumerator Execute (UTContext context)
		{
			// apply
			SerializedObject renderSettings = new SerializedObject(UTInternalCall.InvokeStatic("UnityEngine.RenderSettings", "GetRenderSettings") as UObject);
			SerializedObject lightmapSettings = new SerializedObject(UTInternalCall.InvokeStatic("UnityEditor.LightmapEditorSettings", "GetLightmapSettings") as UObject);

			// environment
			var theSkybox = skybox.EvaluateIn(context);
			renderSettings.FindProperty("m_SkyboxMaterial").objectReferenceValue = theSkybox;
			var theSun = sun.EvaluateIn(context);
			if (theSun != null) {
				// only set the sun if something is selected, otherwise try to use the value from the scene
				renderSettings.FindProperty("m_Sun").objectReferenceValue = theSun;
			}

			var theAmbientSource = ambientSource.EvaluateIn(context);
			renderSettings.FindProperty("m_AmbientMode").intValue = (int) theAmbientSource;
			if (theAmbientSource == AmbientMode.Skybox) {
				if (theSkybox == null) {
					// ambient light color is only required if skybox is empty
					// renderSettings contains a m_AmbientLightColor property, but this is not used by the lighting editor
					renderSettings.FindProperty("m_AmbientSkyColor").colorValue = ambientLightColor.EvaluateIn(context);
				}
			} else if (theAmbientSource == AmbientMode.Gradient) {
				renderSettings.FindProperty("m_AmbientSkyColor").colorValue = ambientSkyColor.EvaluateIn(context);
				renderSettings.FindProperty("m_AmbientEquatorColor").colorValue = ambientEquatorColor.EvaluateIn(context);
				renderSettings.FindProperty("m_AmbientGroundColor").colorValue = ambientGroundColor.EvaluateIn(context);
			} else if (theAmbientSource == AmbientMode.Color) {
				// renderSettings contains a m_AmbientLightColor property, but this is not used by the lighting editor
				renderSettings.FindProperty("m_AmbientSkyColor").colorValue = ambientLightColor.EvaluateIn(context);
			}

			renderSettings.FindProperty("m_AmbientIntensity").floatValue = ambientIntensity.EvaluateIn(context);

			lightmapSettings.FindProperty("m_GISettings.m_EnvironmentLightingMode").intValue = (int) ambientGI.EvaluateIn(context);

			var theReflectionSource = reflectionSource.EvaluateIn(context);
			renderSettings.FindProperty("m_DefaultReflectionMode").intValue = (int)theReflectionSource;
			if (theReflectionSource == UnityEngine.Rendering.DefaultReflectionMode.Skybox) {
				renderSettings.FindProperty("m_DefaultReflectionResolution").intValue = (int) reflectionResolution.EvaluateIn(context);
			} else if (theReflectionSource == UnityEngine.Rendering.DefaultReflectionMode.Custom) {
				renderSettings.FindProperty("m_CustomReflection").objectReferenceValue = customReflection.EvaluateIn(context);
			}

			renderSettings.FindProperty("m_ReflectionIntensity").floatValue = reflectionIntensity.EvaluateIn(context);
			renderSettings.FindProperty("m_ReflectionBounces").intValue = reflectionBounces.EvaluateIn(context);

			// realtime GI
			var theActivateRealtimeGI = activateRealtimeGI.EvaluateIn(context);
			lightmapSettings.FindProperty("m_GISettings.m_EnableRealtimeLightmaps").boolValue = theActivateRealtimeGI;
			if (theActivateRealtimeGI) {
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Resolution").floatValue = realtimeResolution.EvaluateIn(context);
				lightmapSettings.FindProperty("m_RuntimeCPUUsage").intValue = (int) cpuUsage.EvaluateIn(context);
			}

			// bake GI
			var theActivateBakedGI = activateBakedGI.EvaluateIn(context);
			lightmapSettings.FindProperty("m_GISettings.m_EnableBakedLightmaps").boolValue = theActivateBakedGI;
			if (theActivateBakedGI) {
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_BakeResolution").floatValue = bakedResolution.EvaluateIn(context);
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Padding").intValue = bakedPadding.EvaluateIn(context);
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_TextureCompression").boolValue = compressed.EvaluateIn(context);
				if (!theActivateRealtimeGI) {
					lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Resolution").floatValue = indirectResolution.EvaluateIn(context);
				}
				var theAmbientOcclusion = ambientOcclusion.EvaluateIn(context);
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_CompAOExponent").floatValue = theAmbientOcclusion;
				if (theAmbientOcclusion > 0) {
					lightmapSettings.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance").floatValue = maxDistance.EvaluateIn(context);
				}
				var theFinalGather = finalGather.EvaluateIn(context);
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_FinalGather").boolValue = theFinalGather;
				if (theFinalGather) {
					lightmapSettings.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount").intValue = rayCount.EvaluateIn(context);
				}
			}

			// general GI
			if (theActivateRealtimeGI || theActivateBakedGI) {
				lightmapSettings.FindProperty("m_LightmapsMode").intValue = (int) directionalMode.EvaluateIn(context);
				lightmapSettings.FindProperty("m_GISettings.m_IndirectOutputScale").floatValue = indirectIntensity.EvaluateIn(context);
				lightmapSettings.FindProperty("m_GISettings.m_AlbedoBoost").floatValue = bounceBoost.EvaluateIn(context);
				lightmapSettings.FindProperty("m_LightmapEditorSettings.m_TextureWidth").intValue = (int) atlasSize.EvaluateIn(context);
			}

			// fog
			var theActivateFog = activateFog.EvaluateIn(context);
			renderSettings.FindProperty("m_Fog").boolValue = theActivateFog;
			if (theActivateFog) {
				renderSettings.FindProperty("m_FogColor").colorValue = fogColor.EvaluateIn(context);
				var theFogMode = fogMode.EvaluateIn(context);
				renderSettings.FindProperty("m_FogMode").intValue = (int) theFogMode;
				if (theFogMode == FogMode.Linear) {
					renderSettings.FindProperty("m_LinearFogStart").floatValue = fogStart.EvaluateIn(context);
					renderSettings.FindProperty("m_LinearFogEnd").floatValue = fogEnd.EvaluateIn(context);
				} else {
					renderSettings.FindProperty("m_FogDensity").floatValue = fogDensity.EvaluateIn(context);
				}
			}

			// other settings
			var theHaloTexture = haloTexture.EvaluateIn(context);
			if (theHaloTexture != null) {
				renderSettings.FindProperty("m_HaloTexture").objectReferenceValue = theHaloTexture;
			}
			renderSettings.FindProperty("m_HaloStrength").floatValue = haloStrength.EvaluateIn(context);
			renderSettings.FindProperty("m_FlareFadeSpeed").floatValue = flareFadeSpeed.EvaluateIn(context);
			renderSettings.FindProperty("m_FlareStrength").floatValue = flareStrength.EvaluateIn(context);
			var theSpotCookie = spotCookie.EvaluateIn(context);
			if (theSpotCookie != null) {
				renderSettings.FindProperty("m_SpotCookie").objectReferenceValue = theSpotCookie;
			}

			renderSettings.ApplyModifiedProperties();
			lightmapSettings.ApplyModifiedProperties();
			yield return null;
		}

		public enum AmbientMode
		{
			Skybox = 0,
			Gradient = 1,
			Color = 3
		}

		public enum TextureSize 
		{
			Resolution32 = 32,
			Resolution54 = 64,
			Resolution128 = 128,
			Resolution256 = 256,
			Resolution512 = 512,
			Resolution1024 = 1024,
			Resolution2048 = 2048,
			Resolution4096 = 4096
		}

		public enum AmbientGI 
		{
			Realtime = 0,
			Baked = 1
		}

		public enum RuntimeCpuUsage {
			Low = 25,
			Medium = 50,
			High = 75,
			Unlimited = 100
		}

		public enum LightmapsMode {
			NonDirectional = 0,
			Directional = 1,
			DirectionalSpecular = 2
		}

		[MenuItem("Assets/Create/uTomate/Bake/Set Lighting Settings", false, 205)]
		public static void AddAction()
		{
			var action = Create<UTSetLightingSettingsAction>();
			action.LoadSettings();
		}

		public string LoadSettingsUndoText
		{
			get
			{
				return "Load current lighting settings.";
			}
		}

		public void LoadSettings()
		{
			var action = this;
			SerializedObject renderSettings = new SerializedObject(UTInternalCall.InvokeStatic("UnityEngine.RenderSettings", "GetRenderSettings") as UObject);
			SerializedObject lightmapSettings = new SerializedObject(UTInternalCall.InvokeStatic("UnityEditor.LightmapEditorSettings", "GetLightmapSettings") as UObject);
			
			// environment
			action.skybox.Value = (Material) renderSettings.FindProperty("m_SkyboxMaterial").objectReferenceValue;
			action.skybox.UseExpression = false;
			action.ambientSource.Value = (AmbientMode) renderSettings.FindProperty("m_AmbientMode").intValue;
			action.ambientSource.UseExpression = false;
			action.ambientLightColor.Value = renderSettings.FindProperty("m_AmbientSkyColor").colorValue;
			action.ambientLightColor.UseExpression = false;
			action.ambientSkyColor.Value = renderSettings.FindProperty("m_AmbientSkyColor").colorValue;
			action.ambientSkyColor.UseExpression = false;
			action.ambientEquatorColor.Value = renderSettings.FindProperty("m_AmbientEquatorColor").colorValue;
			action.ambientEquatorColor.UseExpression = false;
			action.ambientGroundColor.Value = renderSettings.FindProperty("m_AmbientGroundColor").colorValue;
			action.ambientGroundColor.UseExpression = false;
			action.ambientIntensity.Value = renderSettings.FindProperty("m_AmbientIntensity").floatValue;
			action.ambientIntensity.UseExpression = false;
			action.ambientGI.Value =(AmbientGI) lightmapSettings.FindProperty("m_GISettings.m_EnvironmentLightingMode").intValue;
			action.ambientGI.UseExpression = false;
			action.reflectionSource.Value = (DefaultReflectionMode) renderSettings.FindProperty("m_DefaultReflectionMode").intValue;
			action.reflectionSource.UseExpression = false;
			action.reflectionResolution.Value = (TextureSize) renderSettings.FindProperty("m_DefaultReflectionResolution").intValue;
			action.reflectionResolution.UseExpression = false;
			action.customReflection.Value = (Cubemap) renderSettings.FindProperty("m_CustomReflection").objectReferenceValue;
			action.customReflection.UseExpression = false;
			action.reflectionIntensity.Value = renderSettings.FindProperty("m_ReflectionIntensity").floatValue;
			action.reflectionIntensity.UseExpression = false;
			action.reflectionBounces.Value = renderSettings.FindProperty("m_ReflectionBounces").intValue;
			action.reflectionBounces.UseExpression = false;
			
			// realtime GI
			action.activateRealtimeGI.Value = lightmapSettings.FindProperty("m_GISettings.m_EnableRealtimeLightmaps").boolValue;
			action.activateRealtimeGI.UseExpression = false;
			action.realtimeResolution.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Resolution").floatValue;
			action.realtimeResolution.UseExpression = false;
			action.cpuUsage.Value = (RuntimeCpuUsage) lightmapSettings.FindProperty("m_RuntimeCPUUsage").intValue;
			action.cpuUsage.UseExpression = false;

			// bake GI
			action.activateBakedGI.Value = lightmapSettings.FindProperty("m_GISettings.m_EnableBakedLightmaps").boolValue;
			action.activateBakedGI.UseExpression = false;
			action.bakedResolution.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_BakeResolution").floatValue;
			action.bakedResolution.UseExpression = false;
			action.bakedPadding.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Padding").intValue;
			action.bakedPadding.UseExpression = false;
			action.compressed.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_TextureCompression").boolValue;
			action.compressed.UseExpression = false;
			action.indirectResolution.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_Resolution").floatValue;
			action.indirectResolution.UseExpression = false;
			action.ambientOcclusion.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_CompAOExponent").floatValue;
			action.ambientOcclusion.UseExpression = false;
			action.maxDistance.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance").floatValue;
			action.maxDistance.UseExpression = false;
			action.finalGather.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_FinalGather").boolValue;
			action.finalGather.UseExpression = false;
			action.rayCount.Value = lightmapSettings.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount").intValue;
			action.rayCount.UseExpression = false;
			
			// general GI
			action.directionalMode.Value = (LightmapsMode) lightmapSettings.FindProperty("m_LightmapsMode").intValue;
			action.directionalMode.UseExpression = false;
			action.indirectIntensity.Value = lightmapSettings.FindProperty("m_GISettings.m_IndirectOutputScale").floatValue;
			action.indirectIntensity.UseExpression = false;
			action.bounceBoost.Value = lightmapSettings.FindProperty("m_GISettings.m_AlbedoBoost").floatValue;
			action.bounceBoost.UseExpression = false;
			action.atlasSize.Value = (TextureSize) lightmapSettings.FindProperty("m_LightmapEditorSettings.m_TextureWidth").intValue;
			action.atlasSize.UseExpression = false;
			
			// fog
			action.activateFog.Value = renderSettings.FindProperty("m_Fog").boolValue;
			action.activateFog.UseExpression = false;
			action.fogColor.Value = renderSettings.FindProperty("m_FogColor").colorValue;
			action.fogColor.UseExpression = false;
			action.fogMode.Value = (FogMode) renderSettings.FindProperty("m_FogMode").intValue;
			action.fogMode.UseExpression = false;
			action.fogStart.Value = renderSettings.FindProperty("m_LinearFogStart").floatValue;
			action.fogStart.UseExpression = false;
			action.fogEnd.Value = renderSettings.FindProperty("m_LinearFogEnd").floatValue;
			action.fogEnd.UseExpression = false;
			action.fogDensity.Value = renderSettings.FindProperty("m_FogDensity").floatValue;
			action.fogDensity.UseExpression = false;

			// other settings
			action.haloTexture.Value = (Texture2D) renderSettings.FindProperty("m_HaloTexture").objectReferenceValue;
			action.haloTexture.UseExpression = false;
			action.haloStrength.Value = renderSettings.FindProperty("m_HaloStrength").floatValue;
			action.haloStrength.UseExpression = false;
			action.flareFadeSpeed.Value = renderSettings.FindProperty("m_FlareFadeSpeed").floatValue;
			action.flareFadeSpeed.UseExpression = false;
			action.flareStrength.Value = renderSettings.FindProperty("m_FlareStrength").floatValue;
			action.flareStrength.UseExpression = false;
			action.spotCookie.Value = (Texture2D) renderSettings.FindProperty("m_SpotCookie").objectReferenceValue;
			action.spotCookie.UseExpression = false;
		}
	}
}

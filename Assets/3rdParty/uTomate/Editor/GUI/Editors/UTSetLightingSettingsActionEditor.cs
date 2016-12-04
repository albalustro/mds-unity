//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
	using API;
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(UTSetLightingSettingsAction))]
	public class UTSetLightingSettingsActionEditor : UTInspectorBase
	{
		public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
		{
			var action = target as UTSetLightingSettingsAction;
			var ambientSourceUnknown = action.ambientSource.UseExpression;
			var ambientSource = action.ambientSource.Value;
			var reflectionSourceUnknown = action.reflectionSource.UseExpression;
			var reflectionSource = action.reflectionSource.Value;
			var activateRealtimeGIUnknown = action.activateRealtimeGI.UseExpression;
			var activateRealtimeGI = action.activateRealtimeGI.Value;
			var activateBakedGIUnknown = action.activateBakedGI.UseExpression;
			var activateBakedGI = action.activateBakedGI.Value;
			var activateFogUnknown = action.activateFog.UseExpression;
			var activateFog = action.activateFog.Value;

			switch (fieldInfo.Name)
			{
			case "ambientLightColor":
				if (ambientSourceUnknown) {
					// display field if mode is in f(x) state, because we don't know what the user wants
					return UTVisibilityDecision.Visible;
				}
				if (ambientSource == UTSetLightingSettingsAction.AmbientMode.Skybox) {
					// if source is set to skybox then this field is only visible if no skybox is selected 
					return (action.skybox.UseExpression || action.skybox.Value == null) ? 
						UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
				}

				if (ambientSource == UTSetLightingSettingsAction.AmbientMode.Color) {
					// field is visible if source ist set to color
					return UTVisibilityDecision.Visible;
				}

				// otherwise hide field
				return UTVisibilityDecision.Invisible;
			case "ambientSkyColor":
			case "ambientEquatorColor":
			case "ambientGroundColor":
				return ambientSourceUnknown || ambientSource == UTSetLightingSettingsAction.AmbientMode.Gradient ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "ambientGI":
				return activateRealtimeGIUnknown || activateBakedGIUnknown || (activateRealtimeGI && activateBakedGI) ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "reflectionResolution":
				return reflectionSourceUnknown || reflectionSource == UnityEngine.Rendering.DefaultReflectionMode.Skybox ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "customReflection":
				return reflectionSourceUnknown || reflectionSource == UnityEngine.Rendering.DefaultReflectionMode.Custom ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "realtimeResolution":
			case "cpuUsage":
				return activateRealtimeGIUnknown || activateRealtimeGI ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "bakedResolution":
			case "bakedPadding":
			case "compressed":
			case "indirectResolution":
			case "ambientOcclusion":
			case "maxDistance":
			case "finalGather":
			case "rayCount":
				if (!activateBakedGIUnknown && !activateBakedGI) {
					return UTVisibilityDecision.Invisible;
				}
				if (fieldInfo.Name == "indirectResolution") {
					return !activateRealtimeGIUnknown && !activateRealtimeGI ? 
						UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
				} else if (fieldInfo.Name == "maxDistance") {
					if (!action.ambientOcclusion.UseExpression && action.ambientOcclusion.Value == 0) {
						return UTVisibilityDecision.Invisible;
					}
				} else if (fieldInfo.Name == "rayCount") {
					if (!action.finalGather.UseExpression && !action.finalGather.Value) {
						return UTVisibilityDecision.Invisible;
					}
				}

				return UTVisibilityDecision.Visible;
			case "directionalMode":
			case "indirectIntensity":
			case "bounceBoost":
			case "atlasSize":
				return activateRealtimeGIUnknown || activateRealtimeGI || activateBakedGIUnknown || activateBakedGI ? 
					UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
			case "fogColor":
			case "fogMode":
			case "fogDensity":
			case "fogStart":
			case "fogEnd":
				if (!activateFogUnknown && !activateFog) {
					return UTVisibilityDecision.Invisible;
				}

				if (fieldInfo.Name == "fogDensity") {
					if (!action.fogMode.UseExpression && action.fogMode.Value == FogMode.Linear) {
						return UTVisibilityDecision.Invisible;
					}
				}

				if (fieldInfo.Name == "fogStart" || fieldInfo.Name == "fogEnd") {
					if (!action.fogMode.UseExpression && action.fogMode.Value != FogMode.Linear) {
						return UTVisibilityDecision.Invisible;
					}
				}

				return UTVisibilityDecision.Visible;
			default:
				return base.IsVisible(fieldInfo);
			}
		}
	}
}

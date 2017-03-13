//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;

    [CustomEditor(typeof(UTSetGraphicsTierSettingsAction))]
    public class UTSetGraphicsTierSettingsActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTSetGraphicsTierSettingsAction)target;
            switch (fieldInfo.Name)
            {
                case "standardShaderQuality":
                case "cascadedShadowMaps":
                case "reflectionProbeBoxProjection":
                case "reflectionProbeBlending":
                case "renderingPath":
                    return VisibleIf(self.useDefaultSettings.HasValueOrExpression(false));
            }
            return base.IsVisible(fieldInfo);

        }
    }
}
#endif
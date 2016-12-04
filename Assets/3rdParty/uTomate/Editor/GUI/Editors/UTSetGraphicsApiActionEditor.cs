//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1_0 // VR: 5.1.1

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Reflection;
    using API;
    using UnityEditor;
    using UnityEngine.Rendering;

    [CustomEditor(typeof (UTSetGraphicsApiAction))]
    public class UTSetGraphicsApiActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(FieldInfo fieldInfo)
        {
            var self = (UTSetGraphicsApiAction) target;
            if (fieldInfo.Name.Equals("graphicsApis"))
            {
                return self.automaticGraphicsApi.HasValueOrExpression(false) ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            return base.IsVisible(fieldInfo);
        }

        public override string[] AllowedValues(string field)
        {
            var self = (UTSetGraphicsApiAction) target;
            if (field.Equals("graphicsApis"))
            {
                if (self.automaticGraphicsApi.HasValueOrExpression(true)) // if it is set to expression mode or automatic, we don't provide any options.
                {
                    return null;
                }
                if (self.buildTarget.UseExpression) // if the build target isn't known, we cannot provide a list
                {
                    return null;
                }

                var result = (GraphicsDeviceType[]) UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings", "GetSupportedGraphicsAPIs", self.buildTarget.Value);
                return Array.ConvertAll(result, input => Enum.GetName(typeof (GraphicsDeviceType), input));
            }
            return base.AllowedValues(field);
        }
    }
}

#endif

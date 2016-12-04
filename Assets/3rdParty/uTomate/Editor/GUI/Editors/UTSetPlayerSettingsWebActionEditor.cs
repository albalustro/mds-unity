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

    [CustomEditor(typeof(UTSetPlayerSettingsWebAction))]
    public class UTSetPlayerSettingsWebActionEditor : UTSetPlayerSettingsBaseActionEditor
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (fieldInfo.Name)
            { 
                case "virtualRealitySplashImage":
                case "showUnitySplashScreen":
                case "splashScreenStyle":
                    return UTVisibilityDecision.Invisible;
            }

            return base.IsVisible(fieldInfo);
        }
    }
}

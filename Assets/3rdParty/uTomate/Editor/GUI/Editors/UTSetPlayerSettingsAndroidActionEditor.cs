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

    [CustomEditor(typeof(UTSetPlayerSettingsAndroidAction))]
    public class UTSetPlayerSettingsAndroidActionEditor : UTSetPlayerSettingsBaseActionEditor
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTSetPlayerSettingsAndroidAction)target;

            if (fieldInfo.Name.StartsWith("iconSize"))
            {
                return VisibleIf(self.overrideIconForAndroid.HasValueOrExpression(true));
            }
            
            if (fieldInfo.Name.StartsWith("banner")) {
                return VisibleIf(self.enableAndroidBanner.HasValueOrExpression(true));
            }

            return base.IsVisible(fieldInfo);
        }
    }
}

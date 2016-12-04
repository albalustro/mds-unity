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

    [CustomEditor(typeof(UTSetPlayerSettingsPcMacAction))]
    public class UTSetPlayerSettingsPcMacActionEditor : UTSetPlayerSettingsBaseActionEditor
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTSetPlayerSettingsPcMacAction)target;

            if (fieldInfo.Name.StartsWith("iconSize"))
            {
                return VisibleIf(self.overrideIconForStandalone.HasValueOrExpression(true));
            }

#if !UNITY_5_0 // VR: 5.1    
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (fieldInfo.Name)
            {
                case "singlePassStereoscopicRendering":
                    return VisibleIf(self.virtualRealitySupported.HasValueOrExpression(true));
            }
#endif

            return base.IsVisible(fieldInfo);
        }
    }
}

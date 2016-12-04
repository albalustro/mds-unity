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

    [CustomEditor(typeof(UTSetPlayerSettingsIosAction))]
    public class UTSetPlayerSettingsIosActionEditor : UTSetPlayerSettingsBaseActionEditor
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTSetPlayerSettingsIosAction)target;
            
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (fieldInfo.Name)
            {
                case "useIl2CppPrecompiledHeader":
                   return VisibleIf(self.scriptingBackend.HasValueOrExpression(ScriptingImplementation.IL2CPP));
                case "iconSize":
                    return VisibleIf(self.overrideIconForIphone.HasValueOrExpression(true));
                case "launchScreenPortraitImage":
                case "launchScreenLandscapeImage":
                case "launchScreenBackgroundColor":
#if UNITY_5_0 // VR: [5.0, 5.1)
				    return VisibleIf(self.iPhoneLaunchScreen.HasValueOrExpression(UTSetPlayerSettingsIosAction.IosLaunchScreenType.ImageAndBackgroundRelative));
#else // VR: 5.1
                    return VisibleIf(self.iPhoneLaunchScreen.HasValueOrExpression(UTSetPlayerSettingsIosAction.IosLaunchScreenType.ImageAndBackgroundRelative,
                                                                            UTSetPlayerSettingsIosAction.IosLaunchScreenType.ImageAndBackgroundConstant));
#endif
                case "customXibPath":
                    return VisibleIf(self.iPhoneLaunchScreen.HasValueOrExpression(UTSetPlayerSettingsIosAction.IosLaunchScreenType.CustomXib));
                case "launchScreenFillPercentage":
                    return VisibleIf(self.iPhoneLaunchScreen.HasValueOrExpression(UTSetPlayerSettingsIosAction.IosLaunchScreenType.ImageAndBackgroundRelative));

#if !UNITY_5_0 // VR: 5.1
                case "launchScreenSizeInPoints":
                    return VisibleIf(self.iPhoneLaunchScreen.HasValueOrExpression(UTSetPlayerSettingsIosAction.IosLaunchScreenType.ImageAndBackgroundConstant));
#endif
                case "useAnimatedAutoRotation":
                    return VisibleIf(self.defaultOrientation.HasValueOrExpression(UIOrientation.AutoRotation));
            }

            return base.IsVisible(fieldInfo);
        }
    }
}

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

    [CustomEditor(typeof(UTBuildXcodeProjectAction))]
    public class UTBuildXcodeProjectActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            UTXcodeVersion xcodeVersion;
            UTBool useExportOptionsPlist;
            switch (fieldInfo.Name)
            {
                case "developmentTeamId":
                case "useExportOptionsPlist":
                    // only show these fields for xcode8+
                    xcodeVersion = ((UTBuildXcodeProjectAction)target).xcodeVersion;
                    if (xcodeVersion.UseExpression || xcodeVersion.Value == XcodeVersion.Xcode8)
                    {
                        return UTVisibilityDecision.Visible;
                    }
                    return UTVisibilityDecision.Invisible;
                case "provisioningProfile":
                case "outputFile":
                    // always show these fields for xcode7
                    xcodeVersion = ((UTBuildXcodeProjectAction)target).xcodeVersion;
                    if (!xcodeVersion.UseExpression && xcodeVersion.Value == XcodeVersion.Xcode7)
                    {
                        return UTVisibilityDecision.Visible;
                    }

                    // only if checkbox is not marked or in expression mode
                    useExportOptionsPlist = ((UTBuildXcodeProjectAction) target).useExportOptionsPlist;
                    if (useExportOptionsPlist.UseExpression || !useExportOptionsPlist.Value)
                    {
                        return UTVisibilityDecision.Visible;
                    }
                    return UTVisibilityDecision.Invisible;
                case "exportOptionsPlist":
                case "outputDirectory":
                    // never show these fields for xcode7
                    xcodeVersion = ((UTBuildXcodeProjectAction)target).xcodeVersion;
                    if (!xcodeVersion.UseExpression && xcodeVersion.Value == XcodeVersion.Xcode7)
                    {
                        return UTVisibilityDecision.Invisible;
                    }

                    useExportOptionsPlist = ((UTBuildXcodeProjectAction) target).useExportOptionsPlist;
                    if (useExportOptionsPlist.UseExpression || useExportOptionsPlist.Value)
                    {
                        return UTVisibilityDecision.Visible;
                    }
                    return UTVisibilityDecision.Invisible;
            }
            return base.IsVisible(fieldInfo);
        }
    }
}

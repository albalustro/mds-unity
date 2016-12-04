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

    [CustomEditor(typeof(UTCopyFilesAction))]
    public class UTCopyFilesActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTCopyFilesAction)target;
            if (fieldInfo.Name == "onlyIfNewer")
            {
                return VisibleIf(self.overwriteExisting.HasValueOrExpression(true));
            }
            if (fieldInfo.Name == "deleteSourceWhenNotMoved") 
            {
                return VisibleIf(
                    self.moveFiles.HasValueOrExpression(true) && 
                    (
                        self.overwriteExisting.HasValueOrExpression(false) || 
                        (self.overwriteExisting.HasValueOrExpression(true) && self.onlyIfNewer.HasValueOrExpression(true))
                    ));
            }
            
            return base.IsVisible(fieldInfo);
        }
    }
}

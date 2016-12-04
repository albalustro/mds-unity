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

    [CustomEditor(typeof(UTFindGameObjectsAction))]
    public class UTFindGameObjectsActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            var self = (UTFindGameObjectsAction)target;
            switch (fieldInfo.Name)
            {
                case "tags":
                    return self.mode == UTFindGameObjectMode.ByTag ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "names":
                    return self.mode == UTFindGameObjectMode.ByName ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "layers":
                    return self.mode == UTFindGameObjectMode.ByLayer ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "staticFlags":
                    return self.mode == UTFindGameObjectMode.IsStatic ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "activeStatus":
                    return self.mode == UTFindGameObjectMode.IsActive ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "prefabs":
                    return self.mode == UTFindGameObjectMode.ByPrefab ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "components":
                    return self.mode == UTFindGameObjectMode.ByComponent ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "currentGameObjectProperty":
                case "expressions":
                    return self.mode == UTFindGameObjectMode.ByExpression ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;

                case "position":
                case "distance":
                    return self.mode == UTFindGameObjectMode.ByPosition ? UTVisibilityDecision.Visible : UTVisibilityDecision.Invisible;
            }

            return base.IsVisible(fieldInfo);
        }
    }
}

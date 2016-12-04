namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;

    [CustomEditor(typeof(UTUploadToHockeyAppAction))]
    public class UTUploadToHockeyAppActionEditor : UTInspectorBase
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo field)
        {
            var action = (UTUploadToHockeyAppAction)target;

            switch (field.Name)
            {
                case "appID":
                case "versionNumber":
                case "shortVersionNumber":
                    return VisibleIf(action.customBuild.HasValueOrExpression(true));
            }

            return base.IsVisible(field);
        }
    }
}

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;

    [CustomEditor(typeof(UTSetPlayerSettingsWebGLAction))]
    public class UTSetPlayerSettingsWebGLActionEditor : UTSetPlayerSettingsBaseActionEditor
    {
        public override UTVisibilityDecision IsVisible(System.Reflection.FieldInfo fieldInfo)
        {
            // We don't need this for now.
            //   var self = (UTSetPlayerSettingsWebGLAction)target;

         

            return base.IsVisible(fieldInfo);
        }
    }
}

namespace AncientLightStudios.uTomate
{
    using System.Collections;
    using API;
    using UnityEditor;

    [UTDoc(title = "Exit Unity Editor", description = "Exits the Unity editor with a defined exit code.")]
    [UTActionInfo(sinceUTomateVersion = "1.8.0", actionCategory = "Run")]
    [UTInspectorGroups(groups = new[] {"General"})]
    [UTDefaultAction]
    public class UTExitUnityEditorAction : UTAction
    {
        [UTDoc(description = "Return code passed to operating system on application quit.")]
        [UTInspectorHint(required = false, order = 1, @group = "General")]
        public UTInt returnCode;

        public override IEnumerator Execute(UTContext context)
        {
            var theReturnCode = returnCode.EvaluateIn(context);

            EditorApplication.Exit(theReturnCode);

            yield break;
        }

        [MenuItem("Assets/Create/uTomate/Run/Exit Unity Editor", false, 2100)]
        public static void AddAction()
        {
            Create<UTExitUnityEditorAction>();
        }
    }
}

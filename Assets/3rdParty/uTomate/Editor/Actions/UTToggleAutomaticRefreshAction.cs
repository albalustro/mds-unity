//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using System.Collections;
    using UnityEditor;

    [UTActionInfo(actionCategory = "Import & Export", sinceUTomateVersion = "1.3.5")]
    [UTDoc(title = "Toggle automatic refresh", description = "Toggles automatic refresh of external modifications.")]
    [UTDefaultAction]
    public class UTToggleAutomaticRefreshAction : UTAction
    {
        [UTDoc(description = "When ticked, enables automatic refresh, otherwise disables it.")]
        public UTBool automaticRefresh;

        public override IEnumerator Execute(UTContext context)
        {
            var currentState = EditorPrefs.GetBool("kAutoRefresh");
            var desiredState = automaticRefresh.EvaluateIn(context);

            if (currentState != desiredState)
            {
                EditorPrefs.SetBool("kAutoRefresh", desiredState);
            }
            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Import + Export/Toggle Automatic Refresh", false, 410)]
        public static void AddAction()
        {
            Create<UTToggleAutomaticRefreshAction>();
        }
    }
}

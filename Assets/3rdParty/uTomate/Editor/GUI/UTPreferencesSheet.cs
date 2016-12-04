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

    public class UTPreferencesSheet
    {
        [PreferenceItem("uTomate")]
        public static void OnGui()
        {
            EditorGUILayout.HelpBox("Should uTomate print a warning if any selector " +
                    "in the 'Includes' or 'Excludes' list of an action doesn't match any item?", MessageType.None);
            UTPreferences.WarnAboutEmptySelector = EditorGUILayout.Toggle("Warn on empty selectors", UTPreferences.WarnAboutEmptySelector);
        }
    }
}

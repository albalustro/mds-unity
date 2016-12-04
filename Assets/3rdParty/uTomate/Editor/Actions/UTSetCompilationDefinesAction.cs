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
    using System.IO;
    using UnityEditor;

    [UTDoc(title = "Set Scripting Define Symbols", description = "Sets scripting preprocessor define symbols.")]
    [UTActionInfo(actionCategory = "Build", sinceUTomateVersion = "1.2.0")]
    [UTDefaultAction]
    public class UTSetCompilationDefinesAction : UTAction
    {
        [UTInspectorHint(required = true, order = 0)]
        [UTDoc(description = "The target group for which the defines should be set.")]
        public UTBuildTargetGroup buildTargetGroup;

        [UTInspectorHint(required = true, order = 1)]
        [UTDoc(description = "The defines to be set. Put one define per line. If the list is empty all currently set defines will be unset.")]
        public UTString[] defines;

        public override IEnumerator Execute(UTContext context)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup.EvaluateIn(context), string.Join(";", EvaluateAll(defines, context)));
            yield return "";
        }

        private void DeleteFileWithMeta(string path)
        {
            SafeDelete(path);
            SafeDelete(path + ".meta");
        }

        private void SafeDelete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        [MenuItem("Assets/Create/uTomate/Build/Scripting Define Symbols", false, 490)]
        public static void AddAction()
        {
            Create<UTSetCompilationDefinesAction>();
        }
    }
}

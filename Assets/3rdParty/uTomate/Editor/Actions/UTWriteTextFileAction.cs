//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text.RegularExpressions;
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(sinceUTomateVersion = "1.6.0", actionCategory = "Files & Folders")]
    [UTDoc(title = "Write Text File", description = "Writes some text to a file.")]
    [UTInspectorGroups(groups = new[] {"General"})]
    [UTDefaultAction]
    public class UTWriteTextFileAction : UTAction
    {
        [UTDoc(description = "The file to which the text shuold be written.")]
        [UTInspectorHint(order = 1, group = "General", displayAs = UTInspectorHint.DisplayAs.SaveFileSelect, required = true)]
        public UTString file;

        [UTDoc(description = "The line ending that should be used for the text file.")]
        [UTInspectorHint(order = 2, group = "General")]
        public UTLineEnding lineEnding;

        [UTDoc(description = "The text that should be written to the file.")]
        [UTInspectorHint(order = 3, group = "General", displayAs = UTInspectorHint.DisplayAs.TextArea)]
        public UTString text;


        private static readonly Regex NewLine = new Regex("\r\n?|\n");

        public override IEnumerator Execute(UTContext context)
        {
            var theFile = file.EvaluateIn(context);
            if (string.IsNullOrEmpty(theFile))
            {
                throw new UTFailBuildException("Please specify a file to which the text should be written.", this);
            }

            UTFileUtils.EnsureParentFolderExists(theFile);
            var theText = text.EvaluateIn(context);


            var theLineEnding = lineEnding.EvaluateIn(context);
            var lineEndingString = "\n";
            if (theLineEnding == LineEnding.Windows)
            {
                lineEndingString = "\r\n";
            }

            if (theLineEnding == LineEnding.Unix)
            {
                lineEndingString = "\n";
            }

            theText = NewLine.Replace(theText, lineEndingString);
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Writing text to '" + theFile + "' using '" + Enum.GetName(typeof (LineEnding), theLineEnding) + "' line endings.", this);
            }

            File.WriteAllText(theFile, theText);
            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Files + Folders/Write Text File", false, 270)]
        public static void AddAction()
        {
            Create<UTWriteTextFileAction>();
        }
    }
}

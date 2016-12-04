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
    using UnityEngine;

    [UTDoc(title = "Echo", description = "Writes the given text to the console.")]
    [UTActionInfo(actionCategory = "General")]
    [UTDefaultAction]
    public class UTEchoAction : UTAction
    {
        [UTInspectorHint(required = true, order = 0)]
        [UTDoc(description = "The text to be written.")]
        public UTString text;

        [UTDoc(description = "Whether the text should be a normal text, a warning (yellow) or an error (red).")]
        [UTInspectorHint(order = 1)]
        public UTLogType logType;


        [UTDoc(description = "Only write to console when in debug mode.")]
        [UTInspectorHint(order = 1)]
        public UTBool onlyInDebugMode;

        public override IEnumerator Execute(UTContext context)
        {
            var debugMode = onlyInDebugMode.EvaluateIn(context);
            var theLogType = logType.EvaluateIn(context);

            if (!debugMode || UTPreferences.DebugMode)
            {
                var evaluatedText = text.EvaluateIn(context);
                switch (theLogType)
                {
                    case LogType.Normal:
                        Debug.Log(evaluatedText, this);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(evaluatedText, this);
                        break;
                    case LogType.Error:
                        Debug.LogError(evaluatedText, this);
                        break;
                    default:
                        // shouldn't happen
                        throw new System.ArgumentOutOfRangeException("logType");
                }
            }
            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/General/Echo", false, 240)]
        public static void AddAction()
        {
            Create<UTEchoAction>();
        }

        public enum LogType
        {
            Normal,
            Warning,
            Error,
        }
    }
}

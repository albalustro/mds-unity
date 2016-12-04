//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using System.Collections;

    /// <summary>
    /// An entry which allows to add notes to the automation plan. It has no function.
    /// </summary>
    [Serializable]
    [UTDoc(title = "Note", description = "Allows you to place notes into your automation plan.")]
    public class UTAutomationPlanNoteEntry : UTAutomationPlanEntryBase
    {
        [UTDoc(description = "The note's text.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.TextArea)]
        public string text = "";

        public override string Label
        {
            get
            {
                return "Note";
            }
        }

        public override string Text
        {
            get
            {
                return text;
            }
        }

        public override IEnumerator Execute(UTContext context)
        {
            yield return "";
        }
    }
}

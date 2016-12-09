using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;

namespace MDS.Actions
{
    [Serializable]
    public class OpenDialogAction : BaseAction
    {
        public Slug[] slugs { get; set; }

        public override IEnumerator Execute()
        {
            yield return base.Execute();

            DialogueSystem.instance.ShowDialogueMessage(slugs);
            yield return new WaitWhile(DialogueSystem.instance.IsDialogueOpen);
        }
    }
}
